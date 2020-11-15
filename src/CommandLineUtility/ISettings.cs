// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

namespace CommandLineUtility
{
	public interface ISettings
	{
		/// <summary>
		/// Gets or sets an array of all the global unconsumed arguments from the command line.
		/// </summary>
		string[] GlobalUnconsumedArguments { get; set; }
	}
}
