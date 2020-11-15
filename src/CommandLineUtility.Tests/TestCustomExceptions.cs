// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommandLineUtility.Tests
{
	[TestClass]
	public class TestCustomExceptions
	{
		[TestMethod]
		public void PropertySetMethodExceptionThrown()
		{
			bool thrown;
			var settings = new Settings_CustomExceptions();
			CommandLineArgs.Set("-MyString", "2", "13", "5", "4", "-MyString", "value1", "value2");

			try
			{
				CommandLineParser.GetSettings(settings);
				thrown = false;
			}
			catch (Exception exc)
			{
				thrown = true;

				if (exc.InnerException != null &&
					exc.InnerException is TargetInvocationException &&
					exc.InnerException.InnerException != null &&
					exc.InnerException.InnerException is ArgumentException)
				{
					var argExc = exc.InnerException.InnerException as ArgumentException;

					Assert.AreEqual(Settings_CustomExceptions.ExceptionMessage_PropertySetMethod, argExc.Message);
				}
				else
				{
					Assert.Fail("The exception caught was not expected.\nType: {0}\nMessage: {1}", exc.InnerException.GetType(), exc.InnerException.Message);
				}
			}

			if (!thrown)
				Assert.Fail("The expected exception was not thrown.");

			//Not null
			Assert.AreNotEqual(null, settings);

			//Correct value
			Assert.AreEqual("2",     settings.MyString);

			//Is null
			Assert.AreEqual(null,    settings.GlobalUnconsumedArguments);
		}

		[TestMethod]
		public void ValidationMethodExceptionThrown()
		{
			bool thrown;
			var settings = new Settings_CustomExceptions();
			CommandLineArgs.Set("-MyString", Settings_CustomExceptions.PleaseThrowAValidationMethodException);

			try
			{
				CommandLineParser.GetSettings(settings);
				thrown = false;
			}
			catch (Exception exc)
			{
				thrown = true;

				if (exc.InnerException != null &&
					exc.InnerException is TargetInvocationException &&
					exc.InnerException.InnerException != null &&
					exc.InnerException.InnerException is ArgumentException)
				{
					var argExc = exc.InnerException.InnerException as ArgumentException;

					Assert.AreEqual(Settings_CustomExceptions.ExceptionMessage_ValidationMethod, argExc.Message);
				}
				else
				{
					Assert.Fail("The exception caught was not expected.\nType: {0}\nMessage: {1}", exc.InnerException.GetType(), exc.InnerException.Message);
				}
			}

			if (!thrown)
				Assert.Fail("The expected exception was not thrown.");

			//Not null
			Assert.AreNotEqual(null,  settings);
			//Not set
			Assert.AreEqual(null,     settings.MyString);
			Assert.AreEqual(null,     settings.GlobalUnconsumedArguments);
		}
	}
}
