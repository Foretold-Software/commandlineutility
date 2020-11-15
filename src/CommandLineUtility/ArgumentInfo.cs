// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using System.Reflection;

namespace CommandLineUtility
{
	internal class ArgumentInfo
	{
		public readonly PropertyInfo PropertyInfo;
		public readonly ArgumentAttribute ArgumentAttribute;

		public int Index
		{ get { return this.ArgumentAttribute.Index; } }

		public ValidateArgumentInfo ValidateArgumentInfo { get; set; }

		public ArgumentInfo(PropertyInfo propertyInfo, ArgumentAttribute argumentAttribute)
		{
			this.PropertyInfo = propertyInfo;
			this.ArgumentAttribute = argumentAttribute;
		}
	}
}
