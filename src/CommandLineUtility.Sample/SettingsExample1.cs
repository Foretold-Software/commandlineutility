// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommandLineUtility;

namespace CommandLineUtility.Sample
{
	internal class SettingsExample1 : ISettings
	{
		#region ISettings Members
		public string[] GlobalUnconsumedArguments { get; set; }
		#endregion

		#region Constructor
		public SettingsExample1()
		{
			this.Action = Action.Add;
		}
		#endregion

		#region Switches
		[Switch("action", MinArguments = 1)]
		[Switch("add",    Action.Add,    MaxArguments = 0, ExclusiveOf = new string[] { "action", "remove", "modify" })]
		[Switch("remove", Action.Remove, MaxArguments = 0, ExclusiveOf = new string[] { "action", "add", "modify" })]
		[Switch("modify", Action.Modify, MaxArguments = 0, ExclusiveOf = new string[] { "action", "add", "remove" })]
		public Action Action { get; set; }

		[Switch(MinArguments = 1)]
		public string Log { get; set; }

		[Switch("s")]
		[Switch("silent")]
		public bool Silent { get; set; }

		[Switch]
		public string[] Messages { get; set; }

		[Switch("writemode")]
		public List<WriteMode> Folder { get; set; }

		//Note: This enables commands like "command.exe ----mylist string1 string2"
		[Switch("--mylist", JoinArguments = true)]
		public ObservableCollection<string> MyList { get; set; }

		//[Switch("ngt5")]
		//public int[] NumbersGreaterThanFive { get; set; }
		//[Switch("nlt5")]
		//public int[] NumbersLessThanFive { get; set; }


		[Switch("truefalse", MinArguments = 1)]
		public bool TrueFalseValue { get; set; }
		#endregion

		#region Global Arguments
		[Switch("input")]
		[Argument(0, false)]
		public string InputFile { get; set; }

		[Argument(1, false)]
		public string OutputFile { get; set; }

		[Argument(2, false)]
		public Action ActionGIArg { get; set; }
		#endregion

		#region Validation Methods

		[ValidateArgument("truefalse")]
		public bool ValidateTrueFalseValue(string str)
		{
			//This validation method is actually redundant, because the parser would only
			// accept an argument that could be converted to the correct type anyway.
			bool b;
			return bool.TryParse(str, out b);
		}

		/// <summary>
		/// This method validates all global unconsumed arguments as an array of strings values.
		/// </summary>
		/// <param name="args">An array of all global unconsumed arguments.</param>
		/// <returns>Boolean value indicating whether the arguments are acceptable.</returns>
		[ValidateArgument]
		public bool ValidateGlobalUnconsumedArguments(string[] args)
		{
			//Return success.
			return true;
		}

		//	Actually, maybe it can be allowed, just not for methods with a collection parameter type.
		[ValidateArgument(0)]
		[ValidateArgument(1)]
		public bool ValidateFilePath(/*int index, */string filepath)
		{
			//If this global indexed argument does not pass this validation test,
			//then parsing fails, rather than being tested for the next global indexed
			//argument or going to "AllUnconsumedArguments".

			//if (index == 0)
			//	return File.Exists(filepath);
			//else
			//{
			return true;
			//}
		}

		[ValidateArgument(2)]
		public bool ValidateActionGIArg(Action action)
		{
			return true;
		}

		/// <summary>
		/// This method validates the arguments that were passed in after the "Report" switch and are therefore eligible for consumption by the "v" switch.
		/// MaxArguments was already used to trim the string array to the correct length.
		/// </summary>
		/// <param name="input">The arguments passed in on the command line immediately following the "v" switch.</param>
		/// <returns>The number of arguments that will ACTUALLY be consumed by the "v" switch.</returns>
		//[Switch("v")]
		//[ValidateArgument("writemode")]
		public int ValidateWriteModeOld(/*string switchName, */object[] input)
		{
			//Validate that each string in "inputVariablesKVPs" implements the syntax "[a-zA-Z0-9]+=.+" or something similar.

			//Let's assume that four strings were passed in, but the first two strings passed this validation and the third string did not.
			//Then we return 2, even if the fourth string would have passed this validation, it does not even get tested.

			return input.Length;
		}


		[ValidateArgument("writemode")]
		public int ValidateWriteMode(List<WriteMode> modes) //Note: "IEnumerable<WriteMode> modes" works too.
		{
			return modes.Count;
		}
		#endregion
	}

	enum Action
	{
		Add, Remove, Modify
	}

	[Flags]
	enum WriteMode
	{
		None = 0,
		Append = 1,
		Overwrite = 2,
		Replace = 4,
		Reverse = 8,
		Repeat = 16,
		Truncate = 32
	}
}
