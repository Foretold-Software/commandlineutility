// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLineUtility
{
	internal class _string
	{
		public static bool IsNullOrWhiteSpace(string value)
		{
#if NET40 || NET45
			return string.IsNullOrWhiteSpace(value);
#elif NET35
			return value == null || value.Trim().Length == 0;
#endif
		}

		public static string Join(string separator, IEnumerable<string> values)
		{
#if NET40 || NET45
			return string.Join(separator, values);
#elif NET35
			return string.Join(separator, values.ToArray());
#endif
		}
		public static string Join<T>(string separator, IEnumerable<T> values) where T : class
		{
#if NET40 || NET45
			return string.Join(separator, values);
#elif NET35
			return string.Join(separator, values.Select(value => value.ToString()).ToArray());
#endif
		}
	}
}
