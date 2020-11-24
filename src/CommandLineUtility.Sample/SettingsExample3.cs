// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

namespace CommandLineUtility.Sample
{
	internal class SettingsExample3 : ISettings
	{
		#region ISettings Members
		public string[] GlobalUnconsumedArguments { get; set; }
		#endregion

		[Switch]
		public int Count { get; set; }

		public SettingsExample3()
		{
			Scenario1();
		}

		//Both of the below scenarios are examples of ways to populate this object with command line settings, right from this class's constructor.
		void Scenario1()
		{
			CommandLineParser.GetSettings(this);
		}
		void Scenario2()
		{
			ParserInfo parserInfo = GetExample3ParserInfo();
			CommandLineParser.GetSettings(this, parserInfo);
		}

		ParserInfo GetExample3ParserInfo()
		{
			return new ParserInfo
			{
				SwitchIndicators				= new string[] { "-", "/" },
				AllowSwitchCharsInArguments		= false,
				PropertySwitchesAreExclusive	= true,
				ContinueOnFailedValidation		= false,
				SwitchesAreCaseSensitive		= false,
				UnconsumedArgumentMode			= UnconsumedArgumentMode.Allowed
			};
		}
	}
}
