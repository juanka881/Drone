using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Drone.Lib.Helpers
{
	public class ProcessRunner : IDisposable
	{
		private bool isDisposed;
		private Process process;
		private AutoResetEvent processEvent;
		private ConcurrentQueue<ProcessRunnerOutputReceivedEventArgs> processOutputReceivedQueue;
		private Task processMonitorTask;
		private CancellationTokenSource processCancellationTokenSource;
		private bool isRunning;
		private bool processOutputReadlineStarted;
		private bool processErrorReadlineStarted;
		private int exitCode;
		private DateTime exitTimestamp;

		private readonly object sync = new object();

		private readonly string filename;
		private readonly string commandLine;
		private readonly string workDir;

		public ProcessRunner(string filename, string commandLine, string workDir = null)
		{
			if(string.IsNullOrWhiteSpace(filename))
				throw new ArgumentException("filename is empty or null", "filename");

			this.filename = filename;
			this.commandLine = commandLine;
			this.workDir = workDir;
		}

		public event ProcessRunnerOutputReceivedEventHandler ProcessOutputRecevied;

		public event EventHandler ProcessExited;

		public void NotifyProcessOutputReceived(ProcessRunnerOutputReceivedEventArgs e)
		{
			var handler = this.ProcessOutputRecevied;

			if(handler == null)
				return;

			handler(this, e);
		}

		public void NotifyProcessExited()
		{
			var handler = this.ProcessExited;

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

			if(!this.process.WaitForExit((int) ts.TotalMilliseconds))
				return null;

			if(!this.processMonitorTask.IsCompleted)
			{
				if(!this.processMonitorTask.Wait((int) ts.TotalMilliseconds))
					return null;
			}

			return new ProcessRunnerResult(this.exitTimestamp, this.exitCode);
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
				psi.Arguments = this.commandLine;
				psi.CreateNoWindow = true;
				psi.RedirectStandardError = true;
				psi.RedirectStandardInput = true;
				psi.RedirectStandardOutput = true;
				psi.UseShellExecute = false;

				if(this.workDir != null)
					psi.WorkingDirectory = this.workDir;

				this.processEvent = new AutoResetEvent(false);
				this.processOutputReceivedQueue = new ConcurrentQueue<ProcessRunnerOutputReceivedEventArgs>();

				this.processCancellationTokenSource = new CancellationTokenSource();
				var processCancellationToken = this.processCancellationTokenSource.Token;

				this.processMonitorTask = new Task(this.ProcessMonitorTaskCore, processCancellationToken, TaskCreationOptions.LongRunning);
				this.processMonitorTask.Start();

				this.process.OutputDataReceived += Process_OnOutputDataReceived;
				this.process.ErrorDataReceived += Process_OnErrorDataReceived;
				this.process.Exited += Process_OnExited;

				this.process.Start();

				this.process.BeginOutputReadLine();
				this.processOutputReadlineStarted = true;

				this.process.BeginErrorReadLine();
				this.processErrorReadlineStarted = true;
			}
			catch(Exception)
			{
				this.ReleaseProcess();
				throw;
			}
		}

		private void ReleaseProcess()
		{
			if(this.process != null)
			{
				if(this.processOutputReadlineStarted)
				{
					this.processOutputReadlineStarted = false;
					this.process.CancelOutputRead();
				}

				if(this.processErrorReadlineStarted)
				{
					this.processErrorReadlineStarted = false;
					this.process.CancelErrorRead();
				}

				this.process.OutputDataReceived -= this.Process_OnOutputDataReceived;
				this.process.ErrorDataReceived -= this.Process_OnErrorDataReceived;
				this.process.Exited -= this.Process_OnExited;

				this.process.Close();
				this.process.Dispose();
				this.process = null;
			}

			if(this.processEvent != null)
			{
				this.processEvent.Dispose();
				this.processEvent = null;
			}

			if(this.processCancellationTokenSource != null)
			{
				this.processCancellationTokenSource.Dispose();
				this.processCancellationTokenSource = null;
			}

			this.processMonitorTask = null;
		}

		private void Process_OnExited(object sender, EventArgs e)
		{
			this.processCancellationTokenSource.Cancel();
			this.processEvent.Set();
		}

		private void Process_OnErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			this.processOutputReceivedQueue.Enqueue(new ProcessRunnerOutputReceivedEventArgs(e.Data, true));
			this.processEvent.Set();
		}

		private void Process_OnOutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			this.processOutputReceivedQueue.Enqueue(new ProcessRunnerOutputReceivedEventArgs(e.Data, false));
			this.processEvent.Set();
		}

		private void ProcessMonitorTaskCore(object state)
		{
			var cancellationToken = (CancellationToken) state;
			var events = new[] {this.processEvent, cancellationToken.WaitHandle};

			while(!cancellationToken.IsCancellationRequested)
			{
				WaitHandle.WaitAny(events);
				this.DispatchDataReceivedEvents();
			}

			this.exitCode = this.process.ExitCode;
			this.exitTimestamp = this.process.ExitTime;

			this.DispatchDataReceivedEvents();

			try
			{
				this.NotifyProcessExited();
			}
			catch(Exception)
			{
				
			}
		}

		private void DispatchDataReceivedEvents()
		{
			var e = null as ProcessRunnerOutputReceivedEventArgs;
			while(this.processOutputReceivedQueue.TryDequeue(out e))
			{
				try
				{
					this.NotifyProcessOutputReceived(e);
				}
				catch(Exception)
				{
					
				}
			}
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

			ReleaseProcess();

			this.ProcessOutputRecevied = null;
			this.ProcessExited = null;

			this.isDisposed = true;
		}

		~ProcessRunner()
		{
			this.DisposeCore(false);
		}
	}
}
