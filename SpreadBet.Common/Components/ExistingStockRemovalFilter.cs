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
	public class ExistingStockRemovalFilter: IStockFilter
	{
		private readonly IPortfolioDataProvider _portfolioDataProvider;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConsistentChangeStockFilter"/> class.
		/// </summary>
		/// <param name="portfolioDataProvider">The portfolio data provider.</param>
		public ExistingStockRemovalFilter(IPortfolioDataProvider portfolioDataProvider)
		{
			Condition.Requires(portfolioDataProvider).IsNotNull();

			this._portfolioDataProvider = portfolioDataProvider;
		}

		/// <summary>
		/// Filters stock that have not shown a consistent rise or fall over the stated periods
		/// </summary>
		/// <param name="stocks">The stocks.</param>
		/// <returns></returns>
		public IEnumerable<Entities.Stock> GetInvestmentCandidates(IEnumerable<Entities.Stock> stocks)
		{
			var retVal = new List<Entities.Stock>();

			var existingStocks = this._portfolioDataProvider.GetExistingBets();

			Parallel.ForEach(stocks, stock =>
			{
				if (!existingStocks.Any(bet => bet.Stock.Id.Equals(stock.Id)))
				{
					retVal.Add(stock);
				}
			});

			return retVal;
		}
	}
}
