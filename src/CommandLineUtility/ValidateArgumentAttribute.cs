// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using System;

namespace CommandLineUtility
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	public sealed class ValidateArgumentAttribute : Attribute
	{
		/// <summary>
		/// Validate global unconsumed argument(s).
		/// </summary>
		public ValidateArgumentAttribute()
		{
			this._Index = null;
			this._Name = null;
		}
		/// <summary>
		/// Validate global indexed argument(s).
		/// </summary>
		public ValidateArgumentAttribute(int index)
		{
			this._Index = index;
			this._Name = null;
		}
		/// <summary>
		/// Validate a switch's argument(s).
		/// </summary>
		public ValidateArgumentAttribute(string name)
		{
			this._Index = null;
			this._Name = name;
		}

		readonly int? _Index;
		readonly string _Name;

		public int? Index
		{
			get { return _Index; }
		}
		public string Name
		{
			get { return _Name; }
		}

		internal bool HasName
		{
			get { return !_string.IsNullOrWhiteSpace(this.Name); }
		}
		internal bool HasIndex
		{
			get { return this.Index.HasValue; }
		}
	}
}
