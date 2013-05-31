// -----------------------------------------------------------------------
// <copyright file="IStockHistoryDataProvider.cs" company="">
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

	public interface IStockHistoryDataProvider
	{
		StockPriceHistory GetStockHistory(Stock stock);
		StockPriceHistory GetStockHistory(Stock stock, int periods);
	}
}
