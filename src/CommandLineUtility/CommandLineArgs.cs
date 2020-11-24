// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using System;
using System.Linq;

namespace CommandLineUtility
{
	/// <summary>
	/// A helper class to aid in testing.
	/// </summary>
	internal static class CommandLineArgs
	{
		private static string[] _Arguments;
		private static string[] Arguments
		{
			get
			{
				if (_Arguments == null)
				{
					_Arguments = Environment.GetCommandLineArgs().Skip(1).ToArray();
				}
				return _Arguments;
			}
			set { _Arguments = value; }
		}

		internal static string[] Get()
		{
			return Arguments;
		}

		internal static void Set(params string[] arguments)
		{
			Arguments = arguments;
		}
	}
}
