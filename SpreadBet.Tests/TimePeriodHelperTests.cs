using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadBet.Common.Helpers;

namespace SpreadBet.Tests
{
	[TestClass]
	public class TimePeriodHelperTests
	{
		[TestMethod]
		public void GetTimePeriod_WithKnownDateTime_ReturnsCorrectPeriodId()
		{
			var result1 = TimePeriodHelper.GetTimePeriod(DateTime.Parse("01 January 2013 03:14:23"), (60 * 60));
			var result2 = TimePeriodHelper.GetTimePeriod(DateTime.Parse("01 January 2013 04:44:23"), (60 * 60));

			Assert.AreEqual(1, result2 - result1);
		}
	}
}
