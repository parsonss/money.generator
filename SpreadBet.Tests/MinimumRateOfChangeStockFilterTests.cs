using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadBet.Common.Entities;
using SpreadBet.Common.Interfaces;
using Moq;
using SpreadBet.Common.Components;

namespace SpreadBet.Tests
{
	[TestClass]
	public class MinimumRateOfChangeStockFilterTests
	{
		[TestMethod]
		public void GetInvestmentCandidates_RateOfGrowthSufficient_ReturnsStock()
		{
			var stock = new Stock
			{
				Id = "STK.PLC",
				Name = "STOCK PLC",
				Description = "The Test Stock Price"
			};

			var history = new StockPriceHistory
			{
				Stock = stock,
				Prices = new Dictionary<Period, Price>()
			};

			var currentDate = DateTime.UtcNow.AddMinutes(10);
			var lastPeriodEnd = DateTime.Parse(currentDate.ToString("dd MMMM yyyy HH:00:00"));

			history.Prices.Add(new Period { Id = 1, To = lastPeriodEnd.AddHours(-6) }, new Price { Mid = 25.23m });
			history.Prices.Add(new Period { Id = 2, To = lastPeriodEnd.AddHours(-5) }, new Price { Mid = 26.23m });
			history.Prices.Add(new Period { Id = 3, To = lastPeriodEnd.AddHours(-4) }, new Price { Mid = 28.23m });
			history.Prices.Add(new Period { Id = 4, To = lastPeriodEnd.AddHours(-3) }, new Price { Mid = 30.23m });
			history.Prices.Add(new Period { Id = 5, To = lastPeriodEnd.AddHours(-2) }, new Price { Mid = 30.25m });
			history.Prices.Add(new Period { Id = 6, To = lastPeriodEnd.AddHours(-1) }, new Price { Mid = 31.50m });
			history.Prices.Add(new Period { Id = 7, To = lastPeriodEnd.AddHours(0) }, new Price { Mid = 35.40m });

			var stockPriceProvider = new Mock<IStockHistoryDataProvider>();
			stockPriceProvider.Setup(x => x.GetStockHistory(stock, 5)).Returns(history);

			var filter = new MinimumRateOfChangeStockFilter(stockPriceProvider.Object,5, 25m, 25m);

			var results = filter.GetInvestmentCandidates(new Stock[] { stock });

			Assert.AreEqual(1, results.Count());

		}

		[TestMethod]
		public void GetInvestmentCandidates_RateOfGrowthInsufficient_ReturnsNothing()
		{
			var stock = new Stock
			{
				Id = "STK.PLC",
				Name = "STOCK PLC",
				Description = "The Test Stock Price"
			};

			var history = new StockPriceHistory
			{
				Stock = stock,
				Prices = new Dictionary<Period, Price>()
			};

			var currentDate = DateTime.UtcNow.AddMinutes(10);
			var lastPeriodEnd = DateTime.Parse(currentDate.ToString("dd MMMM yyyy HH:00:00"));

			history.Prices.Add(new Period { Id = 1, To = lastPeriodEnd.AddHours(-6) }, new Price { Mid = 25.23m });
			history.Prices.Add(new Period { Id = 2, To = lastPeriodEnd.AddHours(-5) }, new Price { Mid = 26.23m });
			history.Prices.Add(new Period { Id = 3, To = lastPeriodEnd.AddHours(-4) }, new Price { Mid = 28.23m });
			history.Prices.Add(new Period { Id = 4, To = lastPeriodEnd.AddHours(-3) }, new Price { Mid = 30.23m });
			history.Prices.Add(new Period { Id = 5, To = lastPeriodEnd.AddHours(-2) }, new Price { Mid = 30.25m });
			history.Prices.Add(new Period { Id = 6, To = lastPeriodEnd.AddHours(-1) }, new Price { Mid = 31.50m });
			history.Prices.Add(new Period { Id = 7, To = lastPeriodEnd.AddHours(0) }, new Price { Mid = 35.40m });

			var stockPriceProvider = new Mock<IStockHistoryDataProvider>();
			stockPriceProvider.Setup(x => x.GetStockHistory(stock, 5)).Returns(history);

			var filter = new MinimumRateOfChangeStockFilter(stockPriceProvider.Object, 5, 35m, 35m);

			var results = filter.GetInvestmentCandidates(new Stock[] { stock });

			Assert.AreEqual(0, results.Count());
		}

