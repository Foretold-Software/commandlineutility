// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

namespace CommandLineUtility
{
	public class ParserInfo
	{
		#region Constants / Fields
		public static readonly ParserInfo Default = GetDefaultParserInfo();
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets a value indicating whether the parser allows
		/// switch indicator characters at the beginning of arguments.
		/// </summary>
		public bool AllowSwitchCharsInArguments { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether the parser should continue
		/// or throw an exception when an argument fails validation.
		/// </summary>
		public bool ContinueOnFailedValidation { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether all the switches on
		/// a given property are automatically exclusive of each other.
		/// </summary>
		public bool PropertySwitchesAreExclusive { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether switch names are case sensitive.
		/// </summary>
		public bool SwitchesAreCaseSensitive { get; set; }
		/// <summary>
		/// Gets or sets an array of string values that will be used to indicate a switch.
		/// </summary>
		public string[] SwitchIndicators { get; set; }
		/// <summary>
		/// Gets a value indicating whether global unconsumed arguments are allowed in the command line.
		/// </summary>
		public UnconsumedArgumentMode UnconsumedArgumentMode { get; set; }
		#endregion

		#region Constructor / Methods
		public ParserInfo()
		{
			this.SwitchIndicators = new string[0];
		}

		private static ParserInfo GetDefaultParserInfo()
		{
			ParserInfo info = new ParserInfo();

			info.SwitchIndicators = new string[] { "-", "/" };
			info.AllowSwitchCharsInArguments = false;
			info.PropertySwitchesAreExclusive = true;
			info.ContinueOnFailedValidation = false;
			info.SwitchesAreCaseSensitive = false;
			info.UnconsumedArgumentMode = UnconsumedArgumentMode.Allowed;

			return info;
		}
		#endregion
	}
}
