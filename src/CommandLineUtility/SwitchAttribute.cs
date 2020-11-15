// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLineUtility
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
	public sealed class SwitchAttribute : Attribute
	{
		#region Fields
		private int _MinOccurrences;
		private int _MaxOccurrences;
		private int _MinArguments;
		private int _MaxArguments;
		private List<string> _ExclusiveOf;
		public static string DefaultArgumentSeparator = ";";
		#endregion

		#region Properties
		/// <summary>
		/// The name of the switch, as it will be used on the command line.
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// A list of the names of other switches that this
		/// switch CANNOT be used with on the same command line.
		/// </summary>
		public string[] ExclusiveOf
		{
			get
			{
				if (this._ExclusiveOf == null)
					this._ExclusiveOf = new List<string>();

				return this._ExclusiveOf.ToArray();
			}
			set
			{
				if (value == null)
					this._ExclusiveOf = new List<string>();
				else
					this._ExclusiveOf = value.ToList();
			}
		}
		/// <summary>
		/// A value to set to the associated property if this switch is specified.
		/// If the property type is an enum and SetValue is null, SetValue will be
		/// assigned the default value of the enum.
		/// </summary>
		public object SetValue { get; set; }

		/// <summary>
		/// Gets or sets the minumum number of occurrences of the current switch that are allowed in a command line.
		/// Generally, you do not want to REQUIRE a switch. If there is any required information, then it
		/// should be a global indexed argument, and its associated property should be decorated with the
		/// <see cref="ArgumentAttribute"/> attribute.
		/// </summary>
		public int MinOccurrences
		{
			get { return _MinOccurrences; }
			set
			{
				_MinOccurrences = value;
				//Note: This logic has been enabled in CommandLineParser.ValidateClass().
				//if (_MaxOccurrences > 0 && _MinOccurrences > _MaxOccurrences)
				//	_MaxOccurrences = _MinOccurrences;
			}
		}
		/// <summary>
		/// Gets or sets the maximum number of occurrences of the current
		/// switch that are allowed in a command line, or -1 if there is no limit.
		/// </summary>
		public int MaxOccurrences
		{
			get { return _MaxOccurrences; }
			set
			{
				_MaxOccurrences = value;
				//Note: This logic has been enabled in CommandLineParser.ValidateClass().
				//if (_MaxOccurrences > 0 && _MinOccurrences > _MaxOccurrences)
				//	_MinOccurrences = _MaxOccurrences;
			}
		}
		/// <summary>
		/// Gets or sets the minumum number of arguments that the current switch can consume.
		/// </summary>
		public int MinArguments
		{
			get { return _MinArguments; }
			set
			{
				_MinArguments = value;
				//Note: This logic has been enabled in CommandLineParser.ValidateClass().
				//if (_MaxArguments >= 0 && _MinArguments > _MaxArguments)
				//	_MaxArguments = _MinArguments;
			}
		}
		/// <summary>
		/// Gets or sets the maximum number of arguments that the current
		/// switch can consume, or -1 if there is no limit. If the property
		/// is not an array and this value is not 0, then it will be set to 1.
		/// </summary>
		public int MaxArguments
		{
			get { return _MaxArguments; }
			set
			{
				_MaxArguments = value;
				//Note: This logic has been enabled in CommandLineParser.ValidateClass().
				//if (_MaxArguments >= 0 && _MinArguments > _MaxArguments)
				//	_MinArguments = _MaxArguments;
			}
		}

		public bool JoinArguments { get; set; }
		/// <summary>
		/// Gets or sets one or more characters that are to be used
		/// to separate joined switch arguments. If this value is not
		/// specified or is an empty string, then the entire remainder
		/// of the argument string beyond the switch name is treated
		/// as one large switch argument.
		/// Note: Setting this value to String.Empty will ensure that
		/// the entire remainder of the argument is returned as one value.
		/// </summary>
		public string ArgumentSeparator { get; set; }
		#endregion

		#region Constructors
		public SwitchAttribute()
		{
			this.Name = string.Empty; //The parser will set the name to the name of the property, if it is not already set.
			this.ExclusiveOf = new string[0];
			this.SetValue = null;

			this.JoinArguments = false;
			this.ArgumentSeparator = null;

			this.MinOccurrences = 0;
			this.MaxOccurrences = -1;
			this.MinArguments = 0;
			this.MaxArguments = -1;
		}
		public SwitchAttribute(string name)
			: this()
		{
			this.Name = name;
		}
		public SwitchAttribute(string name, object setValue)
			: this()
		{
			this.Name = name;
			this.SetValue = setValue;
		}
		public SwitchAttribute(object setValue)
			: this()
		{
			this.SetValue = setValue;
		}
		#endregion

		#region Helper Methods
		/// <summary>
		/// A helper method to add an additional switch to the list of exclusive switches.
		/// </summary>
		/// <param name="switchName">The name of the switch to add to the list of exclusive switches.</param>
		internal void AddExclusion(string switchName)
		{
			////Turn it into a list.
			//var newExclusiveOf = new List<string>(this.ExclusiveOf ?? new string[0]);

			////Add the new exclusive switch's name.
			//newExclusiveOf.Add(switchName);

			////Turn it back into an array.
			//this.ExclusiveOf = newExclusiveOf.ToArray();

			//Add the new exclusive switch's name.
			this._ExclusiveOf.Add(switchName);
		}
		#endregion
	}
}
