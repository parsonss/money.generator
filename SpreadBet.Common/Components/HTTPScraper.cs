// -----------------------------------------------------------------------
// <copyright file="HTTPScraper.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SpreadBet.Common.Components
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Net;
	using System.IO;
	using System.Reflection;
	using System.Net.Configuration;
	using SpreadBet.Common.Interfaces;
	using SpreadBet.Common.Entities;

	public class DefaultScraper : IScraper
	{
		private const string __USERAGENT = "Mozilla/5.0 (X11; U; Linux x86_64; en-US) AppleWebKit/534.3 (KHTML, like Gecko) Chrome/6.0.472.63 Safari/534.3";

		private CookieContainer _cookieContainer = null;
		private long _proxyIndex = 0;

		public DefaultScraper()
		{
			this._cookieContainer = new CookieContainer();
		}

		public ScrapeInfo Scrape(string url)
		{
			return Scrape(url, "GET", null);
		}

		public ScrapeInfo Scrape(string url, IEnumerable<KeyValuePair<string, string>> postData)
		{
			return Scrape(url, "POST", postData);
		}

		private ScrapeInfo Scrape(string url, string method, IEnumerable<KeyValuePair<string, string>> postData)
		{
			var retVal = new ScrapeInfo();

			HttpWebResponse objResponse = null;

			try
			{
				if (method.ToUpper() == "GET")
					objResponse = this.PerformGet(url);
				else if (method.ToUpper() == "POST")
					objResponse = this.PerformPost(url, postData);

				if (objResponse != null)
				{
					// Try to fetch the page from the given URL, in case of any error return null string
					using (objResponse)
					{
						// In case of page not found error, return null string
						if (objResponse.StatusCode != HttpStatusCode.NotFound)
						{
							// If there is a proper response then read the contents in the response and return these contents
							using (var sr = new StreamReader(objResponse.GetResponseStream()))
							{
								retVal.Content = sr.ReadToEnd();
								sr.Close();
							}
						}

						retVal.ContentType = objResponse.ContentType;
						retVal.FinalUrl = objResponse.ResponseUri.ToString();
						retVal.ResponseCode = objResponse.StatusCode;
						retVal.Success = objResponse.StatusCode == HttpStatusCode.OK;
					}
				}
			}
			catch (WebException wex)
			{
				if (wex.Status == WebExceptionStatus.ProtocolError)
				{
					// Try to fetch the page from the given URL, in case of any error return null string
					using (objResponse = wex.Response as HttpWebResponse)
					{
						if (objResponse != null)
						{
							retVal.Success = false;
							retVal.ResponseCode = objResponse.StatusCode;

							try
							{
								// Although the response code indicates some problem there may still be some content.
								using (var sr = new StreamReader(objResponse.GetResponseStream()))
								{
									retVal.Content = sr.ReadToEnd();
									sr.Close();
								}
							}
							catch (Exception) { }
						}
					}
				}
				else
					retVal = null;
			}
			catch
			{
				retVal = null;
			}

			return retVal;
		}

		private HttpWebResponse PerformGet(string url)
		{
			HttpWebResponse retVal = null;

			try
			{

				ToggleAllowUnsafeHeaderParsing(true);

				var httpRequest = (HttpWebRequest)WebRequest.Create(url);

				httpRequest.UserAgent = __USERAGENT;

				httpRequest.CookieContainer = this._cookieContainer;
				httpRequest.AllowAutoRedirect = true;
				httpRequest.MaximumAutomaticRedirections = 5;
				httpRequest.Accept = "application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";
				httpRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 5.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
				httpRequest.Timeout = 300000;
				httpRequest.KeepAlive = false;

				retVal = (HttpWebResponse)httpRequest.GetResponse();
			}
			catch (WebException wex)
			{
				retVal = (HttpWebResponse)wex.Response;
			}
			catch (Exception ex)
			{
				retVal = null;
			}
			finally
			{
				ToggleAllowUnsafeHeaderParsing(false);
			}

			return retVal;
		}

		public static bool ToggleAllowUnsafeHeaderParsing(bool enable)
		{
			//Get the assembly that contains the internal class
			Assembly assembly = Assembly.GetAssembly(typeof(SettingsSection));
			if (assembly != null)
			{
				//Use the assembly in order to get the internal type for the internal class
				Type settingsSectionType = assembly.GetType("System.Net.Configuration.SettingsSectionInternal");
				if (settingsSectionType != null)
				{
					//Use the internal static property to get an instance of the internal settings class.
					//If the static instance isn't created already invoking the property will create it for us.
					object anInstance = settingsSectionType.InvokeMember("Section",
					BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.NonPublic, null, null, new object[] { });
					if (anInstance != null)
					{
						//Locate the private bool field that tells the framework if unsafe header parsing is allowed
						FieldInfo aUseUnsafeHeaderParsing = settingsSectionType.GetField("useUnsafeHeaderParsing", BindingFlags.NonPublic | BindingFlags.Instance);
						if (aUseUnsafeHeaderParsing != null)
						{
							aUseUnsafeHeaderParsing.SetValue(anInstance, enable);
							return true;
						}

					}
				}
			}
			return false;
		}

		private HttpWebResponse PerformPost(string url, IEnumerable<KeyValuePair<string, string>> postData)
		{
			HttpWebResponse retVal = null;

			try
			{
				// Post form data		
				var httpRequest = (HttpWebRequest)WebRequest.Create(url);
				httpRequest.CookieContainer = this._cookieContainer;
				httpRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 5.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
				httpRequest.ContentType = "application/x-www-form-urlencoded";
				httpRequest.Method = "POST";
				httpRequest.Accept = "application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";
				httpRequest.AllowAutoRedirect = true;

				if ((postData != null) && (postData.Count() != 0))
				{
					string postString = string.Empty;
					foreach (var pair in postData)
					{
						postString += string.Format("{0}={1}", pair.Key, pair.Value) + "&";
					}
					postString = postString.TrimEnd(new char[] { '&' });

					byte[] bytes = Encoding.ASCII.GetBytes(postString);
					httpRequest.ContentLength = bytes.Length;
					using (Stream os = httpRequest.GetRequestStream())
					{
						os.Write(bytes, 0, bytes.Length);
					}
				}

				// Get the response that should include set_cookie if we have been successful.
				retVal = (HttpWebResponse)httpRequest.GetResponse();
			}
			catch (WebException wex)
			{
				retVal = (HttpWebResponse)wex.Response;
			}
			catch
			{
				retVal = null;
			}

			return retVal;
		}
	}
}
