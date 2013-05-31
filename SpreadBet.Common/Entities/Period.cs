// -----------------------------------------------------------------------
// <copyright file="Period.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SpreadBet.Common.Entities
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class Period
	{
		public int Id { get; set; }
		public DateTime From { get; set; }
		public DateTime To { get; set; }
	}
}
