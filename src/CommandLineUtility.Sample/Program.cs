// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using CommandLineUtility;

namespace CommandLineUtility.Sample
{
	class Program
	{
		static void Main(string[] args)
		{
			OutputAllArgs(args);

			Scenario5();
		}

		//All of the below scenarios are good examples of ways to get your settings from the command line.
		//Some of them are as little as one line of code.
		static void Scenario1()
		{
			SettingsExample3 settings = new SettingsExample3();
			//The 'settings' object is now populated.
		}
		static void Scenario2()
		{
			ISettings settings = CommandLineParser.GetSettings();
			//The 'settings' object is now populated.
		}
		static void Scenario3()
		{
			ParserInfo parserInfo = GetExample2ParserInfo();
			SettingsExample2 settings = CommandLineParser.GetSettings<SettingsExample2>(parserInfo);
			//The 'settings' object is now populated.
		}
		static void Scenario4()
		{
			CommandLineParser parser = new CommandLineParser();
			ISettings settings = parser.ParseSettings();
			//The 'settings' object is now populated.
		}
		static void Scenario5()
		{
			ISettings settings = new SettingsExample1();
			CommandLineParser parser = new CommandLineParser(settings);
			parser.ParseSettings();
			//The 'settings' object is now populated.
		}
		static void Scenario6()
		{
			CommandLineParser parser = new CommandLineParser(typeof(SettingsExample1));
			SettingsExample1 settings = parser.ParseSettings() as SettingsExample1;
			//The 'settings' object is now populated.
		}
		static void Scenario7()
		{
			ParserInfo parserInfo = GetExample2ParserInfo();
			CommandLineParser parser = new CommandLineParser(parserInfo);
			SettingsExample2 settings = parser.ParseSettings() as SettingsExample2;
			//The 'settings' object is now populated.
		}

		static ParserInfo GetExample1ParserInfo()
		{
			return new ParserInfo
			{
				SwitchIndicators				= new string[] { "--", "/" },
				AllowSwitchCharsInArguments		= false,
				PropertySwitchesAreExclusive	= true,
				ContinueOnFailedValidation		= false,
				SwitchesAreCaseSensitive		= false,
				UnconsumedArgumentMode			= UnconsumedArgumentMode.Allowed
			};
		}
		static ParserInfo GetExample2ParserInfo()
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

		static void OutputAllArgs(string[] args)
		{
			foreach (var item in args)
			{
				System.Console.WriteLine(string.Format("\"{0}\"", item));
			}
		}
	}
}
