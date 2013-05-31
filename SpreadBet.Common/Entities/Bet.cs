// -----------------------------------------------------------------------
// <copyright file="Bet.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SpreadBet.Common.Entities
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	/// <summary>
	/// TODO: Update summary.
	/// </summary>
	public class Bet
	{
		public Stock Stock { get; set; }

		public decimal InitialLoss { get; set; }
		public decimal BidAmount { get; set; }
		public decimal OpeningPosition { get; set; }
		public Direction Direction { get; set; }
	}

	public enum Direction
	{
		Increase, 
		Decrease
	}
}

