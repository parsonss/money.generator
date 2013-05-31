// -----------------------------------------------------------------------
// <copyright file="ScrapeInfo.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SpreadBet.Common.Entities
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Net;

	/// <summary>
	/// Wraps the response from the request to scrape an intividual page
	/// </summary>
	public class ScrapeInfo
	{
		/// <summary>
		/// The html content retrieved from the requested url
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// The content type of the response 
		/// </summary>
		public string ContentType { get; set; }

		/// <summary>
		/// Where redirections took place this is the url of the final page
		/// </summary>
		public string FinalUrl { get; set; }

		/// <summary>
		/// The HTTP Response code 
		/// </summary>
		public HttpStatusCode ResponseCode { get; set; }

		/// <summary>
		/// True when the request was completed successfully; otherwise false
		/// </summary>
		public bool Success { get; set; }
	}
}
