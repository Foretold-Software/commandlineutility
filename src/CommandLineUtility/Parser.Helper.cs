// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using System;
using System.Linq;
using System.Reflection;

namespace CommandLineUtility
{
	partial class CommandLineParser
	{
		private static Type GetISettingsType()
		{
			var settingsClassTypes = Assembly
				.GetEntryAssembly()
				.GetTypes()
				.Where(type => type.ImplementsISettings())
				.ToList();

			if (settingsClassTypes.Count == 0)
				throw Exception("Unable to find a settings class that implements interface {0}.", typeof(ISettings));
			else
			{
				return settingsClassTypes.First();
			}
		}

		/// <summary>
		/// Gets the appropriate SwitchInfo object for the given arg, and filters the switch character(s) from the arg.
		/// </summary>
		/// <param name="settingsClassInfo"></param>
		/// <param name="arg"></param>
		/// <param name="switchInfo"></param>
		/// <returns></returns>
		private bool TryGetSwitch(ref string arg, out SwitchInfo switchInfo)
		{
			switchInfo = null;
			string tempArg = arg; //Need a temporary variable because 'ref' parameters are not allowed in anonymous methods, like the lambda expressions below.
			string switchIndicator = this.ParserInfo.SwitchIndicators.FirstOrDefault(indicator => tempArg.StartsWith(indicator));

			if (switchIndicator != null)
			{
				string switchName;
				arg = arg.Substring(switchIndicator.Length);

				for (int length = arg.Length; length > 0 && switchInfo == null; length--)
				{
					switchName = arg.Substring(0, length);

					switchInfo = this.SettingsInfo.Switches.FirstOrDefault(info => info.Name.Equals(switchName, this.SettingsInfo.ComparisonRule));
				}

				if (switchInfo != null &&							//If a switch was found...
					switchInfo.Name.Length < arg.Length &&			//...and it SHOULD be a joined-arguments switch...
					!switchInfo.SwitchAttribute.JoinArguments)		//...but it's NOT a joined-arguments switch...
				{
					switchInfo = null;								//...then we did not actually find a matching switch.
				}
			}

			return switchInfo != null;
		}

		private bool TryGetValidationMethod(string name, out ValidateArgumentInfo valArginfo)
		{
			valArginfo = this.SettingsInfo.ValidateArgumentInfos
							.FirstOrDefault(info => info.HasName && info.Name.Equals(name, this.SettingsInfo.ComparisonRule));

			return valArginfo != null;
		}

		private bool TryGetValidationMethod(int index, out ValidateArgumentInfo valArginfo)
		{
			valArginfo = this.SettingsInfo.ValidateArgumentInfos
							.FirstOrDefault(info => info.Index == index);

			return valArginfo != null;
		}

		private bool TryGetValidationMethod(out ValidateArgumentInfo valArginfo)
		{
			valArginfo = this.SettingsInfo.ValidateArgumentInfos
							.FirstOrDefault(info => !info.HasName && !info.HasIndex);

			return valArginfo != null;
		}
	}
}
