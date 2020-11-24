// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

namespace CommandLineUtility.Tests
{
	class Settings_SettingsBase : SettingsBase<Settings_SettingsBase>
	{
		[Switch]
		public int IntValue { get; set; }
	}

	class Settings_SettingsBase_Constructor : SettingsBase<Settings_SettingsBase_Constructor>
	{
		public Settings_SettingsBase_Constructor()
		{
			CommandLineParser.GetSettings(this);
		}

		[Switch]
		public int IntValue { get; set; }
	}

	class Settings_SettingsBase_Constructor_SpecificArgs : SettingsBase<Settings_SettingsBase_Constructor>
	{
		public Settings_SettingsBase_Constructor_SpecificArgs()
		{
			CommandLineParser.GetSettings(this, "-IntValue", "2", "13", "5", "4");
		}

		[Switch]
		public int IntValue { get; set; }
	}
}
