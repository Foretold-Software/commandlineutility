// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using System;

namespace CommandLineUtility
{
	//NTS: There should not ever be any exclusivity with global arguments.
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class ArgumentAttribute : Attribute
	{
		public ArgumentAttribute(int index)
		{
			this._Index = index;
			this._Required = true;
		}
		public ArgumentAttribute(int index, bool required)
		{
			this._Index = index;
			this._Required = required;
		}

		readonly int _Index;
		readonly bool _Required;

		public int Index
		{
			get { return _Index; }
		}
		public bool Required
		{
			get { return _Required; }
		}
	}
}
