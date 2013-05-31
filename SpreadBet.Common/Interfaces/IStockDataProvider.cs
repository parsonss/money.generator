// -----------------------------------------------------------------------
// <copyright file="IStockDataProvider.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SpreadBet.Common.Interfaces
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using SpreadBet.Common.Entities;

	/// <summary>
	/// TODO: Update summary.
	/// </summary>
	public interface IStockDataProvider
	{
		void SaveStockData(IEnumerable<StockPrice> stockPrice);
	}
}
