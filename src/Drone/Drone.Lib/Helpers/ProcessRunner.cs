using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Drone.Lib.Helpers
{
	public class ProcessRunner : IDisposable
	{
		private bool isDisposed;
		private bool isRunning;
		private Process process;

		private Task processReadStandardOutputTask;
		private Task processReadStandardErrorTask;
		private Task processDispatchOutputEventsTask;
		private CancellationTokenSource processReadStandardOutputTokenSource;
		private CancellationTokenSource processReadStandardErrorTokenSource;
		private CancellationTokenSource processDispatchOutputEventsTokenSource;
		private ConcurrentQueue<ProcessRunnerOutputEventArgs> processOutputQueue;
		
		private int? exitCode;
		private DateTime? exitTimestamp;

		private readonly object sync = new object();

		private readonly string filename;
		private readonly string args;
		private readonly string workDir;
		private readonly bool redirectStreams;

		public ProcessRunner(string filename, string args, string workDir = null, bool redirectStreams = false)
		{
			if(string.IsNullOrWhiteSpace(filename))
				throw new ArgumentException("filename is empty or null", "filename");

			this.filename = filename;
			this.args = args;
			this.workDir = workDir;
			this.redirectStreams = redirectStreams;
			this.processOutputQueue = new ConcurrentQueue<ProcessRunnerOutputEventArgs>();
		}

		public event ProcessRunnerOutputEventHandler OutputRecevied;

		public event EventHandler Exited;

		private void NotifyProcessOutputReceived(ProcessRunnerOutputEventArgs e)
		{
			this.CheckIsDisposed();

			var handler = this.OutputRecevied;

			if(handler == null)
				return;

			handler(this, e);
		}

		private void NotifyProcessExited()
		{
			this.CheckIsDisposed();

			var handler = this.Exited;

			if (handler == null)
				return;

			handler(this, EventArgs.Empty);
		}

		public void Start()
		{
			this.CheckIsDisposed();

			lock(this.sync)
			{
				if(this.isRunning)
					return;

				this.isRunning = true;
				this.StartProcess();
			}
		}

		public ProcessRunnerResult WaitForExit(TimeSpan ts)
		{
			this.CheckIsDisposed();

			if(this.process == null)
				return null;

			var hasProcessExited = this.process.WaitForExit((int) ts.TotalMilliseconds);

			if(!hasProcessExited)
				return null;

			if(this.redirectStreams)
			{
				this.processReadStandardOutputTask.Wait(ts);

				var hasReadStandardOutputTaskEnded = this.processReadStandardOutputTask.IsCompleted;

				if (!hasReadStandardOutputTaskEnded)
					return null;

				this.processReadStandardErrorTask.Wait(ts);

				var hasReadStandardErrorTaskEnded = this.processReadStandardErrorTask.IsCompleted;

				if (!hasReadStandardErrorTaskEnded)
					return null;

				this.processDispatchOutputEventsTask.Wait(ts);

				var hasDispatchOutputTaskEnded = this.processDispatchOutputEventsTask.IsCompleted;

				if (!hasDispatchOutputTaskEnded)
					return null;	
			}

			if(this.exitCode == null)
				throw new Exception("unable to get exit code even though process has stopped");

			if(this.exitTimestamp == null)
				throw new Exception("unable to get exit timestamp even though process has stopped");

			return new ProcessRunnerResult(this.exitTimestamp.Value, this.exitCode.Value);
		}

		public ProcessRunnerResult WaitForExit()
		{
			return this.WaitForExit(TimeSpan.FromMilliseconds(Timeout.Infinite));
		}

		private void StartProcess()
		{
			try
			{
				this.process = new Process();
				this.process.EnableRaisingEvents = true;

				var psi = new ProcessStartInfo();
				this.process.StartInfo = psi;

				psi.FileName = this.filename;
				psi.Arguments = this.args;
				psi.CreateNoWindow = true;
				psi.RedirectStandardError = this.redirectStreams;
				psi.RedirectStandardInput = this.redirectStreams;
				psi.RedirectStandardOutput = this.redirectStreams;
				psi.UseShellExecute = false;

				if(this.workDir != null)
					psi.WorkingDirectory = this.workDir;

				if(this.redirectStreams)
				{
					this.processReadStandardOutputTokenSource = new CancellationTokenSource();

					this.processReadStandardOutputTask = new Task(
						this.ProcessReadStandardOutputTaskCore,
						TaskCreationOptions.LongRunning);

					this.processReadStandardErrorTokenSource = new CancellationTokenSource();

					this.processReadStandardErrorTask = new Task(
						this.ProcessReadStandardErrorTaskCore,
						TaskCreationOptions.LongRunning);

					this.processDispatchOutputEventsTokenSource = new CancellationTokenSource();
					this.processDispatchOutputEventsTask = new Task(
						this.ProcessDispatchOutputEventsTaskCore,
						this.processDispatchOutputEventsTokenSource.Token,
						TaskCreationOptions.LongRunning);
				}

				this.process.Exited += Process_OnExited;

				this.process.Start();

				if(this.redirectStreams)
				{
					this.processReadStandardOutputTask.Start();
					this.processReadStandardErrorTask.Start();
					this.processDispatchOutputEventsTask.Start();	
				}
			}
			catch(Exception)
			{
				this.Release();
				throw;
			}
		}

		private void ProcessDispatchOutputEventsTaskCore()
		{
			var token = this.processDispatchOutputEventsTokenSource.Token;

			while(!token.IsCancellationRequested)
			{
				var e = null as ProcessRunnerOutputEventArgs;

				if (this.processOutputQueue.TryDequeue(out e))
					this.NotifyProcessOutputReceived(e);

				if(this.exitCode != null && this.processOutputQueue.Count == 0)
					break;
			}
		}

		private void ProcessReadStandardErrorTaskCore()
		{
			this.ProcessReadStreamTaskCore(
				this.process.StandardOutput, 
				this.processReadStandardOutputTokenSource.Token, 
				false);
		}

		private void ProcessReadStandardOutputTaskCore()
		{
			this.ProcessReadStreamTaskCore(
				this.process.StandardError, 
				this.processReadStandardErrorTokenSource.Token,
				true);
		}

		private void ProcessReadStreamTaskCore(
			StreamReader stream, 
			CancellationToken token, 
			bool isErrorStream)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");

			while(!token.IsCancellationRequested)
			{
				if(stream.Peek() == -1)
					break;
				
				var data = stream.ReadLine();
				this.processOutputQueue.Enqueue(new ProcessRunnerOutputEventArgs(data, isErrorStream));
			}
		}

		private void Release()
		{
			if (this.processReadStandardOutputTokenSource != null)
			{
				this.processReadStandardOutputTokenSource.Cancel();
				this.processReadStandardOutputTokenSource.Dispose();
				this.processReadStandardOutputTokenSource = null;
			}

			if (this.processReadStandardOutputTask != null)
			{
				this.processReadStandardOutputTask.Dispose();
				this.processReadStandardOutputTask = null;
			}

			if(this.processReadStandardErrorTokenSource != null)
			{
				this.processReadStandardErrorTokenSource.Cancel();
				this.processReadStandardErrorTokenSource.Dispose();
				this.processReadStandardErrorTokenSource = null;
			}

			if(this.processReadStandardErrorTask != null)
			{
				this.processReadStandardErrorTask.Dispose();
				this.processReadStandardErrorTask = null;
			}

			if(this.processDispatchOutputEventsTokenSource != null)
			{
				this.processDispatchOutputEventsTokenSource.Cancel();
				this.processDispatchOutputEventsTokenSource.Dispose();
				this.processDispatchOutputEventsTokenSource = null;
			}

			if(this.processDispatchOutputEventsTask != null)
			{
				this.processDispatchOutputEventsTask.Dispose();
				this.processDispatchOutputEventsTask = null;
			}

			if(this.process != null)
			{
				this.process.Exited -= this.Process_OnExited;

				this.process.Close();
				this.process.Dispose();
				this.process = null;
			}
		}

		private void Process_OnExited(object sender, EventArgs e)
		{
			this.exitCode = this.process.ExitCode;
			this.exitTimestamp = this.process.ExitTime;
			this.NotifyProcessExited();
		}

		private void CheckIsDisposed()
		{
			if(this.isDisposed)
				throw new ObjectDisposedException(this.GetType().FullName);
		}

		public void Dispose()
		{
			this.DisposeCore(true);
		}

		private void DisposeCore(bool disposing)
		{
			if(this.isDisposed)
				return;

			if(!disposing)
				return;

			this.Release();

			this.OutputRecevied = null;
			this.Exited = null;

			this.isDisposed = true;
		}

		~ProcessRunner()
		{
			this.DisposeCore(false);
		}
	}
}
