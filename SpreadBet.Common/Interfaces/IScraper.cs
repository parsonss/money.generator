// -----------------------------------------------------------------------
// <copyright file="IScraper.cs" company="">
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

	public interface IScraper
	{
		ScrapeInfo Scrape(string url);
		ScrapeInfo Scrape(string url, IEnumerable<KeyValuePair<string, string>> postData);
	}
}
