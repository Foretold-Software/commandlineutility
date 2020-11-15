// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineUtility.Tests
{
	public class Settings_CustomExceptions : ISettings
	{
		#region ISettings Members
		public string[] GlobalUnconsumedArguments { get; set; }
		#endregion

		internal const string ExceptionMessage_PropertySetMethod = "Switch 'MyString' cannot be specified more than once.";
		internal const string ExceptionMessage_ValidationMethod = "Validation method threw an exception.";
		internal const string PleaseThrowAValidationMethodException = "PleaseThrowAValidationMethodException";

		private string _MyString;
		[Switch]
		public string MyString
		{
			get { return _MyString; }
			set
			{
				if (_MyString != null)
					throw new ArgumentException(ExceptionMessage_PropertySetMethod);
				_MyString = value;
			}
		}


		[ValidateArgument("MyString")]
		public bool ValidationMethod1(string arg)
		{
			if (arg == PleaseThrowAValidationMethodException)
			{
				throw new ArgumentException(ExceptionMessage_ValidationMethod);
			}
			else return true;
		}
	}
}
