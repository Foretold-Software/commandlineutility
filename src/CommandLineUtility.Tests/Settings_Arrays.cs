// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

namespace CommandLineUtility.Tests
{
	public class Settings_Arrays : ISettings
	{
		#region ISettings Members
		public string[] GlobalUnconsumedArguments { get; set; }
		#endregion


		[Switch]
		public int[] IntArray { get; set; }

		[Switch]
		public string[] StringArray { get; set; }

		[Switch]
		public int[] IntArrayWithArgumentValidation { get; set; }

		[Switch("MyJoinedString=", JoinArguments = true, ArgumentSeparator = ";| ")]
		public string[] JoinedStringsWithSeparatorsAndEqual { get; set; }

		[Switch(JoinArguments = true, ArgumentSeparator = "")]
		public int[] JoinedIntArrayAlways1Element { get; set; }


		[ValidateArgument("StringArray")]
		public int ValidationMethod1(string[] args)
		{
			return args.Length;
		}

		[ValidateArgument("IntArrayWithArgumentValidation")]
		public int ValidationMethod2(int[] args)
		{
			return args.Length / 2;
		}
	}
}
