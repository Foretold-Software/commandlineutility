// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandLineUtility
{
	internal static partial class Extensions
	{
		#region Constants
		private const string CollectionName = "System.Collections.Generic.ICollection`1";
		private static readonly string ISettingsName = typeof(ISettings).ToString();
		#endregion

		#region Get Convertible Types
		/// <summary>
		/// Gets an array of types that can be converted by the System.IConvertibleType interface.
		/// </summary>
		private static Type[] GetIConvertibleTypes()
		{
			var allMethods = typeof(IConvertible).GetMethods();
			var convertMethods = allMethods
				.Where(method =>
				{
					var parameters = method.GetParameters();

					return parameters.Length == 1 && parameters[0].ParameterType == typeof(IFormatProvider);
				});

			return convertMethods.Select(method => method.ReturnType).ToArray();
		}
		#endregion

		#region Convertible Type Queries
		/// <summary>
		/// Determines whether the specified type can be
		/// converted by the System.IConvertibleType interface.
		/// </summary>
		private static bool IsIConvertibleType(Type type)
		{
			return GetIConvertibleTypes().Any(iConvType => iConvType == type);
		}

		/// <summary>
		/// Determines whether the specified type can be
		/// converted from a string by this command line parser.
		/// </summary>
		internal static bool IsConvertibleType(this Type type)
		{
			return type.IsEnum || IsIConvertibleType(type);
		}

		/// <summary>
		/// Determines whether the specified type is a supported collection type.
		/// </summary>
		/// <param name="collectionType"></param>
		/// <returns></returns>
		internal static bool IsCollection(this Type collectionType)
		{
			return collectionType.IsArray || collectionType.GetInterface(CollectionName) != null;
		}

		/// <summary>
		/// Determines whether the specified type is a supported collection type of convertible types.
		/// </summary>
		/// <param name="collectionType"></param>
		/// <returns></returns>
		private static bool IsCollectionOfConvertibleType(Type collectionType)
		{
			return collectionType.IsCollection() && collectionType.GetElementTypeOrType().IsConvertibleType();
		}

		internal static bool IsConvertibleOrCollectionOfConvertibleType(this Type type)
		{
			return type.IsConvertibleType() || IsCollectionOfConvertibleType(type);
		}

		internal static Type GetElementTypeOrType(this Type type)
		{
			Type interfaceType;

			//Array: string[]
			if (type.IsArray)
				return type.GetElementType();
			//Collection: List<string>
			else if (null != (interfaceType = type.GetInterface(CollectionName)))
				return interfaceType.GetGenericArguments().First();
			//Convertible type: string
			else
				return type;
		}

		internal static bool ImplementsISettings(this Type type)
		{
			return type.GetInterface(ISettingsName) != null;
		}

		internal static bool IsFlagsEnum(this Type type)
		{
			//Note: GetCustomAttributes method will not return null.
			return type.IsEnum && type.GetCustomAttributes(typeof(FlagsAttribute), false).Count() > 0;
		}
		#endregion

		#region Casting
		internal static bool TryCastArg(this string arg, Type type, out object output)
		{
			try
			{
				output = CastArg(arg, type);
				return true;
			}
			catch
			{
				output = null;
				return false;
			}
		}

		private static object CastArg(string arg, Type type)
		{
			object castedObject = null;
			Type elementType = type.GetElementTypeOrType();

			if (elementType.IsEnum)
			{
				if (elementType.IsFlagsEnum())
				{
					object tempValue;

					foreach (var enumValue in arg.Split('|'))
					{
						tempValue = elementType.ParseEnum(enumValue.Trim());

						if (castedObject == null)
							castedObject = tempValue;
						else
						{
							castedObject = CombineEnumFlagValues(tempValue, castedObject);
						}
					}
				}
				else
				{
					castedObject = elementType.ParseEnum(arg);
				}
			}
			else
			{
				castedObject = Convert.ChangeType(arg, elementType);
			}

			return castedObject;
		}

		internal static object CastToType(this List<object> list, Type toType)
		{
			//If it's a flags enum...
			if (toType.IsFlagsEnum())
			{
				//Combine all the flag enum values.
				return list.CombineEnumFlagValues();
			}
			//If it's an array...
			else if (toType.IsArray)
			{
				//Note: Arrays need to be created differently than collections
				// because they do not have a public parameterless constructor.

				Type elementType = toType.GetElementTypeOrType();

				//Create an array of the correct type and length.
				Array array = Array.CreateInstance(elementType, list.Count);

				//Set each value in the array.
				for (int i = 0; i < list.Count; i++)
					array.SetValue(list[i], i);

				return array;
			}
			//If it's a collection...
			else if (toType.IsCollection())
			{
				object collection;
				MethodInfo method;

				//Create a new collection of the correct type.
				try { collection = Activator.CreateInstance(toType); }
				catch (Exception exc)
				{ throw Exception(exc, "A public parameterless constructor could not be found for type {0}.", toType); }

				//Get the 'Add' method.
				try { method = toType.GetMethod("Add", new Type[] { toType.GetElementTypeOrType() }); }
				catch (Exception exc)
				{ throw Exception(exc, "The 'Add' method could not be found in type {0}.", toType); }


				try
				{
					//Add each item to the collection.
					foreach (var item in list)
						method.Invoke(collection, new object[] { item });
				}
				catch (Exception exc)
				{ throw Exception(exc, "An error occurred while calling the 'Add' method in type {0}.", toType); }

				return collection;
			}
			//If it's a convertible type...
			else
			{
				return Convert.ChangeType(list.First(), toType);
			}
		}
		#endregion

		#region Enum Helper Methods
		private static object ParseEnum(this Type enumType, string enumValue)
		{
			try
			{ return Enum.Parse(enumType, enumValue, false); }
			catch
			{ return Enum.Parse(enumType, enumValue, true); }
		}

		internal static object CombineEnumFlagValues(this List<object> enumFlagValues)
		{
			object combinedValue = null;

			foreach (var flag in enumFlagValues)
			{
				if (combinedValue == null)
					combinedValue = flag;
				else
				{
					combinedValue = CombineEnumFlagValues(combinedValue, flag);
				}
			}

			return combinedValue;
		}

		private static object CombineEnumFlagValues(object enumValue1, object enumValue2)
		{
			var enumType = (enumValue1 ?? enumValue2).GetType();

			if (Enum.GetUnderlyingType(enumType) == typeof(byte))
			{
				return Enum.ToObject(enumType, (byte)enumValue1 | (byte)enumValue2);
			}
			else if (Enum.GetUnderlyingType(enumType) == typeof(int))
			{
				return Enum.ToObject(enumType, (int)enumValue1 | (int)enumValue2);
			}
			else if (Enum.GetUnderlyingType(enumType) == typeof(long))
			{
				return Enum.ToObject(enumType, (long)enumValue1 | (long)enumValue2);
			}
			else if (Enum.GetUnderlyingType(enumType) == typeof(sbyte))
			{
				return Enum.ToObject(enumType, (sbyte)enumValue1 | (sbyte)enumValue2);
			}
			else if (Enum.GetUnderlyingType(enumType) == typeof(short))
			{
				return Enum.ToObject(enumType, (short)enumValue1 | (short)enumValue2);
			}
			else if (Enum.GetUnderlyingType(enumType) == typeof(uint))
			{
				return Enum.ToObject(enumType, (uint)enumValue1 | (uint)enumValue2);
			}
			else if (Enum.GetUnderlyingType(enumType) == typeof(ulong))
			{
				return Enum.ToObject(enumType, (ulong)enumValue1 | (ulong)enumValue2);
			}
			else if (Enum.GetUnderlyingType(enumType) == typeof(ushort))
			{
				return Enum.ToObject(enumType, (ushort)enumValue1 | (ushort)enumValue2);
			}
			else throw Exception("Enum base type not support {0}.", enumType);
		}
		#endregion
	}
}
