// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLineUtility
{
	partial class CommandLineParser
	{
		/// <summary>
		/// Validates a class inheriting from <see cref="ISettings"/> to
		/// ensure that it follows the rules of this command line parser.
		/// 
		/// To improve reliability, <see cref="ValidateSettingsClass"/> is called
		/// by every overload of the <see cref="CommandLineParser"/> constructor
		/// and static <see cref="GetSettings"/> methods.
		/// 
		/// To improve performance, those constructors and methods will only call
		/// <see cref="ValidateSettingsClass"/> if a debugger is attached to the process.
		/// </summary>
		public void ValidateSettingsClass()
		{
			List<SwitchInfo> switchInfos;
			List<ArgumentInfo> argInfos;
			List<ValidateArgumentInfo> valArgInfos;

			Type stringType = typeof(string);

			if (!this.SettingsInfo.Type.ImplementsISettings())
				throw Exception("The settings class type does not implement the {0} interface.", typeof(ISettings));

			#region Switches
			if ((switchInfos = this.SettingsInfo.Switches).Count > 0)
			{
				//Ensure that no switch names are repeated.
				for (int i = 0; i < switchInfos.Count - 1; i++)
				{
					for (int j = i + 1; j < switchInfos.Count; j++)
					{
						if (switchInfos[i].Name.Equals(switchInfos[j].Name, this.ComparisonRule))
						{
							throw Exception("Switch names repeated: '{0}' and '{1}'", switchInfos[i].Name, switchInfos[j].Name);
						}
					}
				}

				foreach (var switchInfo in switchInfos)
				{
					//Ensure that no switch is exclusive of itself.
					if (switchInfo.SwitchAttribute.ExclusiveOf != null && switchInfo.SwitchAttribute.ExclusiveOf.Any(name => name.Equals(switchInfo.Name, this.ComparisonRule)))
						throw Exception("Switch '{0}' cannot be exclusive of itself.", switchInfo.Name);

					//Ensure that no switch has invalid occurrence or argument constraints.
					if (switchInfo.SwitchAttribute.MinOccurrences < 0)
						throw Exception("Switch '{0}' MinOccurrences must be a non-negative number: {1}", switchInfo.Name, switchInfo.SwitchAttribute.MinOccurrences);
					if (switchInfo.SwitchAttribute.MaxOccurrences == 0) //'MaxOccurrences < 0' is allowed, this indicates unlimited occurrences.
						throw Exception("Switch '{0}' MaxOccurrences must be a non-zero number: {1}", switchInfo.Name, switchInfo.SwitchAttribute.MaxOccurrences);
					if (switchInfo.SwitchAttribute.MaxOccurrences > 0 && switchInfo.SwitchAttribute.MinOccurrences > switchInfo.SwitchAttribute.MaxOccurrences)
						throw Exception("Switch '{0}' MinOccurrences '{1}' cannot be greater than MaxOccurrences '{2}'", switchInfo.Name, switchInfo.SwitchAttribute.MinOccurrences, switchInfo.SwitchAttribute.MaxOccurrences);
					if (switchInfo.SwitchAttribute.MinArguments < 0)
						throw Exception("Switch '{0}' MinArguments must be a non-negative number: {1}", switchInfo.Name, switchInfo.SwitchAttribute.MinArguments);
					if (switchInfo.SwitchAttribute.MaxArguments > 0 && switchInfo.SwitchAttribute.MinArguments > switchInfo.SwitchAttribute.MaxArguments)
						throw Exception("Switch '{0}' MinArguments '{1}' cannot be greater than MaxArguments '{2}'", switchInfo.Name, switchInfo.SwitchAttribute.MinArguments, switchInfo.SwitchAttribute.MaxArguments);

					//Ensure that every switch property is a type that is convertible by this command line parser.
					if (!switchInfo.Type.IsConvertibleOrCollectionOfConvertibleType())
						throw Exception("Switch '{0}' property type '{1}' must be a convertible type or a collection of a convertible type, where a convertible type is an enum or some type convertible to by System.IConvertible interface.", switchInfo.Name, switchInfo.Type);

					//Ensure that only collections have an ArgumentSeparator.
					if (switchInfo.SwitchAttribute.ArgumentSeparator != null && !switchInfo.Type.IsCollection() && !switchInfo.Type.IsFlagsEnum())
						throw Exception("Switch '{0}' is not a collection, and therefore should not specify an ArgumentSeparator '{1}'.", switchInfo.Name, switchInfo.SwitchAttribute.ArgumentSeparator);

					//Ensure that ONLY collections allow the parser to consume more than one argument.
					//Note: MaxArguments can be -1, which will result in only one argument being used anyway, since it is not a collection.
					if (!switchInfo.Type.IsCollection() && (switchInfo.SwitchAttribute.MaxArguments > 1 || switchInfo.SwitchAttribute.MinArguments > 1))
						throw Exception("Switch '{0}' property must be a collection to allow more than one argument. It is type '{1}'.", switchInfo.Name, switchInfo.Type);

					//Ensure SetValue is the same type as or a descendant type of the property, or null.
					//Note: With the types that I am allowing, there is no possibility of having subclasses, all the types are structs or sealed.
					if (switchInfo.SwitchAttribute.SetValue != null && !TypeIsGTE(switchInfo.Type, switchInfo.SwitchAttribute.SetValue.GetType()))
						throw Exception("Switch '{0}' SetValue type '{1}' must be the same type as- or a descendent type of- the property type '{2}'.", switchInfo.Name, switchInfo.SwitchAttribute.SetValue.GetType(), switchInfo.Type);

					//Ensure the property's 'set' method is public.
					if (switchInfo.PropertyInfo.GetSetMethod() == null || !switchInfo.PropertyInfo.GetSetMethod().IsPublic)
						throw Exception("Switch '{0}' property must have a public 'set' method.", switchInfo.Name);

					//Ensure that each value in 'ExclusiveOf' is a valid switch name, that it exists.
					if (switchInfo.SwitchAttribute.ExclusiveOf != null)
						foreach (var exclusiveOf in switchInfo.SwitchAttribute.ExclusiveOf)
						{
							if (!switchInfos.Any(info => info.Name.Equals(exclusiveOf, this.ComparisonRule)))
								throw Exception("Switch '{0}' 'ExclusiveOf' value '{1}' is not a valid switch, it doesn't exist.", switchInfo.Name, exclusiveOf);
						}
				}

				//Ensure there are no switches that are both required and also exclusive of each other.
				var requiredSwitchInfos = switchInfos.Where(info => info.SwitchAttribute.MinArguments > 0).ToList();
				for (int i = 0; i < requiredSwitchInfos.Count - 1; i++)
				{
					for (int j = i + 1; j < requiredSwitchInfos.Count; j++)
					{
						if ((
							requiredSwitchInfos[i].SwitchAttribute.ExclusiveOf != null &&
							requiredSwitchInfos[i].SwitchAttribute.ExclusiveOf.Any(excOf => excOf.Equals(requiredSwitchInfos[j].Name, this.ComparisonRule))
							) || (
							requiredSwitchInfos[j].SwitchAttribute.ExclusiveOf != null &&
							requiredSwitchInfos[j].SwitchAttribute.ExclusiveOf.Any(excOf => excOf.Equals(requiredSwitchInfos[i].Name, this.ComparisonRule))))
						{
							throw Exception("Switches '{0}' and '{1}' are exclusive of each other, and so they cannot both be required.", requiredSwitchInfos[i].Name, requiredSwitchInfos[j].Name);
						}
					}
				}
			}
			#endregion

			#region Global Indexed Arguments
			if ((argInfos = this.SettingsInfo.GlobalIndexedArguments).Count > 0)
			{
				//Ensure that no argument index is repeated.
				for (int i = 0; i < argInfos.Count - 1; i++)
				{
					for (int j = i + 1; j < argInfos.Count; j++)
					{
						if (argInfos[i].Index == argInfos[j].Index)
						{
							throw Exception("Argument index repeated: '{0}'", argInfos[i].Index);
						}
					}
				}

				//Ensure that argument indices are consecutive.
				var maxArgIndex = argInfos.Max(info => info.Index);
				for (int i = 0; i < maxArgIndex; i++)
				{
					if (!argInfos.Any(info => info.Index == i))
						throw Exception("Argument {0} is missing. The highest argument index is {1}, therefore every index between 0 and {1} (inclusive) must be present.", i, maxArgIndex);
				}

				foreach (var argInfo in argInfos)
				{
					//Ensure there are no negative argument indices.
					if (argInfo.Index < 0)
						throw Exception("Argument {0} must have a non-negative integer as its index.", argInfo.Index);

					//Ensure the type is an IConvertible type or an enum type.
					if (!argInfo.PropertyInfo.PropertyType.IsConvertibleType())
						throw Exception("Argument {0} property type '{1}' must be a type that can be converted to by the System.IConvertible interface, or an enum type.", argInfo.Index, argInfo.PropertyInfo.PropertyType);

					//Ensure the property's 'set' method is public.
					if (argInfo.PropertyInfo.GetSetMethod() == null || !argInfo.PropertyInfo.GetSetMethod().IsPublic)
						throw Exception("Argument '{0}' property must have a public 'set' method.", argInfo.Index);
				}
			}
			#endregion

			#region Validate Argument Methods

			if ((valArgInfos = this.SettingsInfo.ValidateArgumentInfos).Count > 0)
			{
				foreach (var valArgInfo in valArgInfos)
				{
					var parameters = valArgInfo.MethodInfo.GetParameters();

					//Ensure the ValidateArgumentAttribute does not have both a switch name and an argument index.
					//Note: This condition MUST BE checked FIRST.
					if (valArgInfo.HasName && valArgInfo.HasIndex)
						throw Exception("Argument validation method '{0}' may have either a name or an index, but not both.", valArgInfo.ValidateArgumentAttribute.Name);

					if (valArgInfo.HasName) //It's for a switch.
					{
						string name = valArgInfo.Name;
						SwitchInfo switchInfo = switchInfos.FirstOrDefault(info => info.Name.Equals(name, this.ComparisonRule));

						//Ensure the validation method has an associated SwitchAttribute.
						if (switchInfo == null)
							throw Exception("Argument validation method '{0}' does not have an associated switch.", name);

						//Ensure the ValidateArgumentAttribute method has exactly one parameter.
						if (parameters.Length != 1)
							throw Exception("Argument validation method '{0}' must have exactly one parameter.", name);
						else
						{
							var parameterType = parameters[0].ParameterType;
							var propertyType = switchInfo.PropertyInfo.PropertyType;
							//var propertyType = argInfos.First(argInfo => argInfo.Index == valArgInfo.Index).PropertyInfo.PropertyType;

							//Ensure that the types of the switch's property and the validation method's parameter are compatible.
							if (parameterType.IsCollection()) //The method's parameter is a collection.
							{
								Type parameterElementType = parameterType.GetElementTypeOrType();

								if (propertyType.IsCollection()) //The switch's property is a collection.
								{
									Type propertyElementType = propertyType.GetElementTypeOrType();

									//Ensure that the parameter's collection's element type is GTE the property's collection's element type, or is a string.
									if (!TypeIsGTE(parameterElementType, propertyElementType) && !parameterElementType.Equals(stringType))
										throw Exception("Argument validation method '{0}' has a parameter that is a collection whose element type '{1}' must be equal to or an ancestor type of '{2}', or is '{3}'.", name, parameterElementType, propertyElementType, stringType);
								}
								else //The switch's property is not a collection.
								{
									//Ensure that the parameter collection's element type is GTE the property's type, or is a string.
									if (!TypeIsGTE(parameterElementType, propertyType) && !parameterElementType.Equals(stringType))
										throw Exception("Argument validation method '{0}' has a parameter that is a collection whose element type '{1}' must be equal to or an ancestor type of '{2}', or is '{3}'.", name, parameterElementType, propertyType, stringType);
								}

								//Ensure the validation method has return type int.
								if (valArgInfo.MethodInfo.ReturnType != typeof(int))
									throw Exception("Argument validation method '{0}' must have return type '{1}'.", name, typeof(int));
							}
							else //The method's parameter is not a collection.
							{
								if (propertyType.IsCollection()) //The switch's property is a collection.
								{
									//Ensure the the method's parameter is a collection if the property is a collection.
									throw Exception("Argument validation method '{0}' has a parameter whose type '{1}' must be a collection with the same element type as its associated property '{2}'.", name, parameterType, propertyType);
								}
								else //Neither type is a collection.
								{
									//Ensure that the parameter type is GTE the property type, or is a string.
									if (!TypeIsGTE(parameterType, propertyType) && !parameterType.Equals(stringType))
										throw  Exception("Argument validation method '{0}' has a parameter whose type must be equal to or an ancestor type of '{1}', or is '{2}'.", name, propertyType, stringType);
								}

								//Ensure the validation method has return type bool.
								if (valArgInfo.MethodInfo.ReturnType != typeof(bool))
									throw Exception("Argument validation method '{0}' must have return type '{1}'.", name, typeof(bool));
							}
						}
					}
					else if (valArgInfo.ValidateArgumentAttribute.HasIndex)
					{
						int index = valArgInfo.ValidateArgumentAttribute.Index.Value;

						//Ensure the ValidateArgumentAttribute has an associated ArgumentAttribute.
						if (!argInfos.Any(info => info.Index == index))
							throw Exception("Argument validation method '{0}' does not have an associated argument.", index);

						//Ensure the ValidateArgumentAttribute has return type bool.
						if (valArgInfo.MethodInfo.ReturnType != typeof(bool))
							throw Exception("Argument validation method '{0}' must have return type '{1}'.", index, typeof(bool));

						//Ensure the ValidateArgumentAttribute method has exactly one parameter.
						if (parameters.Length != 1)
							throw Exception("Argument validation method '{0}' must have exactly one parameter.", index);
						else
						{
							var parameterType = parameters[0].ParameterType;
							var propertyType = argInfos.First(argInfo => argInfo.Index == valArgInfo.Index).PropertyInfo.PropertyType;

							//Ensure that the parameter type is GTE the property type, or is a string.
							if (!TypeIsGTE(parameterType, propertyType) && !parameterType.Equals(stringType))
								throw Exception("Argument validation method '{0}' has a parameter whose type must be equal to or an ancestor type of '{1}', or is '{2}'.", index, propertyType, stringType);
						}
					}
					else
					{
						//If the ValidateArgumentAttribute has no name and no index, then it is a global unconsumed argument validation method.
						string name = valArgInfo.MethodInfo.Name;

						//If global unconsumed arguments are not allowed, throw an exception.
						if (this.ParserInfo.UnconsumedArgumentMode == UnconsumedArgumentMode.NotAllowed)
							throw Exception("Argument validation method '{0}' is set to validate global unconsumed arguments, but they are not allowed.", name);

						//Ensure the ValidateArgumentAttribute has return type bool.
						if (valArgInfo.MethodInfo.ReturnType != typeof(bool))
							throw Exception("Global unconsumed argument validation method '{0}' must have return type '{1}'.", name, typeof(bool));

						//Ensure the ValidateArgumentAttribute has one parameter of type string[].
						if (parameters.Length != 1 || parameters[0].ParameterType != typeof(string[]))
							throw Exception("Global unconsumed argument validation method '{0}' must have one parameter of type '{1}'.", name, typeof(string[]));
					}
				}


				//Ensure argument validation method names/indices are not repeated.
				//Note: The logic of these if-statements could cause some issues if
				//  run BEFORE the previous foreach block, so DEFINITELY keep it AFTER that foreach block.
				for (int i = 0; i < valArgInfos.Count - 1; i++)
				{
					for (int j = i + 1; j < valArgInfos.Count; j++)
					{
						//Ensure that no switch argument validation methods are repeated.
						if (valArgInfos[i].ValidateArgumentAttribute.HasName && valArgInfos[j].ValidateArgumentAttribute.HasName)
						{
							if (valArgInfos[i].Name.Equals(valArgInfos[j].Name, this.ComparisonRule))
								throw Exception("Argument validation method name repeated: '{0}' and '{1}'", valArgInfos[i].Name, valArgInfos[j].Name);
						}
						//Ensure that no global argument validation methods are repeated.
						if (valArgInfos[i].ValidateArgumentAttribute.HasIndex && valArgInfos[j].ValidateArgumentAttribute.HasIndex)
						{
							if (valArgInfos[i].ValidateArgumentAttribute.Index.Value == valArgInfos[j].ValidateArgumentAttribute.Index.Value)
								throw Exception("Argument validation method index repeated: '{0}'", valArgInfos[i].ValidateArgumentAttribute.Index.Value);
						}
						//Ensure that there is only one validation method for global unconsumed arguments.
						if (!valArgInfos[i].ValidateArgumentAttribute.HasName && !valArgInfos[i].ValidateArgumentAttribute.HasIndex &&
							!valArgInfos[j].ValidateArgumentAttribute.HasName && !valArgInfos[j].ValidateArgumentAttribute.HasIndex)
						{
							throw Exception("Only one validation method allowed for global unconsumed arguments: '{0}' and '{1}'", valArgInfos[i].MethodInfo.Name, valArgInfos[j].MethodInfo.Name);
						}
					}
				}
			}

			#endregion
		}

		/// <summary>
		/// Determines whether the specified types are equal,
		/// or if the first is an ancestor of the second.
		/// </summary>
		private static bool TypeIsGTE(Type ancestorType, Type descendentType)
		{
			return descendentType.Equals(ancestorType) || descendentType.IsSubclassOf(ancestorType);
		}

	}
}
