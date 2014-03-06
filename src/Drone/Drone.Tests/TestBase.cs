using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.IO;

namespace Drone.Tests
{
	[TestFixture]
	public class TestBase
	{
		private readonly IList<Action> fixtureCleanup;
		private readonly IList<Action> testCleanup;

		public TestBase()
		{
			this.fixtureCleanup = new List<Action>();
			this.testCleanup = new List<Action>();
		}

		public void FixtureCleanUpFile(string filename)
		{
			this.fixtureCleanup.Add(() => this.CleanUpFile(filename));
		}

		public void TestCleanUpFile(string filename)
		{
			this.testCleanup.Add(() => this.CleanUpFile(filename));
		}

		public void CleanUpFile(string filename)
		{
			if (File.Exists(filename))
				File.Delete(filename);
		}
			
		[TestFixtureSetUp]
		public virtual void FixtureSetUp()
		{

		}

		[TestFixtureTearDown]
		public virtual void FixtureTearDown()
		{
			foreach (var action in this.fixtureCleanup)
			{
				try
				{
					action();
				}
				catch (Exception)
				{
					
				}
			}
		}

		[SetUp]
		public virtual void SetUp()
		{
			
		}

		[TearDown]
		public virtual void TearDown()
		{
			foreach (var action in this.testCleanup)
			{
				try
				{
					action();
				}
				catch (Exception)
				{

				}
			}
		}
	}
}