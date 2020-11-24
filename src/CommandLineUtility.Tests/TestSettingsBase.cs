// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommandLineUtility.Tests
{
	[TestClass]
	public class TestSettingsBase
	{
		[TestMethod]
		public void SpecificArgs()
		{
			var settings = new Settings_SettingsBase_Constructor_SpecificArgs();

			//Correct value
			Assert.AreEqual(2, settings.IntValue);

			//Not null
			Assert.AreNotEqual(null, settings.GlobalUnconsumedArguments);
			//Correct length
			Assert.AreEqual(3,       settings.GlobalUnconsumedArguments.Length);
			//Array elements are correct
			Assert.AreEqual("13",    settings.GlobalUnconsumedArguments[0]);
			Assert.AreEqual("5",     settings.GlobalUnconsumedArguments[1]);
			Assert.AreEqual("4",     settings.GlobalUnconsumedArguments[2]);
		}

		[TestMethod]
		public void Constructor()
		{
			CommandLineArgs.Set("-IntValue", "2", "13", "5", "4");
			var settings = new Settings_SettingsBase_Constructor();

			//Correct value
			Assert.AreEqual(2, settings.IntValue);

			//Not null
			Assert.AreNotEqual(null, settings.GlobalUnconsumedArguments);
			//Correct length
			Assert.AreEqual(3,       settings.GlobalUnconsumedArguments.Length);
			//Array elements are correct
			Assert.AreEqual("13",    settings.GlobalUnconsumedArguments[0]);
			Assert.AreEqual("5",     settings.GlobalUnconsumedArguments[1]);
			Assert.AreEqual("4",     settings.GlobalUnconsumedArguments[2]);
		}

		[TestMethod]
		public void FromCommandLine()
		{
			CommandLineArgs.Set("-IntValue", "2", "13", "5", "4");
			var settings = Settings_SettingsBase.FromCommandLine();

			//Correct value
			Assert.AreEqual(2, settings.IntValue);

			//Not null
			Assert.AreNotEqual(null, settings.GlobalUnconsumedArguments);
			//Correct length
			Assert.AreEqual(3,       settings.GlobalUnconsumedArguments.Length);
			//Array elements are correct
			Assert.AreEqual("13",    settings.GlobalUnconsumedArguments[0]);
			Assert.AreEqual("5",     settings.GlobalUnconsumedArguments[1]);
			Assert.AreEqual("4",     settings.GlobalUnconsumedArguments[2]);
		}

		[TestMethod]
		public void FromCommandLineWithPassedArgs()
		{
			CommandLineArgs.Set();
			var settings = Settings_SettingsBase.FromCommandLine(new[] { "-IntValue", "2", "13", "5", "4" });

			//Correct value
			Assert.AreEqual(2, settings.IntValue);

			//Not null
			Assert.AreNotEqual(null, settings.GlobalUnconsumedArguments);
			//Correct length
			Assert.AreEqual(3,       settings.GlobalUnconsumedArguments.Length);
			//Array elements are correct
			Assert.AreEqual("13",    settings.GlobalUnconsumedArguments[0]);
			Assert.AreEqual("5",     settings.GlobalUnconsumedArguments[1]);
			Assert.AreEqual("4",     settings.GlobalUnconsumedArguments[2]);
		}
	}
}
