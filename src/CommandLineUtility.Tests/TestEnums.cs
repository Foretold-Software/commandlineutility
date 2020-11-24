// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommandLineUtility.Tests
{
	[TestClass]
	public class TestEnums
	{
		[TestMethod]
		public void Enum()
		{
			CommandLineArgs.Set("-Enum", "Value1", "Value2", "Value5|Value3", "Value3");
			Settings_Enums settings = CommandLineParser.GetSettings<Settings_Enums>();

			// ----- Enum -----
			//Correct flags are set
			Assert.AreEqual(MyEnum.Value1, settings.Enum);

			// ----- GlobalUnconsumedArguments -----
			//Not null
			Assert.AreNotEqual(null,         settings.GlobalUnconsumedArguments);
			//Correct length
			Assert.AreEqual(3,               settings.GlobalUnconsumedArguments.Length);
			//Array elements are correct
			Assert.AreEqual("Value2",        settings.GlobalUnconsumedArguments[0]);
			Assert.AreEqual("Value5|Value3", settings.GlobalUnconsumedArguments[1]);
			Assert.AreEqual("Value3",        settings.GlobalUnconsumedArguments[2]);

			//Other properties should not be set.
			Assert.AreEqual(null,             settings.EnumArray);
			Assert.AreEqual(null,             settings.FlagsEnumArray);
			Assert.AreEqual(null,             settings.FlagsEnumCollection);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumVM1);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumVM2);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumJoined);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumJoinedWithSeparators);
		}

		[TestMethod]
		public void EnumArray()
		{
			CommandLineArgs.Set("-EnumArray", "Value1", "Value2", "Value5|Value3", "Value3");
			Settings_Enums settings = CommandLineParser.GetSettings<Settings_Enums>();

			// ----- EnumArray -----
			//Not null
			Assert.AreNotEqual(null,         settings.EnumArray);
			//Correct length
			Assert.AreEqual(2,               settings.EnumArray.Length);
			//Correct flags are set
			Assert.AreEqual(MyEnum.Value1,   settings.EnumArray[0]);
			Assert.AreEqual(MyEnum.Value2,   settings.EnumArray[1]);

			// ----- GlobalUnconsumedArguments -----
			//Not null
			Assert.AreNotEqual(null,         settings.GlobalUnconsumedArguments);
			//Correct length
			Assert.AreEqual(2,               settings.GlobalUnconsumedArguments.Length);
			//Array elements are correct
			Assert.AreEqual("Value5|Value3", settings.GlobalUnconsumedArguments[0]);
			Assert.AreEqual("Value3",        settings.GlobalUnconsumedArguments[1]);

			//Other properties should not be set.
			Assert.AreEqual(null,             settings.FlagsEnumArray);
			Assert.AreEqual(null,             settings.FlagsEnumCollection);
			Assert.AreEqual(MyEnum.None,      settings.Enum);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumVM1);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumVM2);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumJoined);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumJoinedWithSeparators);
		}

		[TestMethod]
		public void FlagsEnumVM1()
		{
			CommandLineArgs.Set("-FlagsEnumVM1", "Flag1", "Flag5|Flag3", "Flag3", "Flag4");
			Settings_Enums settings = CommandLineParser.GetSettings<Settings_Enums>();

			//Correct flags are set
			Assert.AreEqual(MyFlagsEnum.Flag1 | MyFlagsEnum.Flag3 | MyFlagsEnum.Flag4 | MyFlagsEnum.Flag5, settings.FlagsEnumVM1);

			//Other properties should not be set.
			Assert.AreEqual(null,             settings.GlobalUnconsumedArguments);
			Assert.AreEqual(null,             settings.EnumArray);
			Assert.AreEqual(null,             settings.FlagsEnumArray);
			Assert.AreEqual(null,             settings.FlagsEnumCollection);
			Assert.AreEqual(MyEnum.None,      settings.Enum);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumVM2);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumJoined);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumJoinedWithSeparators);
		}

		[TestMethod]
		public void FlagsEnumVM2()
		{
			CommandLineArgs.Set("-FlagsEnumVM2", "Flag1", "Flag5|Flag3", "Flag3", "Flag4");
			Settings_Enums settings = CommandLineParser.GetSettings<Settings_Enums>();

			// ----- FlagsEnumVM2 -----
			//Correct flags are set
			Assert.AreEqual(MyFlagsEnum.Flag1 | MyFlagsEnum.Flag3 | MyFlagsEnum.Flag5, settings.FlagsEnumVM2);

			// ----- GlobalUnconsumedArguments -----
			//Not null
			Assert.AreNotEqual(null, settings.GlobalUnconsumedArguments);
			//Correct length
			Assert.AreEqual(2, settings.GlobalUnconsumedArguments.Length);
			//Array elements are correct
			Assert.AreEqual("Flag3", settings.GlobalUnconsumedArguments[0]);
			Assert.AreEqual("Flag4", settings.GlobalUnconsumedArguments[1]);

			//Other properties should not be set.
			Assert.AreEqual(null,             settings.EnumArray);
			Assert.AreEqual(null,             settings.FlagsEnumArray);
			Assert.AreEqual(null,             settings.FlagsEnumCollection);
			Assert.AreEqual(MyEnum.None,      settings.Enum);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumVM1);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumJoined);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumJoinedWithSeparators);
		}

		[TestMethod]
		public void FlagsEnumArray()
		{
			CommandLineArgs.Set("-FlagsEnumArray", "Flag1", "Flag5|Flag3", "Flag3", "Flag4");
			Settings_Enums settings = CommandLineParser.GetSettings<Settings_Enums>();

			//Not null
			Assert.AreNotEqual(null, settings.FlagsEnumArray);
			//Correct length
			Assert.AreEqual(4, settings.FlagsEnumArray.Length);
			//Correct flags are set
			Assert.AreEqual(MyFlagsEnum.Flag1,                     settings.FlagsEnumArray[0]);
			Assert.AreEqual(MyFlagsEnum.Flag3 | MyFlagsEnum.Flag5, settings.FlagsEnumArray[1]);
			Assert.AreEqual(MyFlagsEnum.Flag3,                     settings.FlagsEnumArray[2]);
			Assert.AreEqual(MyFlagsEnum.Flag4,                     settings.FlagsEnumArray[3]);


			//Other properties should not be set.
			Assert.AreEqual(null,             settings.GlobalUnconsumedArguments);
			Assert.AreEqual(null,             settings.EnumArray);
			Assert.AreEqual(null,             settings.FlagsEnumCollection);
			Assert.AreEqual(MyEnum.None,      settings.Enum);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumVM1);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumVM2);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumJoined);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumJoinedWithSeparators);
		}

		[TestMethod]
		public void FlagsEnumCollection()
		{
			CommandLineArgs.Set("-FlagsEnumCollection", "Flag1", "Flag5|Flag3", "Flag3", "Flag4");
			Settings_Enums settings = CommandLineParser.GetSettings<Settings_Enums>();

			//Not null
			Assert.AreNotEqual(null,                               settings.FlagsEnumCollection);
			//Correct length
			Assert.AreEqual(4,                                     settings.FlagsEnumCollection.Count);
			//Correct flags are set
			Assert.AreEqual(MyFlagsEnum.Flag1,                     settings.FlagsEnumCollection[0]);
			Assert.AreEqual(MyFlagsEnum.Flag3 | MyFlagsEnum.Flag5, settings.FlagsEnumCollection[1]);
			Assert.AreEqual(MyFlagsEnum.Flag3,                     settings.FlagsEnumCollection[2]);
			Assert.AreEqual(MyFlagsEnum.Flag4,                     settings.FlagsEnumCollection[3]);


			//Other properties should not be set.
			Assert.AreEqual(null,             settings.GlobalUnconsumedArguments);
			Assert.AreEqual(null,             settings.EnumArray);
			Assert.AreEqual(null,             settings.FlagsEnumArray);
			Assert.AreEqual(MyEnum.None,      settings.Enum);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumVM1);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumVM2);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumJoined);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumJoinedWithSeparators);
		}

		[TestMethod]
		public void FlagsEnumJoined()
		{
			CommandLineArgs.Set("-FlagsEnumJoinedFlag1|Flag5|Flag3|Flag3");
			Settings_Enums settings = CommandLineParser.GetSettings<Settings_Enums>();

			//Correct flags are set
			Assert.AreEqual(MyFlagsEnum.Flag1 | MyFlagsEnum.Flag3 | MyFlagsEnum.Flag5, settings.FlagsEnumJoined);

			//Other properties should not be set.
			Assert.AreEqual(null,             settings.GlobalUnconsumedArguments);
			Assert.AreEqual(null,             settings.EnumArray);
			Assert.AreEqual(null,             settings.FlagsEnumArray);
			Assert.AreEqual(null,             settings.FlagsEnumCollection);
			Assert.AreEqual(MyEnum.None,      settings.Enum);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumVM1);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumVM2);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumJoinedWithSeparators);
		}

		[TestMethod]
		public void FlagsEnumJoinedWithSeparators()
		{
			//Apparently, the Enum.Parse method doesn't care about white space :)
			CommandLineArgs.Set("-FlagsEnumJoinedWithSeparatorsFlag1|Flag5,Flag5; Flag5  |Flag3 ; Flag3");
			Settings_Enums settings = CommandLineParser.GetSettings<Settings_Enums>();

			//Correct flags are set
			Assert.AreEqual(MyFlagsEnum.Flag1 | MyFlagsEnum.Flag3 | MyFlagsEnum.Flag5, settings.FlagsEnumJoinedWithSeparators);

			//Other properties should not be set.
			Assert.AreEqual(null,             settings.GlobalUnconsumedArguments);
			Assert.AreEqual(null,             settings.EnumArray);
			Assert.AreEqual(null,             settings.FlagsEnumArray);
			Assert.AreEqual(null,             settings.FlagsEnumCollection);
			Assert.AreEqual(MyEnum.None,      settings.Enum);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumVM1);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumVM2);
			Assert.AreEqual(MyFlagsEnum.None, settings.FlagsEnumJoined);
		}

	}
}
