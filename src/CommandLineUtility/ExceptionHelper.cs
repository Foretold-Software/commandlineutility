// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using System;

namespace CommandLineUtility
{
	internal static class ExceptionHelper
	{
		internal static Exception Exception(Exception exc, string formatString, params object[] objects)
		{
			return new Exception(string.Format(formatString, objects), exc);
		}

		internal static Exception Exception(string formatString, params object[] objects)
		{
			return new Exception(string.Format(formatString, objects));
		}

		internal static ArgumentException ArgumentException(string formatString, params object[] objects)
		{
			return new ArgumentException(string.Format(formatString, objects));
		}
	}

	partial class CommandLineParser
	{
		private static Exception Exception(Exception exc, string formatString, params object[] objects)
		{
			return ExceptionHelper.Exception(exc, formatString, objects);
		}

		private static Exception Exception(string formatString, params object[] objects)
		{
			return ExceptionHelper.Exception(formatString, objects);
		}

		private static ArgumentException ArgumentException(string formatString, params object[] objects)
		{
			return ExceptionHelper.ArgumentException(formatString, objects);
		}
	}

	partial class SettingsClassInfo
	{
		private static Exception Exception(Exception exc, string formatString, params object[] objects)
		{
			return ExceptionHelper.Exception(exc, formatString, objects);
		}
	}

	partial class Extensions
	{
		private static Exception Exception(Exception exc, string formatString, params object[] objects)
		{
			return ExceptionHelper.Exception(exc, formatString, objects);
		}

		private static Exception Exception(string formatString, params object[] objects)
		{
			return ExceptionHelper.Exception(formatString, objects);
		}
	}
}