		[TestMethod]
		public void GetInvestmentCandidates_RateOfReductionInsufficient_ReturnsNothing()
		{
			var stock = new Stock
			{
				Id = "STK.PLC",
				Name = "STOCK PLC",
				Description = "The Test Stock Price"
			};

			var history = new StockPriceHistory
			{
				Stock = stock,
				Prices = new Dictionary<Period, Price>()
			};

			var currentDate = DateTime.UtcNow.AddMinutes(10);
			var lastPeriodEnd = DateTime.Parse(currentDate.ToString("dd MMMM yyyy HH:00:00"));

			history.Prices.Add(new Period { Id = 1, To = lastPeriodEnd.AddHours(-6) }, new Price { Mid = 25.23m });
			history.Prices.Add(new Period { Id = 2, To = lastPeriodEnd.AddHours(-5) }, new Price { Mid = 24.23m });
			history.Prices.Add(new Period { Id = 3, To = lastPeriodEnd.AddHours(-4) }, new Price { Mid = 23.23m });
			history.Prices.Add(new Period { Id = 4, To = lastPeriodEnd.AddHours(-3) }, new Price { Mid = 22.23m });
			history.Prices.Add(new Period { Id = 5, To = lastPeriodEnd.AddHours(-2) }, new Price { Mid = 21.25m });
			history.Prices.Add(new Period { Id = 6, To = lastPeriodEnd.AddHours(-1) }, new Price { Mid = 20.50m });
			history.Prices.Add(new Period { Id = 7, To = lastPeriodEnd.AddHours(0) }, new Price { Mid = 20.40m });

			var stockPriceProvider = new Mock<IStockHistoryDataProvider>();
			stockPriceProvider.Setup(x => x.GetStockHistory(stock, 5)).Returns(history);

			var filter = new MinimumRateOfChangeStockFilter(stockPriceProvider.Object, 5, 35m, 35m);

			var results = filter.GetInvestmentCandidates(new Stock[] { stock });

			Assert.AreEqual(0, results.Count());
		}

		[TestMethod]
		public void GetInvestmentCandidates_RateOfReductionSufficient_ReturnsStock()
		{
			var stock = new Stock
			{
				Id = "STK.PLC",
				Name = "STOCK PLC",
				Description = "The Test Stock Price"
			};

			var history = new StockPriceHistory
			{
				Stock = stock,
				Prices = new Dictionary<Period, Price>()
			};

			var currentDate = DateTime.UtcNow.AddMinutes(10);
			var lastPeriodEnd = DateTime.Parse(currentDate.ToString("dd MMMM yyyy HH:00:00"));

			history.Prices.Add(new Period { Id = 1, To = lastPeriodEnd.AddHours(-6) }, new Price { Mid = 25.23m });
			history.Prices.Add(new Period { Id = 2, To = lastPeriodEnd.AddHours(-5) }, new Price { Mid = 24.23m });
			history.Prices.Add(new Period { Id = 3, To = lastPeriodEnd.AddHours(-4) }, new Price { Mid = 23.23m });
			history.Prices.Add(new Period { Id = 4, To = lastPeriodEnd.AddHours(-3) }, new Price { Mid = 22.23m });
			history.Prices.Add(new Period { Id = 5, To = lastPeriodEnd.AddHours(-2) }, new Price { Mid = 21.25m });
			history.Prices.Add(new Period { Id = 6, To = lastPeriodEnd.AddHours(-1) }, new Price { Mid = 20.50m });
			history.Prices.Add(new Period { Id = 7, To = lastPeriodEnd.AddHours(0) }, new Price { Mid = 20.40m });

			var stockPriceProvider = new Mock<IStockHistoryDataProvider>();
			stockPriceProvider.Setup(x => x.GetStockHistory(stock, 5)).Returns(history);

			var filter = new MinimumRateOfChangeStockFilter(stockPriceProvider.Object, 5, 10m, 10m);

			var results = filter.GetInvestmentCandidates(new Stock[] { stock });

			Assert.AreEqual(1, results.Count());
		}

