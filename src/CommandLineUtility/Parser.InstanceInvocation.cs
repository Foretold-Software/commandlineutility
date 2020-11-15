// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandLineUtility
{
	partial class CommandLineParser
	{
		private static void SetProperty(PropertyInfo property, object instance, object value)
		{
			try
			{ property.SetValue(instance, value, null); }
			catch (Exception exc)
			{ throw Exception(exc, "An error occurred while setting the '{0}' property with the value '{1}'.", property.Name, value.ToString()); }
		}

		private static void SetProperty_Cast(PropertyInfo property, object instance, List<object> list)
		{
			object value = list.CastToType(property.PropertyType);
			try
			{ property.SetValue(instance, value, null); }
			catch (Exception exc)
			{ throw Exception(exc, "An error occurred while setting the '{0}' property with the value '{1}'.", property.Name, value.ToString()); }
		}

		private static object Switch_InvokeValidationMethod(MethodInfo method, object instance, List<object> list)
		{
			try
			{
				var parameterType = method.GetParameters().First().ParameterType;
				object parameter = list.CastToType(parameterType);

				return method.Invoke(instance, new object[] { parameter });
			}
			catch (Exception exc)
			{ throw Exception(exc, "An error occurred while invoking the validation method: '{0}'. See inner exception(s) for detials.", method.Name); }
		}

		/// <summary>
		/// Call a validation method for a global indexed argument whose
		/// parameter's type is descended from the argument's property's type.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="instance"></param>
		/// <param name="castedArg"></param>
		/// <returns></returns>
		private static bool GIA_InvokeValidationMethod(MethodInfo method, object instance, string arg, object castedArg)
		{
			try
			{
				if (method.GetParameters().First().ParameterType == typeof(string))
					return (bool)method.Invoke(instance, new object[] { arg });
				else
					return (bool)method.Invoke(instance, new object[] { castedArg });
			}
			catch (Exception exc)
			{ throw Exception(exc, "An error occurred while invoking the '{0}' validation method.", method.Name); }
		}

		/// <summary>
		/// Call a validation method for the global unconsumed arguments whose
		/// parameter's type is descended from the argument's property's type.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="instance"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		private static bool GUA_InvokeValidationMethod(MethodInfo method, object instance, List<object> list)
		{
			object parameter = list.CastToType(method.GetParameters().First().ParameterType);

			try
			{ return (bool)method.Invoke(instance, new object[] { parameter }); }
			catch (Exception exc)
			{ throw Exception(exc, "An error occurred while invoking the '{0}' validation method.", method.Name); }
		}
	}
}
