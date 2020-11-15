// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

namespace CommandLineUtility
{
	/// <summary>
	/// Provides a standard base class for implementations of
	/// the <see cref="ISettings"/> interface.
	/// </summary>
	public class SettingsBase<T> : ISettings
		where T : SettingsBase<T>
	{
		#region ISettings Members
		public string[] GlobalUnconsumedArguments { get; set; }
		#endregion

		public static T FromCommandLine()
		{
			CommandLineParser p = new CommandLineParser(typeof(T));
			return p.ParseSettings() as T;
		}
		public static T FromCommandLine(ParserInfo parserInfo)
		{
			CommandLineParser p = new CommandLineParser(typeof(T), parserInfo);
			return p.ParseSettings() as T;
		}
		public static T FromCommandLine(string[] commandLineArguments)
		{
			CommandLineArgs.Set(commandLineArguments);
			CommandLineParser p = new CommandLineParser(typeof(T));
			return p.ParseSettings() as T;
		}
		public static T FromCommandLine(ParserInfo parserInfo, string[] commandLineArguments)
		{
			CommandLineArgs.Set(commandLineArguments);
			CommandLineParser p = new CommandLineParser(typeof(T), parserInfo);
			return p.ParseSettings() as T;
		}
	}
}
