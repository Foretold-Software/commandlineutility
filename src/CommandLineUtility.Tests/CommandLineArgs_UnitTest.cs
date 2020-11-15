// Copyright (c) Foretold Software, LLC. All rights reserved. Licensed under the Microsoft Public License (MS-PL). See the license.md file in the project root directory for full license information.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommandLineUtility.Tests
{
	[TestClass]
	public class CommandLineArgs_UnitTest
	{
		[TestMethod]
		public void Set()
		{
			CommandLineArgs.Set("1", "2", "3");
			var args = CommandLineArgs.Get();

			Assert.AreNotEqual(null, args);
			Assert.AreEqual(3, args.Length);

			Assert.AreEqual("1", args[0]);
			Assert.AreEqual("2", args[1]);
			Assert.AreEqual("3", args[2]);
		}
	}
}
