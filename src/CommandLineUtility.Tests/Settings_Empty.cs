// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

namespace CommandLineUtility.Tests
{
	class Settings_Empty : ISettings
	{
		public string[] GlobalUnconsumedArguments { get; set; }
	}

	class Settings_Empty_SettingsBase : SettingsBase<Settings_Empty_SettingsBase> { }
}
