// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommandLineUtility.Tests
{
	[TestClass]
	public partial class TestArrays
	{
		[TestMethod]
		public void IntArray()
		{
			CommandLineArgs.Set("-IntArray", "2", "13", "5", "4");
			var settings = CommandLineParser.GetSettings<Settings_Arrays>();

			//Not null
			Assert.AreNotEqual(null, settings.IntArray);
			//Correct length
			Assert.AreEqual(4,       settings.IntArray.Length);
			//Array elements are correct
			Assert.AreEqual(2,       settings.IntArray[0]);
			Assert.AreEqual(13,      settings.IntArray[1]);
			Assert.AreEqual(5,       settings.IntArray[2]);
			Assert.AreEqual(4,       settings.IntArray[3]);

			//Other properties should be null.
			Assert.AreEqual(null, settings.GlobalUnconsumedArguments);
			Assert.AreEqual(null, settings.StringArray);
			Assert.AreEqual(null, settings.JoinedStringsWithSeparatorsAndEqual);
			Assert.AreEqual(null, settings.JoinedIntArrayAlways1Element);
		}

		[TestMethod]
		public void StringArray()
		{
			CommandLineArgs.Set("-StringArray", "string1", "string2", "string3");
			var settings = CommandLineParser.GetSettings<Settings_Arrays>();

			//Not null
			Assert.AreNotEqual(null,   settings.StringArray);
			//Correct length
			Assert.AreEqual(3,         settings.StringArray.Length);
			//Array elements are correct
			Assert.AreEqual("string1", settings.StringArray[0]);
			Assert.AreEqual("string2", settings.StringArray[1]);
			Assert.AreEqual("string3", settings.StringArray[2]);

			//Other properties should be null.
			Assert.AreEqual(null, settings.GlobalUnconsumedArguments);
			Assert.AreEqual(null, settings.IntArray);
			Assert.AreEqual(null, settings.JoinedStringsWithSeparatorsAndEqual);
			Assert.AreEqual(null, settings.JoinedIntArrayAlways1Element);
		}

		[TestMethod]
		public void IntArrayWithArgumentValidation()
		{
			CommandLineArgs.Set("-IntArrayWithArgumentValidation", "2", "13", "5", "4");
			var settings = CommandLineParser.GetSettings<Settings_Arrays>();

			// ----- IntArrayWithArgumentValidation -----
			//Not null
			Assert.AreNotEqual(null, settings.IntArrayWithArgumentValidation);
			//Correct length
			Assert.AreEqual(2,       settings.IntArrayWithArgumentValidation.Length);
			//Array elements are correct
			Assert.AreEqual(2,       settings.IntArrayWithArgumentValidation[0]);
			Assert.AreEqual(13,      settings.IntArrayWithArgumentValidation[1]);

			// ----- GlobalUnconsumedArguments -----
			//Not null
			Assert.AreNotEqual(null, settings.GlobalUnconsumedArguments);
			//Correct length
			Assert.AreEqual(2,       settings.GlobalUnconsumedArguments.Length);
			//Array elements are correct
			Assert.AreEqual("5",     settings.GlobalUnconsumedArguments[0]);
			Assert.AreEqual("4",     settings.GlobalUnconsumedArguments[1]);


			//Other properties should be null.
			Assert.AreEqual(null, settings.StringArray);
			Assert.AreEqual(null, settings.JoinedStringsWithSeparatorsAndEqual);
			Assert.AreEqual(null, settings.JoinedIntArrayAlways1Element);
		}

		[TestMethod]
		public void JoinedStringsWithSeparatorsAndEqual()
		{
			CommandLineArgs.Set("-IntArray", "2", "-MyJoinedString= string1;string2 string3|string4", "-StringArray", "string1");
			var settings = CommandLineParser.GetSettings<Settings_Arrays>();

			// ---------- JoinedIntArrayAlways1Element ----------
			//Not null
			Assert.AreNotEqual(null,   settings.JoinedStringsWithSeparatorsAndEqual);
			//Correct length
			Assert.AreEqual(5,         settings.JoinedStringsWithSeparatorsAndEqual.Length);
			//Array elements are correct
			Assert.AreEqual("",        settings.JoinedStringsWithSeparatorsAndEqual[0]);
			Assert.AreEqual("string1", settings.JoinedStringsWithSeparatorsAndEqual[1]);
			Assert.AreEqual("string2", settings.JoinedStringsWithSeparatorsAndEqual[2]);
			Assert.AreEqual("string3", settings.JoinedStringsWithSeparatorsAndEqual[3]);
			Assert.AreEqual("string4", settings.JoinedStringsWithSeparatorsAndEqual[4]);

			//MyInts
			Assert.AreNotEqual(null, settings.IntArray);
			Assert.AreEqual(1,       settings.IntArray.Length);
			Assert.AreEqual(2,       settings.IntArray[0]);

			//MyStrings
			Assert.AreNotEqual(null,   settings.StringArray);
			Assert.AreEqual(1,         settings.StringArray.Length);
			Assert.AreEqual("string1", settings.StringArray[0]);

			//Other properties should be null.
			Assert.AreEqual(null, settings.GlobalUnconsumedArguments);
			Assert.AreEqual(null, settings.IntArrayWithArgumentValidation);
			Assert.AreEqual(null, settings.JoinedIntArrayAlways1Element);
		}

		[TestMethod]
		public void JoinedIntArrayAlways1Element()
		{
			CommandLineArgs.Set("-IntArray", "2", "-JoinedIntArrayAlways1Element5684153", "-StringArray", "string1");
			var settings = CommandLineParser.GetSettings<Settings_Arrays>();

			// ---------- JoinedIntArrayAlways1Element ----------
			//Not null
			Assert.AreNotEqual(null, settings.JoinedIntArrayAlways1Element);
			//Correct length
			Assert.AreEqual(1, settings.JoinedIntArrayAlways1Element.Length);
			//Array element is correct
			Assert.AreEqual(5684153, settings.JoinedIntArrayAlways1Element[0]);

			//MyInts
			Assert.AreNotEqual(null, settings.IntArray);
			Assert.AreEqual(1,       settings.IntArray.Length);
			Assert.AreEqual(2,       settings.IntArray[0]);

			//MyStrings
			Assert.AreNotEqual(null,   settings.StringArray);
			Assert.AreEqual(1,         settings.StringArray.Length);
			Assert.AreEqual("string1", settings.StringArray[0]);

			//Other properties should be null.
			Assert.AreEqual(null, settings.GlobalUnconsumedArguments);
			Assert.AreEqual(null, settings.IntArrayWithArgumentValidation);
			Assert.AreEqual(null, settings.JoinedStringsWithSeparatorsAndEqual);
		}

		[TestMethod]
		public void JoinedIntArrayAlways1Element_ArgumentException()
		{
			CommandLineArgs.Set("-IntArray", "2", "-JoinedIntArrayAlways1Element5684;153", "-StringArray", "string1");

			try
			{
				CommandLineParser.GetSettings<Settings_Arrays>();
				Assert.Fail("Joined-argument switch with empty string separator and more than one argument should have thrown an exception.");
			}
			catch { }
		}
	}
}
