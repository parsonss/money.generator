// -----------------------------------------------------------------------
// <copyright file="StockPrice.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SpreadBet.Common.Entities
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class StockPrice
	{
		public int PeriodId { get; set; }
		public Stock Stock { get; set; }
		public Price Price { get; set; }
		public DateTime UpdatedAt { get; set; }
	}
}
