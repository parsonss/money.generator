// -----------------------------------------------------------------------
// <copyright file="ConsistantChangeStockFilter.cs" company="">
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

	/// <summary>
	/// TODO: Update summary.
	/// </summary>
	public class ConsistentChangeStockFilter: IStockFilter
	{
		private readonly IStockHistoryDataProvider _stockHistoryProvider;
		private readonly int _periods;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConsistentChangeStockFilter"/> class.
		/// </summary>
		/// <param name="stockHistoryProvider">The stock history provider.</param>
		/// <param name="periods">The periods used to reach a decision as to whether or not the stock is an investment candidate</param>
		public ConsistentChangeStockFilter(IStockHistoryDataProvider stockHistoryProvider, int periods)
		{
			Condition.Requires(stockHistoryProvider).IsNotNull();
			Condition.Requires(periods).IsGreaterThan(1);

			this._stockHistoryProvider = stockHistoryProvider;
			this._periods = periods;
		}

		/// <summary>
		/// Filters stock that have not shown a consistent rise or fall over the stated periods
		/// </summary>
		/// <param name="stocks">The stocks.</param>
		/// <returns></returns>
		public IEnumerable<Entities.Stock> GetInvestmentCandidates(IEnumerable<Entities.Stock> stocks)
		{
			var retVal = new List<Entities.Stock>();

			Func<Entities.Stock, bool> assessSuitability = (Entities.Stock stock) =>
			{
				var stockHistory = this._stockHistoryProvider.GetStockHistory(stock, this._periods);
				if ((stockHistory != null) && 
					(stockHistory.Prices.Count >= this._periods) &&								// Make sure we have the right number of periods
					stockHistory.Prices.Keys.Max(x => x.To) >= DateTime.UtcNow.AddHours(-1) )	// The latest price cannot be more than 1 hour old
				{
					var trend = stockHistory.Prices
									.OrderByDescending(x => x.Key.Id)		// Order the prices in descending chronological order
									.Take(this._periods)					// Take the latest required number of periods
									.Reverse()								// Back to chronological order
									.Select(price => new
									{
										PeriodId = price.Key.Id,
										MidPrice = price.Value.Mid
									}).ToList();

					// Now make sure we have consecutive periods
					if ((trend.Max(x => x.PeriodId) - trend.Min(x => x.PeriodId)) + 1 == this._periods)
					{
						int? direction = null;
						decimal? previousPrice = null;
						bool suitable = true;

						foreach (var price in trend.Select(x => x.MidPrice))
						{
							if (previousPrice != null)
							{
								if (price < previousPrice)
								{
									suitable = (direction == null) || (direction == -1);
									direction = -1;
								}
								else if (price >= previousPrice)
								{
									suitable = (direction == null) || (direction == 1);
									direction = 1;
								}
							}

							if (!suitable) break;
							previousPrice = price;
						}

						return suitable;
					}
				}

				return false;
			};

			Parallel.ForEach(stocks, stock =>
			{
				if (assessSuitability(stock)) retVal.Add(stock);
			});

			return retVal;
		}
	}
}
