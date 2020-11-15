// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

namespace CommandLineUtility
{
	partial class CommandLineParser
	{
		public static ISettings GetSettings()
		{
			var parser = new CommandLineParser();
			return parser.ParseSettings();
		}
		public static T GetSettings<T>() where T : class, ISettings
		{
			var parser = new CommandLineParser(typeof(T));
			return parser.ParseSettings() as T;
		}

		public static T GetSettings<T>(params string[] commandLineArguments) where T : class, ISettings
		{
			CommandLineArgs.Set(commandLineArguments);
			var parser = new CommandLineParser(typeof(T));
			return parser.ParseSettings() as T;
		}

		public static T GetSettings<T>(T settingsObject) where T : class, ISettings
		{
			var parser = new CommandLineParser(settingsObject);
			return parser.ParseSettings() as T;
		}

		public static T GetSettings<T>(T settingsObject, params string[] commandLineArguments) where T : class, ISettings
		{
			CommandLineArgs.Set(commandLineArguments);
			var parser = new CommandLineParser(settingsObject);
			return parser.ParseSettings() as T;
		}

		public static T GetSettings<T>(ParserInfo parserInfo) where T : class, ISettings
		{
			var parser = new CommandLineParser(typeof(T), parserInfo);
			return parser.ParseSettings() as T;
		}

		public static T GetSettings<T>(ParserInfo parserInfo, params string[] commandLineArguments) where T : class, ISettings
		{
			CommandLineArgs.Set(commandLineArguments);
			var parser = new CommandLineParser(typeof(T), parserInfo);
			return parser.ParseSettings() as T;
		}

		public static T GetSettings<T>(T settingsObject, ParserInfo parserInfo) where T : class, ISettings
		{
			var parser = new CommandLineParser(settingsObject, parserInfo);
			return parser.ParseSettings() as T;
		}

		public static T GetSettings<T>(T settingsObject, ParserInfo parserInfo, params string[] commandLineArguments) where T : class, ISettings
		{
			CommandLineArgs.Set(commandLineArguments);
			var parser = new CommandLineParser(settingsObject, parserInfo);
			return parser.ParseSettings() as T;
		}
	}
}
