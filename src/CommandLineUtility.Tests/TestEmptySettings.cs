// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommandLineUtility.Tests
{
	[TestClass]
	public class TestEmptySettings
	{
		[TestMethod]
		public void EmptyISettings()
		{
			CommandLineArgs.Set("arg1", "arg2", "arg3", "arg4");
			var settings = CommandLineParser.GetSettings<Settings_Empty>();

			//Not null
			Assert.AreNotEqual(null, settings.GlobalUnconsumedArguments);
			//Correct length
			Assert.AreEqual(4,       settings.GlobalUnconsumedArguments.Length);
			//Array elements are correct
			Assert.AreEqual("arg1",  settings.GlobalUnconsumedArguments[0]);
			Assert.AreEqual("arg2",  settings.GlobalUnconsumedArguments[1]);
			Assert.AreEqual("arg3",  settings.GlobalUnconsumedArguments[2]);
			Assert.AreEqual("arg4",  settings.GlobalUnconsumedArguments[3]);
		}
		[TestMethod]
		public void EmptySettingsBase()
		{
			CommandLineArgs.Set("arg1", "arg2", "arg3", "arg4");
			var settings = Settings_Empty_SettingsBase.FromCommandLine();

			//Not null
			Assert.AreNotEqual(null, settings.GlobalUnconsumedArguments);
			//Correct length
			Assert.AreEqual(4, settings.GlobalUnconsumedArguments.Length);
			//Array elements are correct
			Assert.AreEqual("arg1", settings.GlobalUnconsumedArguments[0]);
			Assert.AreEqual("arg2", settings.GlobalUnconsumedArguments[1]);
			Assert.AreEqual("arg3", settings.GlobalUnconsumedArguments[2]);
			Assert.AreEqual("arg4", settings.GlobalUnconsumedArguments[3]);
		}
	}
}
