// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommandLineUtility.Tests
{
	[TestClass]
	public partial class TestCollections
	{
		[TestMethod]
		public void IntList()
		{
			CommandLineArgs.Set("-IntList", "2", "13", "5", "4");
			var settings = CommandLineParser.GetSettings<Settings_Collections>();

			//Not null
			Assert.AreNotEqual(null, settings.IntList);
			//Correct length
			Assert.AreEqual(4,       settings.IntList.Count);
			//Array elements are correct
			Assert.AreEqual(2,       settings.IntList[0]);
			Assert.AreEqual(13,      settings.IntList[1]);
			Assert.AreEqual(5,       settings.IntList[2]);
			Assert.AreEqual(4,       settings.IntList[3]);

			//Other properties should be null.
			Assert.AreEqual(null, settings.GlobalUnconsumedArguments);
			Assert.AreEqual(null, settings.StringList);
			Assert.AreEqual(null, settings.IntObservableCollection);
			Assert.AreEqual(null, settings.JoinedStringListWithSeparatorsAndEqual);
			Assert.AreEqual(null, settings.JoinedIntListAlways1Element);
		}

		[TestMethod]
		public void StringList()
		{
			CommandLineArgs.Set("-StringList", "string1", "string2", "string3");
			var settings = CommandLineParser.GetSettings<Settings_Collections>();

			//Not null
			Assert.AreNotEqual(null,   settings.StringList);
			//Correct length
			Assert.AreEqual(3,         settings.StringList.Count);
			//Array elements are correct
			Assert.AreEqual("string1", settings.StringList[0]);
			Assert.AreEqual("string2", settings.StringList[1]);
			Assert.AreEqual("string3", settings.StringList[2]);

			//Other properties should be null.
			Assert.AreEqual(null, settings.GlobalUnconsumedArguments);
			Assert.AreEqual(null, settings.IntList);
			Assert.AreEqual(null, settings.IntObservableCollection);
			Assert.AreEqual(null, settings.JoinedStringListWithSeparatorsAndEqual);
			Assert.AreEqual(null, settings.JoinedIntListAlways1Element);
		}

		[TestMethod]
		public void IntObservableCollection()
		{
			CommandLineArgs.Set("-IntObservableCollection", "2", "13", "5", "4");
			var settings = CommandLineParser.GetSettings<Settings_Collections>();

			//Not null
			Assert.AreNotEqual(null, settings.IntObservableCollection);
			//Correct length
			Assert.AreEqual(4,       settings.IntObservableCollection.Count);
			//Array elements are correct
			Assert.AreEqual(2,       settings.IntObservableCollection[0]);
			Assert.AreEqual(13,      settings.IntObservableCollection[1]);
			Assert.AreEqual(5,       settings.IntObservableCollection[2]);
			Assert.AreEqual(4,       settings.IntObservableCollection[3]);

			//Other properties should be null.
			Assert.AreEqual(null, settings.GlobalUnconsumedArguments);
			Assert.AreEqual(null, settings.IntList);
			Assert.AreEqual(null, settings.StringList);
			Assert.AreEqual(null, settings.JoinedStringListWithSeparatorsAndEqual);
			Assert.AreEqual(null, settings.JoinedIntListAlways1Element);
		}

		[TestMethod]
		public void IntListWithArgumentValidation()
		{
			CommandLineArgs.Set("-IntListWithArgumentValidation", "2", "13", "5", "4");
			var settings = CommandLineParser.GetSettings<Settings_Collections>();

			// ----- IntCollectionWithArgumentValidation -----
			//Not null
			Assert.AreNotEqual(null, settings.IntListWithArgumentValidation);
			//Correct length
			Assert.AreEqual(2,       settings.IntListWithArgumentValidation.Count);
			//Array elements are correct
			Assert.AreEqual(2,       settings.IntListWithArgumentValidation[0]);
			Assert.AreEqual(13,      settings.IntListWithArgumentValidation[1]);

			// ----- GlobalUnconsumedArguments -----
			//Not null
			Assert.AreNotEqual(null, settings.GlobalUnconsumedArguments);
			//Correct length
			Assert.AreEqual(2,       settings.GlobalUnconsumedArguments.Length);
			//Array elements are correct
			Assert.AreEqual("5",     settings.GlobalUnconsumedArguments[0]);
			Assert.AreEqual("4",     settings.GlobalUnconsumedArguments[1]);


			//Other properties should be null.
			Assert.AreEqual(null, settings.StringList);
			Assert.AreEqual(null, settings.IntObservableCollection);
			Assert.AreEqual(null, settings.JoinedStringListWithSeparatorsAndEqual);
			Assert.AreEqual(null, settings.JoinedIntListAlways1Element);
		}

		[TestMethod]
		public void JoinedStringsWithSeparatorsAndEqual()
		{
			CommandLineArgs.Set("-IntList", "2", "-MyJoinedString= string1;string2 string3|string4", "-StringList", "string1");
			var settings = CommandLineParser.GetSettings<Settings_Collections>();

			// ---------- JoinedIntCollectionAlways1Element ----------
			//Not null
			Assert.AreNotEqual(null,   settings.JoinedStringListWithSeparatorsAndEqual);
			//Correct length
			Assert.AreEqual(5,         settings.JoinedStringListWithSeparatorsAndEqual.Count);
			//Array elements are correct
			Assert.AreEqual("",        settings.JoinedStringListWithSeparatorsAndEqual[0]);
			Assert.AreEqual("string1", settings.JoinedStringListWithSeparatorsAndEqual[1]);
			Assert.AreEqual("string2", settings.JoinedStringListWithSeparatorsAndEqual[2]);
			Assert.AreEqual("string3", settings.JoinedStringListWithSeparatorsAndEqual[3]);
			Assert.AreEqual("string4", settings.JoinedStringListWithSeparatorsAndEqual[4]);

			//MyInts
			Assert.AreNotEqual(null, settings.IntList);
			Assert.AreEqual(1,       settings.IntList.Count);
			Assert.AreEqual(2,       settings.IntList[0]);

			//MyStrings
			Assert.AreNotEqual(null,   settings.StringList);
			Assert.AreEqual(1,         settings.StringList.Count);
			Assert.AreEqual("string1", settings.StringList[0]);

			//Other properties should be null.
			Assert.AreEqual(null, settings.IntObservableCollection);
			Assert.AreEqual(null, settings.GlobalUnconsumedArguments);
			Assert.AreEqual(null, settings.IntListWithArgumentValidation);
			Assert.AreEqual(null, settings.JoinedIntListAlways1Element);
		}

		[TestMethod]
		public void JoinedIntListAlways1Element()
		{
			CommandLineArgs.Set("-IntList", "2", "-JoinedIntListAlways1Element5684153", "-StringList", "string1");
			var settings = CommandLineParser.GetSettings<Settings_Collections>();

			// ---------- JoinedIntCollectionAlways1Element ----------
			//Not null
			Assert.AreNotEqual(null, settings.JoinedIntListAlways1Element);
			//Correct length
			Assert.AreEqual(1, settings.JoinedIntListAlways1Element.Count);
			//Array element is correct
			Assert.AreEqual(5684153, settings.JoinedIntListAlways1Element[0]);

			//MyInts
			Assert.AreNotEqual(null, settings.IntList);
			Assert.AreEqual(1,       settings.IntList.Count);
			Assert.AreEqual(2,       settings.IntList[0]);

			//MyStrings
			Assert.AreNotEqual(null,   settings.StringList);
			Assert.AreEqual(1,         settings.StringList.Count);
			Assert.AreEqual("string1", settings.StringList[0]);

			//Other properties should be null.
			Assert.AreEqual(null, settings.IntObservableCollection);
			Assert.AreEqual(null, settings.GlobalUnconsumedArguments);
			Assert.AreEqual(null, settings.IntListWithArgumentValidation);
			Assert.AreEqual(null, settings.JoinedStringListWithSeparatorsAndEqual);
		}

		[TestMethod]
		public void JoinedIntListAlways1Element_ArgumentException()
		{
			CommandLineArgs.Set("-IntList", "2", "-JoinedIntListAlways1Element5684;153", "-StringList", "string1");

			try
			{
				CommandLineParser.GetSettings<Settings_Collections>();
				Assert.Fail("Joined-argument switch with empty string separator and more than one argument should have thrown an exception.");
			}
			catch { }
		}
	}
}
