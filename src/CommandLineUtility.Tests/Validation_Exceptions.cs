// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineUtility.Tests
{
	class Validation_Exceptions : ISettings
	{
		#region ISettings Members
		public string[] GlobalUnconsumedArguments { get; set; }
		#endregion
	}
}
