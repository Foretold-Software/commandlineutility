// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLineUtility
{
	partial class CommandLineParser
	{
		public ISettings ParseSettings()
		{
			//Note: The below shorthand variable names are as follows:
			// sArg = Switch Argument
			// giArg = Global Indexed Argument
			// guArg = Global Unconsumed Argument

			int giArgIndex = 0;                             //Holds the current index of the global indexed arguments
			var sArgValues = new List<object>();            //Contains the values of the current switch's arguments
			var giArgValues = new List<object>();           //Contains the values of the global indexed arguments
			var guArgValues = new List<object>();           //Contains the values of the global unconsumed arguments

			SwitchInfo switchInfo;                          //Contains the current switch's info
			var args = CommandLineArgs.Get();               //Contains the command line arguments without the executable path as the first argument.
			var previousSwitches = new List<SwitchInfo>();  //Tracks which switches have been used so far


			for (int i = 0; i < args.Length; i++)
			{
				string arg = args[i];

				//If the current arg is a switch...
				if (TryGetSwitch(ref arg, out switchInfo))
				{
					//Check the switch's exclusivity with previous switches
					Switch_CheckExclusivity(switchInfo, previousSwitches);

					//Check the switch's max occurrences
					Switch_CheckMaxOccurrences(switchInfo, previousSwitches);

					//Retrieve the eligible switch arguments
					sArgValues = Switch_RetrieveEligibleArguments(switchInfo, arg, args.Skip(i + 1));

					//Validate the switch arguments
					Switch_ValidateArguments(switchInfo, ref sArgValues);

					//Set the property value for the current switch
					Switch_SetPropertyValue(switchInfo, sArgValues);

					//Jump ahead in the list of arguments by the number of switch arguments that were actually consumed
					if (!switchInfo.SwitchAttribute.JoinArguments)
						i += sArgValues.Count;

					//Add the current switch to the list of previous switches
					previousSwitches.Add(switchInfo);
				}
				//...else it is a global argument.
				else
				{
					//If there is still an available global indexed argument
					if (giArgIndex < this.SettingsInfo.GlobalIndexedArguments.Count)
					{
						GIA_ConsumeArgumentValue(giArgIndex, arg);

						giArgIndex++;
					}
					//...otherwise, it is a global unconsumed argument.
					else
					{
						//Add it to the list to save for later, until after we retrieve ALL the remaining global unconsumed arguments.
						guArgValues.Add(arg);
					}
				}
			}

			Switch_CheckRequirements(previousSwitches);
			GIA_CheckRequirements(giArgIndex);
			GUA_CheckRequirementsAndSetPropertyValue(guArgValues);

			return this.SettingsInfo.Instance;
		}

		#region ParseSettings() Switch Flow
		private void Switch_CheckExclusivity(SwitchInfo currentSwitch, List<SwitchInfo> previousSwitches)
		{
			SwitchInfo switchInfo = null;

			//Check whether the current switch's name is in
			// the "ExclusiveOf" property of any previous switches...
			switchInfo = previousSwitches.FirstOrDefault(prevSw =>
				prevSw.SwitchAttribute.ExclusiveOf != null &&
				prevSw.SwitchAttribute.ExclusiveOf.Any(excOfSw => excOfSw.Equals(currentSwitch.Name, this.SettingsInfo.ComparisonRule)));

			//...if not, then check whether the current switch's
			// "ExclusiveOf" property contains any of the previous switches' names.
			if (switchInfo == null)
			{
				switchInfo = previousSwitches.FirstOrDefault(prevSw =>
					currentSwitch.SwitchAttribute.ExclusiveOf != null &&
					currentSwitch.SwitchAttribute.ExclusiveOf.Any(excOfSw => excOfSw.Equals(prevSw.Name, this.SettingsInfo.ComparisonRule)));
			}

			//If either of these two conditions are met, then a previous switch was
			// found that is not compatible with the current switch...
			if (switchInfo != null)
			{
				//...so throw an exception.
				throw ArgumentException("The following switches are exclusive of each other, but are both present on the command line: '{0}' '{1}'", switchInfo.Name, currentSwitch.Name);
			}
		}

		private void Switch_CheckMaxOccurrences(SwitchInfo currentSwitch, List<SwitchInfo> previousSwitches)
		{
			//If there is a maximum number of times that the switch can appear in a single command line...
			if (currentSwitch.SwitchAttribute.MaxOccurrences > 0)
			{
				int previousAppearances = previousSwitches.Count(sw => sw.Name.Equals(currentSwitch.Name, this.SettingsInfo.ComparisonRule));

				//...then ensure that this maximum has not been exceeded.
				if (previousAppearances >= currentSwitch.SwitchAttribute.MaxOccurrences) // Use " >= " because previousAppearances does not include the current appearance of the switch.
				{
					throw new ArgumentException(string.Format("The '{0}' switch must not appear more than {1} time(s) on the command line.", currentSwitch.Name, currentSwitch.SwitchAttribute.MaxOccurrences));
				}
			}
		}

		private List<object> Switch_RetrieveEligibleArguments(SwitchInfo currentSwitch, string currentArg, IEnumerable<string> potentialArguments)
		{
			object obj;
			int argCount;                                               //The number of consumable, castible arguments
			List<string> switchArguments;                               //Contains the arguments to be consumed by a switch
			List<object> switchArgumentsCasted = new List<object>();    //Contains the arguments to be consumed by a switch, casted to the correct type

			//Gather all the consumable arguments immediately following the switch, up to the maximum allowed by the switch.
			if (currentSwitch.SwitchAttribute.JoinArguments)
			{
				switchArguments = Switch_GetArgumentsJoined(currentSwitch, currentArg);
			}
			else
			{
				switchArguments = Switch_GetArguments(currentSwitch, potentialArguments);
			}

			//Convert the string arguments to the appropriate objects.
			foreach (var switchArgument in switchArguments)
			{
				if (switchArgument.TryCastArg(currentSwitch.Type, out obj))
				{
					switchArgumentsCasted.Add(obj);
				}
				else break;
			}

			argCount = switchArgumentsCasted.Count;

			//If it is a joined argument switch and not all arguments could be converted, throw an exception.
			if (currentSwitch.SwitchAttribute.JoinArguments && argCount != switchArguments.Count)
				throw ArgumentException("The '{0}' switch contains {1} joined arguments on the command line, but only {2} of them could be converted: '{3}'", currentSwitch.Name, switchArguments.Count, argCount, _string.Join("' '", switchArguments));

			//If there are too few arguments, throw an exception.
			if (argCount < currentSwitch.SwitchAttribute.MinArguments)
				throw new ArgumentException(string.Format("The '{0}' switch requires at least {1} argument(s), {2} were provided on the command line: '{3}'", currentSwitch.Name, currentSwitch.SwitchAttribute.MinArguments, argCount, _string.Join("' '", switchArguments.Take(argCount))));

			return switchArgumentsCasted;
		}

		private List<string> Switch_GetArguments(SwitchInfo switchInfo, IEnumerable<string> args)
		{
			SwitchInfo temp;
			string tempArg;
			var arguments = new List<string>();

			foreach (var arg in args)
			{
				tempArg = arg; //Need a temp variable to pass as 'ref' within this foreach loop.

				if ((switchInfo.SwitchAttribute.MaxArguments < 0 || arguments.Count < switchInfo.SwitchAttribute.MaxArguments) &&   //If the switch can accept more arguments and...
					(this.ParserInfo.AllowSwitchCharsInArguments || !TryGetSwitch(ref tempArg, out temp)))                          //...switch chars are allowed in args OR it's not another switch...
				{
					//...then add it to the list of args.
					arguments.Add(arg);
				}
				else
				{
					//...otherwise break.
					break;
				}
			}

			return arguments;
		}

		private static List<string> Switch_GetArgumentsJoined(SwitchInfo switchInfo, string arg)
		{
			var arguments = new List<string>();

			if (arg.Length > switchInfo.Name.Length)
			{
				//If there is a separator, and it's a collection or a flags enum...
				if (!string.IsNullOrEmpty(switchInfo.SwitchAttribute.ArgumentSeparator) && (switchInfo.Type.IsCollection() || switchInfo.Type.IsFlagsEnum()))
				{
					//...then parse the arguments.
					arguments.AddRange(arg
						.Substring(switchInfo.Name.Length)
						.Split(switchInfo.SwitchAttribute.ArgumentSeparator.ToCharArray()));
				}
				else
				{
					//...otherwise the rest of the command line argument is the switch's argument.
					arguments.Add(arg.Substring(switchInfo.Name.Length));
				}

				//If there are too many arguments, throw an exception.
				if (switchInfo.SwitchAttribute.MaxArguments >= 0 && arguments.Count > switchInfo.SwitchAttribute.MaxArguments)
					throw ArgumentException("The '{0}' switch requires at most {1} argument(s), {2} were provided on the command line: '{3}'", switchInfo.Name, switchInfo.SwitchAttribute.MaxArguments, arguments.Count, _string.Join("' '", arguments));
			}
			//else there are no arguments.

			return arguments;
		}

		private void Switch_ValidateArguments(SwitchInfo currentSwitch, ref List<object> arguments)
		{
			object validity;
			int numValidArgs;
			ValidateArgumentInfo valArgInfo;

			//If there are any switch arguments, try to validate them.
			if (arguments.Count > 0 && TryGetValidationMethod(currentSwitch.Name, out valArgInfo))
			{
				//Get the number of switch arguments that passed validation.
				validity = Switch_InvokeValidationMethod(valArgInfo.MethodInfo, this.SettingsInfo.Instance, arguments);

				if (validity is int)
				{
					numValidArgs = (int)validity;
				}
				else if (validity is bool)
				{
					//Note: numValidArgs is assigned 'arguments.Count' instead of '1' because the method
					//  'Switch_InvokeValidationMethod' above may combine several arguments into one in
					//  the case of flag enums, resulting in a bool return type.
					numValidArgs = (bool)validity ? arguments.Count : 0;

					if (numValidArgs == 0 && !this.ParserInfo.ContinueOnFailedValidation)
						throw ArgumentException("Switch argument did not pass validation: {0}", arguments.First().ToString());
				}
				else throw Exception("Method '{0}' has an incorrect return type: {1}. Run program in \"Debug\" configuration to check the validity of your settings class.", valArgInfo.MethodInfo.Name, validity.GetType());

				//If an invalid count was returned, throw an exception.
				if (numValidArgs < 0 || numValidArgs > arguments.Count)
					throw Exception("Switch argument validation method returned an incorrent argument count: {0} returned / {1} possible", numValidArgs, arguments.Count);

				//If not all joined arguments passed validation then fail, because remaining arguments cannot be consumed globally since they are joined arguments.
				//Note: This scenario will be allowed if ContinueOnFailedValidation is true.
				if (currentSwitch.SwitchAttribute.JoinArguments && numValidArgs < arguments.Count && !this.ParserInfo.ContinueOnFailedValidation)
					throw ArgumentException("Switch '{0}' contains joined arguments that did not pass validation. {1} passed / {2} provided: '{3}'", currentSwitch.Name, numValidArgs, arguments.Count, _string.Join("' '", arguments));

				//If there are too few arguments, throw an exception.
				if (numValidArgs < currentSwitch.SwitchAttribute.MinArguments)
					throw ArgumentException("The '{0}' switch requires at least {1} argument(s) to pass validation, but only {2} passed: '{3}'", currentSwitch.Name, currentSwitch.SwitchAttribute.MinArguments, numValidArgs, _string.Join("' '", arguments));

				//Resize the switch argument list to the its new count.
				arguments = arguments.Take(numValidArgs).ToList();
			}
		}

		private void Switch_SetPropertyValue(SwitchInfo switchInfo, List<object> arguments)
		{
			int numArgs = arguments.Count;

			//If there are no arguments, try to use the SwitchAttribute's SetValue property.
			if (arguments.Count == 0)
			{
				//SetValue is given the correct value by GetSwitchInfos() method
				SetProperty(switchInfo.PropertyInfo, this.SettingsInfo.Instance, switchInfo.SwitchAttribute.SetValue);
			}
			//If there are switch arguments...
			else if (arguments.Count > 0)
			{
				//Assign the object to the settings class's property.
				SetProperty_Cast(switchInfo.PropertyInfo, this.SettingsInfo.Instance, arguments);
			}
		}

		private void Switch_CheckRequirements(List<SwitchInfo> switches)
		{
			//Check that all required scitches have their MinOccurrences requirement met.
			foreach (var sw in this.SettingsInfo.Switches)
			{
				int numOccurrences = switches.Count(info => info.Name.Equals(sw.Name, this.SettingsInfo.ComparisonRule));

				//Note: This is checking for names that are hard-coded in, so capitalization should never be an issue here.
				if (sw.SwitchAttribute.MinOccurrences > numOccurrences)
					throw ArgumentException("Switch '{0}' requires a minimum of {1} occurrences, but only {2} occurrences are present in the command line.", sw.Name, sw.SwitchAttribute.MinOccurrences, numOccurrences);
			}
		}
		#endregion

		#region ParseSettings() GIA Flow
		private void GIA_ConsumeArgumentValue(int giArgIndex, string arg)
		{
			object castedArgument;
			ValidateArgumentInfo validateArgumentInfo;
			var argumentInfo = this.SettingsInfo.GlobalIndexedArguments[giArgIndex];

			//Cast the argument to the correct type.
			if (!arg.TryCastArg(argumentInfo.PropertyInfo.PropertyType, out castedArgument))
				throw Exception("Unable to cast global indexed argument {0} to type {1}: '{2}'", giArgIndex, argumentInfo.PropertyInfo.PropertyType, arg);

			//If there is a validation method...
			if (TryGetValidationMethod(giArgIndex, out validateArgumentInfo))
			{
				//...try to validate...
				if (GIA_InvokeValidationMethod(validateArgumentInfo.MethodInfo, this.SettingsInfo.Instance, arg, castedArgument))
				{
					//...and set the value.
					SetProperty(argumentInfo.PropertyInfo, this.SettingsInfo.Instance, castedArgument);
				}
				else if (!this.ParserInfo.ContinueOnFailedValidation)
					throw ArgumentException("Global indexed argument {0} did not pass validation: '{1}'", giArgIndex, arg);
			}
			//If there is no validation method...
			else
			{
				//...then set the value.
				SetProperty(argumentInfo.PropertyInfo, this.SettingsInfo.Instance, castedArgument);
			}
		}

		private void GIA_CheckRequirements(int giArgIndex)
		{
			//The first n number of global indexed arguments have been assigned to.
			//  So if any arguments AFTER the first n are required, then we know that
			//  a required global indexed argument has not been assigned to.
			// Note: n is giArgIndex.
			foreach (var argInfo in this.SettingsInfo.GlobalIndexedArguments.Skip(giArgIndex))
			{
				if (argInfo.ArgumentAttribute.Required)
					throw ArgumentException("Global indexed argument '{0}' is required, but is not present in the command line.", argInfo.Index);
			}
		}
		#endregion

		#region ParseSettings() GUA Flow
		private void GUA_CheckRequirementsAndSetPropertyValue(List<object> guArgValues)
		{
			ValidateArgumentInfo validateArgumentInfo;
			ISettings instance = this.SettingsInfo.Instance;

			//If there are global unconsumed arguments, then assign
			// them to the GlobalUnconsumedArguments property...
			if (guArgValues.Count() > 0)
			{
				//...unless it is not allowed, then throw an exception.
				if (this.ParserInfo.UnconsumedArgumentMode == UnconsumedArgumentMode.NotAllowed)
					throw ArgumentException("Global unconsumed arguments not allowed in the command line: '{0}'", _string.Join("' '", guArgValues));
				else
				{
					//If there is a validation method...
					if (TryGetValidationMethod(out validateArgumentInfo))
					{
						//...try to validate...
						if (GUA_InvokeValidationMethod(validateArgumentInfo.MethodInfo, instance, guArgValues))
						{
							//...and set the value.
							SetProperty_Cast(this.SettingsInfo.GlobalUnconsumedArguments, instance, guArgValues);
						}
						else if (!this.ParserInfo.ContinueOnFailedValidation)
							throw ArgumentException("Global unconsumed arguments did not pass validation: '{0}'", _string.Join("' '", guArgValues));
					}
					//If there is no validation method...
					else
					{
						//...then set the value.
						SetProperty_Cast(this.SettingsInfo.GlobalUnconsumedArguments, instance, guArgValues);
					}
				}
			}
			else if (this.ParserInfo.UnconsumedArgumentMode == UnconsumedArgumentMode.Required)
			{
				//If there are no global unconsumed arguments, but they're required, then throw an exception.
				throw ArgumentException("Global unconsumed arguments are required, but are not present in the command line.");
			}
		}
		#endregion
	}
}
