using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SpreadBet.Common.Entities;
using SpreadBet.Common.Interfaces;
using SpreadBet.Common.Components;

namespace SpreadBet.Tests
{
	[TestClass]
	public class CompositeStockFilterTests
	{
		[TestMethod]
		public void GetInvestmentCandidates_MultipleFilters_ExecutesAlLFilters()
		{
			var stocks = new Stock[]
			{
				new Stock { Id = "STK.1" }, 
				new Stock { Id = "STK.2" }, 
			};

			var filter1 = new Mock<IStockFilter>();
			filter1.Setup(x => x.GetInvestmentCandidates(stocks)).Returns(stocks);

			var filter2 = new Mock<IStockFilter>();
			filter2.Setup(x => x.GetInvestmentCandidates(stocks)).Returns(stocks);

			var filter3 = new Mock<IStockFilter>();
			filter3.Setup(x => x.GetInvestmentCandidates(stocks)).Returns(stocks);

			var compositeFilter = new CompositeStockFilter(filter1.Object, filter2.Object, filter3.Object);

			var result = compositeFilter.GetInvestmentCandidates(stocks);

			filter1.Verify(x => x.GetInvestmentCandidates(stocks), Times.Once());
			filter2.Verify(x => x.GetInvestmentCandidates(stocks), Times.Once());
			filter3.Verify(x => x.GetInvestmentCandidates(stocks), Times.Once());

			Assert.AreEqual(2, stocks.Count());
			Assert.IsTrue(stocks.Any(x => x.Id.Equals("STK.1")));
			Assert.IsTrue(stocks.Any(x => x.Id.Equals("STK.2")));
		}
	}
}
