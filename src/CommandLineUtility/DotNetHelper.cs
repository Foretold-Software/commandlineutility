// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using System.Collections.Generic;
using System.Linq;

namespace CommandLineUtility
{
	internal class _string
	{
		public static bool IsNullOrWhiteSpace(string value)
		{
#if NET35 || NET35_CLIENT
			return IsNullOrWhiteSpace_Net35(value);
#else
			return string.IsNullOrWhiteSpace(value);
#endif
		}

		internal static bool IsNullOrWhiteSpace_Net35(string value)
		{
			return value == null || value.Trim().Length == 0;
		}

		public static string Join(string separator, IEnumerable<string> values)
		{
#if NET35 || NET35_CLIENT
			return string.Join(separator, values.ToArray());
#else
			return string.Join(separator, values);
#endif
		}
		public static string Join<T>(string separator, IEnumerable<T> values) where T : class
		{
#if NET35 || NET35_CLIENT
			return string.Join(separator, values.Select(value => value?.ToString()).ToArray());
#else
			return string.Join(separator, values);
#endif
		}
	}
}
