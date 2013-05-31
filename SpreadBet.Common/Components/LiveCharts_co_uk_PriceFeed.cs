// -----------------------------------------------------------------------
// <copyright file="LondonStockExchangePriceFeed.cs" company="">
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
	using System.Text.RegularExpressions;
	using SpreadBet.Common.Entities;
	using SpreadBet.Common.Helpers;
	using System.Threading.Tasks;
	using CuttingEdge.Conditions;

	/// <summary>
	/// TODO: Update summary.
	/// </summary>
	public class LiveCharts_co_uk_PriceFeed : IStockPriceProvider
	{
		private readonly List<string> _listUrls;
		private readonly IScraper _scraper;

		public LiveCharts_co_uk_PriceFeed(IScraper scraper)
		{
			Condition.Requires(scraper).IsNotNull();

			this._scraper = scraper;
				
			#region Set starting urls

			this._listUrls = new string[]
			{
				"http://www.livecharts.co.uk/share_map.php?letter=0-9", 
				"http://www.livecharts.co.uk/share_map.php?letter=a", 
				"http://www.livecharts.co.uk/share_map.php?letter=b", 
				"http://www.livecharts.co.uk/share_map.php?letter=c", 
				"http://www.livecharts.co.uk/share_map.php?letter=d", 
				"http://www.livecharts.co.uk/share_map.php?letter=e", 
				"http://www.livecharts.co.uk/share_map.php?letter=f", 
				"http://www.livecharts.co.uk/share_map.php?letter=g", 
				"http://www.livecharts.co.uk/share_map.php?letter=h", 
				"http://www.livecharts.co.uk/share_map.php?letter=i", 
				"http://www.livecharts.co.uk/share_map.php?letter=j", 
				"http://www.livecharts.co.uk/share_map.php?letter=k", 
				"http://www.livecharts.co.uk/share_map.php?letter=l", 
				"http://www.livecharts.co.uk/share_map.php?letter=m", 
				"http://www.livecharts.co.uk/share_map.php?letter=n", 
				"http://www.livecharts.co.uk/share_map.php?letter=o", 
				"http://www.livecharts.co.uk/share_map.php?letter=p", 
				"http://www.livecharts.co.uk/share_map.php?letter=q", 
				"http://www.livecharts.co.uk/share_map.php?letter=r", 
				"http://www.livecharts.co.uk/share_map.php?letter=s",
				"http://www.livecharts.co.uk/share_map.php?letter=t", 
				"http://www.livecharts.co.uk/share_map.php?letter=u", 
				"http://www.livecharts.co.uk/share_map.php?letter=v", 
				"http://www.livecharts.co.uk/share_map.php?letter=w", 
				"http://www.livecharts.co.uk/share_map.php?letter=x",
				"http://www.livecharts.co.uk/share_map.php?letter=y",
				"http://www.livecharts.co.uk/share_map.php?letter=z" 
			}.ToList<string>();

			#endregion
		}

		public List<Entities.StockPrice> GetStockPrices()
		{
			var timePeriod = TimePeriodHelper.GetTimePeriod(DateTime.UtcNow);

			var stockPrices = new List<StockPrice>();

			Parallel.ForEach<string>(GetPageUrls(), url =>
			{
				var stock = ReadStockPrices(url, timePeriod);

				if (stock != null)
				{
					lock (stockPrices)
					{
						stockPrices.Add(stock);
					}
				}
			});

			return stockPrices;
		}

		private List<string> GetPageUrls()
		{
			var retVal = new List<string>();

			Parallel.ForEach(this._listUrls, url => 
			{
				var response = this._scraper.Scrape(url);
				if (response.Success)
				{
					retVal.AddRange(
						Regex.Matches(response.Content, "(?ismx)<a[^>]*?href\\s?=\\s?[\\\"\\'](?<url>share_prices/[^\\\"\\']*)")
						.OfType<Match>()
						.Select(match => match.Groups["url"].Value.Trim()));
				}
			});

			return retVal;
		}

		private StockPrice ReadStockPrices(string url, int timePeriod)
		{
			StockPrice retVal = null;

			var response = this._scraper.Scrape(url);
			if (response.Success)
			{
				retVal = this.ExtractStockData(response.Content, timePeriod);
			}

			return retVal;
		}

		private StockPrice ExtractStockData(string content, int timePeriod)
		{
			var nameExp = "(?ismx)" + 
						  "<h2[^>]*?class\\s?=\\s?[\\\"\\']title[\\\"\\'][^>]*?>[^<]*?" + 
						  "(?:<a[^>]*?>)?" + 
						  "\\s*(?<val>[^<]*?)\\s*<";

			var name = Regex.Match(content, nameExp).Groups["val"].Value;

			var symExp = "(?ismx)" + 
						 "symbol[^<]*?" + 
						 "<span[^>]*?class\\s?=\\s?[\\\"\\']shares_one[^>]*?>\\s*" + 
						 "(?:<a[^>]*?>[^<]*?)?" + 
						 "(?<val>[^<]*?)\\s*<";

			var symb = Regex.Match(content, symExp).Groups["val"].Value;

			var midExp = "(?ismx)" +
						 "share\\sprice[^<]*?" +
						 "<span[^>]*?class\\s?=\\s?[\\\"\\']shares_one[^>]*?>\\s*" +
						 "(?<val>[\\d\\,\\.]+)";

			var mid = Regex.Match(content, midExp).Groups["val"].Value;

			var valExp = "(?ismx)" +
						 "<th[^>]*?>[^<]*?security[^<]*?</\\s?th>[^<]*?" + 
						 "<td[^>]*?>\\s*(?:<a[^>]*?>)?(?<value>.*?)\\s*</\\s?(?:a|td)>";

			var bid = Regex.Match(content, string.Format(valExp, "bid")).Groups["val"].Value;
			var offer = Regex.Match(content, string.Format(valExp, "offer")).Groups["val"].Value;
			var security = Regex.Match(content, string.Format(valExp, "security")).Groups["val"].Value;

			return new StockPrice
			{
				PeriodId = timePeriod,
				UpdatedAt = DateTime.UtcNow,
				Stock = new Stock
				{
					Id = symb,
					Name = name, 
					Security = security
				},
				Price = new Price
				{
					Mid = decimal.Parse(mid), 
					Bid = decimal.Parse(bid), 
					Offer = decimal.Parse(offer), 
					UpdatedAt = DateTime.Now
				}
			};
		}
	}
}