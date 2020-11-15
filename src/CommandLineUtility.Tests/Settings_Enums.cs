// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using System;
using System.Collections.Generic;

namespace CommandLineUtility.Tests
{
	public class Settings_Enums : ISettings
	{
		#region ISettings Members
		public string[] GlobalUnconsumedArguments { get; set; }
		#endregion

		[Switch]
		public MyEnum Enum { get; set; }

		[Switch]
		public MyEnum[] EnumArray { get; set; }

		[Switch]
		public MyFlagsEnum FlagsEnumVM1 { get; set; }
		[Switch]
		public MyFlagsEnum FlagsEnumVM2 { get; set; }

		[Switch]
		public MyFlagsEnum[] FlagsEnumArray { get; set; }

		[Switch]
		public List<MyFlagsEnum> FlagsEnumCollection { get; set; }


		[Switch(JoinArguments = true)]
		public MyFlagsEnum FlagsEnumJoined { get; set; }

		[Switch(JoinArguments = true, ArgumentSeparator = ";,")]
		public MyFlagsEnum FlagsEnumJoinedWithSeparators { get; set; }



		[ValidateArgument("FlagsEnumVM1")]
		public bool ValidationMethod1(MyFlagsEnum values)
		{
			return true;
		}

		[ValidateArgument("FlagsEnumVM2")]
		public int ValidationMethod2(MyFlagsEnum[] values)
		{
			return values.Length / 2;
		}
	}

	public enum MyEnum
	{
		None = 0,
		Value1,
		Value2,
		Value3,
		Value4,
		Value5,
		Value6,
	}
	[Flags]
	public enum MyFlagsEnum
	{
		None = 0,   //0x00000000
		Flag1 = 1,  //0x00000001
		Flag2 = 2,  //0x00000010
		Flag3 = 4,  //0x00000100
		Flag4 = 8,  //0x00001000
		Flag5 = 16, //0x00010000
		Flag6 = 32, //0x00100000
	}
}
