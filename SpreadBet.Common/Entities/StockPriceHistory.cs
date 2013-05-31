// -----------------------------------------------------------------------
// <copyright file="StockPriceHistory.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SpreadBet.Common.Entities
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class StockPriceHistory
	{
		public Stock Stock { get; set; }
		public Dictionary<Period, Price> Prices { get; set; }
	}
}
