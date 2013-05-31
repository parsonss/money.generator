// -----------------------------------------------------------------------
// <copyright file="CompositeStockFilter.cs" company="">
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

	public class CompositeStockFilter: IStockFilter
	{
		private readonly List<IStockFilter> _filters;

		public CompositeStockFilter(params IStockFilter[] filters)
		{
			Condition.Requires(filters)
				.IsNotNull()
				.IsNotEmpty();

			this._filters = new List<IStockFilter>(filters);
		}
		
		public IEnumerable<Entities.Stock> GetInvestmentCandidates(IEnumerable<Entities.Stock> stocks)
		{
			var stockList = new List<Entities.Stock>(stocks);

			this._filters.ForEach(x =>
			{
				var filteredStockList = x.GetInvestmentCandidates(stockList);
				stockList = new List<Entities.Stock>(filteredStockList);
			});

			return stockList;
		}
	}
}
