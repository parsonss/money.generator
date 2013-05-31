// -----------------------------------------------------------------------
// <copyright file="InvestmentDecisionByGrowthRate.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SpreadBet.Common.Components
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using SpreadBet.Common.Interfaces;
	using CuttingEdge.Conditions;
	using System.Threading.Tasks;
	using SpreadBet.Common.Entities;

	/// <summary>
	/// TODO: Update summary.
	/// </summary>
	public class DecisionByGrowthAndCurrentPosition: IInvestDecider
	{
		private readonly IStockHistoryDataProvider _stockHistoryProvider;
		private readonly IAccountDataProvider _accountDataProvider;
		private readonly int _periods;
		private readonly decimal _maxBidAmount;
		private readonly decimal _maxSpreadLoss;

		/// <summary>
		/// Initializes a new instance of the <see cref="DecisionByGrowthAndCurrentPosition"/> class.
		/// </summary>
		/// <param name="stockHistoryDataProvider">The stock history data provider.</param>
		/// <param name="accountDataProvider">The account data provider.</param>
		/// <param name="periods">The periods number of periods over which to measure stock growth</param>
		/// <param name="bidAmount">The bid amount.</param>
		/// <param name="maxSpreadLoss">The maximum amount of money we are willing to loose when opening our position based on the stocks spread</param>
		public DecisionByGrowthAndCurrentPosition(
			IStockHistoryDataProvider stockHistoryDataProvider, 
			IAccountDataProvider accountDataProvider, 
			int periods,
			decimal maxBidAmount, 
			decimal maxSpreadLoss)
		{
			Condition.Requires(stockHistoryDataProvider).IsNotNull();
			Condition.Requires(_accountDataProvider).IsNotNull();
			Condition.Requires(periods).IsGreaterThan(0);
			Condition.Requires(maxBidAmount).IsGreaterThan(0);
			Condition.Requires(maxSpreadLoss).IsGreaterThan(0);

			this._stockHistoryProvider = stockHistoryDataProvider;
			this._accountDataProvider = accountDataProvider;
			this._periods = periods;
			this._maxBidAmount = maxBidAmount;
			this._maxSpreadLoss = maxSpreadLoss;
		}

		public IEnumerable<Entities.Bet> GetInvestmentDescisions(IEnumerable<Entities.Stock> stocks)
		{
			var analysis = new List<StockAnalysis>();

			Action<Entities.Stock> performAnalysis = (Stock stock) =>
			{
				var stockHistory = this._stockHistoryProvider.GetStockHistory(stock, this._periods);
				if ((stockHistory != null) &&
					(stockHistory.Prices.Count >= this._periods) &&								// Make sure we have the right number of periods
					stockHistory.Prices.Keys.Max(x => x.To) >= DateTime.UtcNow.AddHours(-1))	// The latest price cannot be more than 1 hour old
				{
					var prices = stockHistory.Prices
									.OrderByDescending(x => x.Key.Id)		// Order the prices in descending chronological order
									.Take(this._periods)					// Take the latest required number of periods
									.Reverse()								// Back to chronological order
									.Select(x => x.Value).ToList();
					
					// Now ensure that on average the change is over a certain amount
					var rateOfChange = ((prices.Last().Mid - prices.First().Mid) / prices.First().Mid) * 100;

					analysis.Add(new StockAnalysis
					{
						Stock = stock, 
						Prices = prices
						RateOfChange = rateOfChange
					});
				}
			};

			Parallel.ForEach(stocks, stock =>
			{
				performAnalysis(stock);
			});

			// We now have the list of stocks and their rate of change.
			analysis
				.OrderByDescending(x => x.RateOfChange)
				.ToList()
				.ForEach(x =>
			{
				var spread = Math.Abs(x.Prices.Last().Bid - x.Prices.Last().Offer);

				// calculate the maximum bet based on the maximum spread loss
				var maxInitialBet = Math.Round(this._maxSpreadLoss / spread, 0, MidpointRounding.AwayFromZero);
				var bidAmount = Math.Min(maxInitialBet, _maxBidAmount);

				var initialLoss = spread * bidAmount;

				var bet = new Bet
				{
					BidAmount = bidAmount, 
					Direction = (x.RateOfChange < 0) ? Direction.Decrease : Direction.Increase,
					OpeningPosition = (x.RateOfChange < 0) ? x.Prices.Last().Offer : x.Prices.Last().Bid,
				};
			});
		}

		private class StockAnalysis
		{
			public Stock Stock { get; set; }
			public IEnumerable<Price> Prices { get; set; }
			public decimal RateOfChange { get; set; }
		}
	}
}
