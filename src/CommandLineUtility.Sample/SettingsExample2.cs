// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using CommandLineUtility;

namespace CommandLineUtility.Sample
{
	internal class SettingsExample2 : SettingsBase<SettingsExample2>
	{
		[Switch("var")]
		public string[] VariablesVar { get; set; }

		[Switch("other", MaxArguments = 4, JoinArguments = true, ArgumentSeparator = "; ")]
		public string[] VariablesOther { get; set; }

		/// <summary>
		/// Validate the 'var' switch's arguments.
		/// </summary>
		[ValidateArgument("var")]
		public int ValidateVar(string[] args)
		{
			return args.Length;
		}

		/// <summary>
		/// Validate global unconsumed arguments.
		/// </summary>
		[ValidateArgument]
		public bool ValidateGUArgs(string[] args)
		{
			return true;
		}
	}
}
