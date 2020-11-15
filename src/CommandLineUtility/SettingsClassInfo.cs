// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandLineUtility
{
	internal partial class SettingsClassInfo
	{
		#region Fields
		internal Type Type;
		internal ISettings Instance;
		internal ParserInfo ParserInfo;

		internal PropertyInfo GlobalUnconsumedArguments;
		internal List<SwitchInfo> Switches;
		internal List<ArgumentInfo> GlobalIndexedArguments;
		internal List<ValidateArgumentInfo> ValidateArgumentInfos;
		#endregion

		#region Properties
		internal StringComparison ComparisonRule
		{
			get
			{
				return this.ParserInfo.SwitchesAreCaseSensitive ?
					StringComparison.InvariantCulture :
					StringComparison.InvariantCultureIgnoreCase;
			}
		}
		#endregion

		#region Constructors
		internal SettingsClassInfo() { }
		//Note: No need to check that this type is ISettings, this should already be done.
		internal SettingsClassInfo(Type type, ParserInfo parserInfo)
		{
			this.Type = type;
			this.ParserInfo = parserInfo;
			this.Instance = GetInstance(this.Type);

			this.Initialize();
		}
		internal SettingsClassInfo(ISettings settingsObject, ParserInfo parserInfo)
		{
			this.Type = settingsObject.GetType();
			this.ParserInfo = parserInfo;
			this.Instance = settingsObject;

			this.Initialize();
		}
		#endregion

		#region Methods
		private void Initialize()
		{
			//Get the 'AllUnconsumedArguments' property.
			this.GlobalUnconsumedArguments = this.Type.GetProperty("GlobalUnconsumedArguments", BindingFlags.Public | BindingFlags.Instance);

			//Get the switches.
			this.Switches = GetSwitchInfos();

			//Get the global indexed arguments, ordered by index.
			this.GlobalIndexedArguments = GetArgumentInfos();

			//Get the argument validation methods.
			this.ValidateArgumentInfos = GetValidateArgumentInfos();

			//Associate ValidateArgumentInfo items with their switches and global arguments.
			foreach (var switchInfo in this.Switches)
			{
				switchInfo.ValidateArgumentInfo = this.ValidateArgumentInfos
					.Where(info => info.ValidateArgumentAttribute.HasName)
					.FirstOrDefault(info => info.Name.Equals(switchInfo.Name, this.ComparisonRule));
			}
			foreach (var argInfo in this.GlobalIndexedArguments)
			{
				argInfo.ValidateArgumentInfo = this.ValidateArgumentInfos
					.Where(info => info.ValidateArgumentAttribute.HasIndex)
					.FirstOrDefault(info => info.Index == argInfo.Index);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static ISettings GetInstance(Type type)
		{
			try
			{ return Activator.CreateInstance(type) as ISettings; }
			catch (Exception exc)
			{ throw Exception(exc, "An error occurred while invoking the default (parameterless) constructor of type {0}.", type); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private List<SwitchInfo> GetSwitchInfos()
		{
			var allProperties = this.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			var allSwitchProperties = allProperties.Where(prop => prop.GetCustomAttributes(typeof(SwitchAttribute), false).Count() > 0);

			var switchInfos = new List<SwitchInfo>();
			foreach (var property in allSwitchProperties)
			{
				//Get all the attributes applied to this property.
				var switchAttributes = property.GetCustomAttributes(typeof(SwitchAttribute), false) as SwitchAttribute[];

				//Ensure that each switch for a given property is exclusive of every other switch on that same property.
				if (ParserInfo.PropertySwitchesAreExclusive)
				{
					foreach (var attribute1 in switchAttributes)
					{
						foreach (var attribute2 in switchAttributes)
						{
							if (!attribute1.Name.Equals(attribute2.Name, this.ComparisonRule) &&						//If they're not the same switch...
								!attribute1.ExclusiveOf.Any(excl => excl.Equals(attribute2.Name, this.ComparisonRule)))	//...and the first switch's ExclusiveOf does not contain the name of the second switch...
							{
								attribute1.AddExclusion(attribute2.Name);												//...then add the second switch's name to the ExclusiveOf list of the first switch.
							}
						}
					}
				}

				//There will be one SwitchInfo for every SwitchAttribute.
				foreach (var attribute in switchAttributes)
				{
					//If there is no name, give it the name of it's property.
					if (_string.IsNullOrWhiteSpace(attribute.Name))
						attribute.Name = property.Name;

					//If the property allows arguments and it is not a collection and it is not a flags enum, then restrict it to one argument.
					if (attribute.MaxArguments != 0 && !property.PropertyType.IsCollection() && !property.PropertyType.IsFlagsEnum())
						attribute.MaxArguments = 1;

					//If the property is a collection and no ArgumentSeparator is specified, then assign it the default separator.
					if (property.PropertyType.IsCollection() && attribute.ArgumentSeparator == null)
						attribute.ArgumentSeparator = SwitchAttribute.DefaultArgumentSeparator;

					//If SetValue is null and the property is a value type, give it the 'default' value of its property's type.
					if (attribute.SetValue == null && property.PropertyType.IsValueType)
					{
						attribute.SetValue = Activator.CreateInstance(property.PropertyType);
					}

					switchInfos.Add(new SwitchInfo(property, attribute));
				}
			}

			return switchInfos;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private List<ArgumentInfo> GetArgumentInfos()
		{
			var allProperties = this.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			var giArgProperties = allProperties.Where(prop => prop.GetCustomAttributes(typeof(ArgumentAttribute), false).Count() == 1);

			var argInfos = new List<ArgumentInfo>();
			foreach (var property in giArgProperties)
			{
				//Get the ArgumentAttribute attribute applied to this property.
				var attribute = (property.GetCustomAttributes(typeof(ArgumentAttribute), false) as ArgumentAttribute[])
					//There should only be one, never more, and never less.
					.First();

				argInfos.Add(new ArgumentInfo(property, attribute));
			}

			return argInfos.OrderBy(argInfo => argInfo.Index).ToList();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private List<ValidateArgumentInfo> GetValidateArgumentInfos()
		{
			var allMethods = this.Type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod);
			var allValidateArgumentMethods = allMethods.Where(method => method.GetCustomAttributes(typeof(ValidateArgumentAttribute), false).Count() > 0);

			var validateArgumentInfos = new List<ValidateArgumentInfo>();
			foreach (var method in allValidateArgumentMethods)
			{
				//Get all the attributes applied to this property.
				var validateArgumentAttributes = method.GetCustomAttributes(typeof(ValidateArgumentAttribute), false) as ValidateArgumentAttribute[];

				//There will be one SwitchInfo for every SwitchAttribute.
				foreach (var attribute in validateArgumentAttributes)
				{
					validateArgumentInfos.Add(new ValidateArgumentInfo(method, attribute));
				}
			}

			return validateArgumentInfos;
		}
		#endregion
	}
}
