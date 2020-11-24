// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CommandLineUtility.Tests
{
	public class Settings_Collections : ISettings
	{
		#region ISettings Members
		public string[] GlobalUnconsumedArguments { get; set; }
		#endregion


		[Switch]
		public List<int> IntList { get; set; }

		[Switch]
		public List<string> StringList { get; set; }

		[Switch]
		public ObservableCollection<int> IntObservableCollection { get; set; }

		[Switch]
		public List<int> IntListWithArgumentValidation { get; set; }

		[Switch("MyJoinedString=", JoinArguments = true, ArgumentSeparator = ";| ")]
		public List<string> JoinedStringListWithSeparatorsAndEqual { get; set; }

		[Switch(JoinArguments = true, ArgumentSeparator = "")]
		public List<int> JoinedIntListAlways1Element { get; set; }


		[ValidateArgument("StringList")]
		public int ValidationMethod1(string[] args)
		{
			return args.Length;
		}

		[ValidateArgument("IntListWithArgumentValidation")]
		public int ValidationMethod2(int[] args)
		{
			return args.Length / 2;
		}
	}
}