		[TestMethod]
		public void GetInvestmentCandidates_LatestPeriodMissing_ReturnsNothing()
		{
			var stock = new Stock
			{
				Id = "STK.PLC",
				Name = "STOCK PLC",
				Description = "The Test Stock Price"
			};

			var history = new StockPriceHistory
			{
				Stock = stock,
				Prices = new Dictionary<Period, Price>()
			};

			var currentDate = DateTime.UtcNow.AddMinutes(10);
			var lastPeriodEnd = DateTime.Parse(currentDate.ToString("dd MMMM yyyy HH:00:00"));

			history.Prices.Add(new Period { Id = 1, To = lastPeriodEnd.AddHours(-8) }, new Price { Mid = 45.23m });
			history.Prices.Add(new Period { Id = 2, To = lastPeriodEnd.AddHours(-7) }, new Price { Mid = 43.23m });
			history.Prices.Add(new Period { Id = 3, To = lastPeriodEnd.AddHours(-6) }, new Price { Mid = 30.23m });
			history.Prices.Add(new Period { Id = 4, To = lastPeriodEnd.AddHours(-5) }, new Price { Mid = 29.23m });
			history.Prices.Add(new Period { Id = 5, To = lastPeriodEnd.AddHours(-4) }, new Price { Mid = 27.25m });
			history.Prices.Add(new Period { Id = 6, To = lastPeriodEnd.AddHours(-3) }, new Price { Mid = 18.50m });
			history.Prices.Add(new Period { Id = 7, To = lastPeriodEnd.AddHours(-2) }, new Price { Mid = 13.40m });

			var stockPriceProvider = new Mock<IStockHistoryDataProvider>();
			stockPriceProvider.Setup(x => x.GetStockHistory(stock, 5)).Returns(history);

			var filter = new MinimumRateOfChangeStockFilter(stockPriceProvider.Object, 5, 25m, 25m);

			var results = filter.GetInvestmentCandidates(new Stock[] { stock });

			Assert.AreEqual(0, results.Count());

		}
		
		[TestMethod]
		public void GetInvestmentCandidates_NotEnoughtPeriods_ReturnsNothing()
		{
			var stock = new Stock
			{
				Id = "STK.PLC",
				Name = "STOCK PLC",
				Description = "The Test Stock Price"
			};

			var history = new StockPriceHistory
			{
				Stock = stock,
				Prices = new Dictionary<Period, Price>()
			};

			var currentDate = DateTime.UtcNow.AddMinutes(10);
			var lastPeriodEnd = DateTime.Parse(currentDate.ToString("dd MMMM yyyy HH:00:00"));

			history.Prices.Add(new Period { Id = 5, To = lastPeriodEnd.AddHours(-2) }, new Price { Mid = 27.25m });
			history.Prices.Add(new Period { Id = 6, To = lastPeriodEnd.AddHours(-1) }, new Price { Mid = 18.50m });
			history.Prices.Add(new Period { Id = 7, To = lastPeriodEnd.AddHours(0) }, new Price { Mid = 13.40m });

			var stockPriceProvider = new Mock<IStockHistoryDataProvider>();
			stockPriceProvider.Setup(x => x.GetStockHistory(stock, 5)).Returns(history);

			var filter = new MinimumRateOfChangeStockFilter(stockPriceProvider.Object, 5, 25m, 25m);

			var results = filter.GetInvestmentCandidates(new Stock[] { stock });

			Assert.AreEqual(0, results.Count());

		}
	}
}
