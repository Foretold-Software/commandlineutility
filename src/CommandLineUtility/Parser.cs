// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using System;
using System.Diagnostics;

namespace CommandLineUtility
{
	//TODO Maybe: Allow Dictionary properties.
	//TODO IMPORTANT: Can arrays be treated as ICollections? Answer: Kinda. Make sure that they're being treated correctly.
	//TODO: Go through all the error messages when completely finished to make sure they make sense.
	//TODO: Test Validation Methods with ancestor and descendent and string parameter data types.
	public partial class CommandLineParser
	{
		#region Properties
		private ParserInfo ParserInfo { get; set; }
		/// <summary>
		/// Contains the information about the ISettings-implementing
		/// class and its members that implement attributes from this library.
		/// </summary>
		private SettingsClassInfo SettingsInfo { get; set; }
		private StringComparison ComparisonRule
		{ get { return this.SettingsInfo.ComparisonRule; } }
		#endregion

		#region Constructors
		public CommandLineParser()
		{
			this.ParserInfo = ParserInfo.Default;
			this.SettingsInfo = new SettingsClassInfo(GetISettingsType(), this.ParserInfo);

			if (Debugger.IsAttached) ValidateSettingsClass();
		}
		public CommandLineParser(ISettings settingsObject)
		{
			this.ParserInfo = ParserInfo.Default;
			this.SettingsInfo = new SettingsClassInfo(settingsObject, this.ParserInfo);

			if (Debugger.IsAttached) ValidateSettingsClass();
		}
		public CommandLineParser(Type settingsClassType)
		{
			if (!settingsClassType.ImplementsISettings())
				throw Exception("The settings class type {0} does not implement the {1} interface.", settingsClassType, typeof(ISettings));

			this.ParserInfo = ParserInfo.Default;
			this.SettingsInfo = new SettingsClassInfo(settingsClassType, this.ParserInfo);

			if (Debugger.IsAttached) ValidateSettingsClass();
		}
		public CommandLineParser(ParserInfo parserInfo)
		{
			this.ParserInfo = parserInfo;
			this.SettingsInfo = new SettingsClassInfo(GetISettingsType(), this.ParserInfo);

			if (Debugger.IsAttached) ValidateSettingsClass();
		}
		public CommandLineParser(Type settingsClassType, ParserInfo parserInfo)
		{
			if (!settingsClassType.ImplementsISettings())
				throw Exception("The settings class type {0} does not implement the {1} interface.", settingsClassType, typeof(ISettings));

			this.ParserInfo = parserInfo;
			this.SettingsInfo = new SettingsClassInfo(settingsClassType, this.ParserInfo);

			if (Debugger.IsAttached) ValidateSettingsClass();
		}
		public CommandLineParser(ISettings settingsObject, ParserInfo parserInfo)
		{
			this.ParserInfo = parserInfo;
			this.SettingsInfo = new SettingsClassInfo(settingsObject, this.ParserInfo);

			if (Debugger.IsAttached) ValidateSettingsClass();
		}
		#endregion
	}
}
