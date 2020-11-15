// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using System.Reflection;

namespace CommandLineUtility
{
	internal class ValidateArgumentInfo
	{
		public readonly MethodInfo MethodInfo;
		public readonly ValidateArgumentAttribute ValidateArgumentAttribute;

		public string Name
		{ get { return ValidateArgumentAttribute.Name; } }
		public int? Index
		{ get { return ValidateArgumentAttribute.Index; } }

		public bool HasName
		{ get { return this.ValidateArgumentAttribute.HasName; } }
		public bool HasIndex
		{ get { return this.ValidateArgumentAttribute.HasIndex; } }

		public ValidateArgumentInfo(MethodInfo methodInfo, ValidateArgumentAttribute validateArgumentAttribute)
		{
			this.MethodInfo = methodInfo;
			this.ValidateArgumentAttribute = validateArgumentAttribute;
		}
	}
}
