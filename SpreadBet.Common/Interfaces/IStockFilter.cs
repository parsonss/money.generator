// -----------------------------------------------------------------------
// <copyright file="IStockFilter.cs" company="">
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

	public interface IStockFilter
	{
		IEnumerable<Stock> GetInvestmentCandidates(IEnumerable<Stock> stocks);
	}
}
