// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using System;
using System.Reflection;

namespace CommandLineUtility
{
	internal class SwitchInfo
	{
		public readonly PropertyInfo PropertyInfo;
		public readonly SwitchAttribute SwitchAttribute;
		
		public string Name
		{ get { return this.SwitchAttribute.Name; } }
		public Type Type
		{ get { return this.PropertyInfo.PropertyType; } }

		public ValidateArgumentInfo ValidateArgumentInfo { get; set; }

		public SwitchInfo(PropertyInfo propertyInfo, SwitchAttribute switchAttribute)
		{
			this.PropertyInfo = propertyInfo;
			this.SwitchAttribute = switchAttribute;
		}
	}
}
