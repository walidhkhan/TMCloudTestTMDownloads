using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using TMCloud.Mqx;
using TMCloud.Mqx.Client;
using TMCloud.Mqx.Core;


namespace TestTMDownloads.Models.Mqx
{
	public static class MqxHelper
	{
		static Logger _logger = LogManager.GetCurrentClassLogger();

		public static IMqxService GetService()
		{
			string wcfMqxServer = Properties.Settings.Default.WcfMqxServer;

			if (string.IsNullOrEmpty(wcfMqxServer))
			{
				// No WCF service specified, so using internal code
				return new MqxService();
			}
			else
			{
				// using WCF service
				IMqxLogger mqxLogger = new MqxLogger();
				return new MqxServerService(wcfMqxServer, mqxLogger);
			}
		}

		public static IMqxService GetService(IHttpRequester httpRequester, IMqxLogger mqxLogger)
		{
			string wcfMqxServer = Properties.Settings.Default.WcfMqxServer;

			if (string.IsNullOrEmpty(wcfMqxServer))
			{
				// No WCF service specified, so using internal code
				return new MqxService(httpRequester, mqxLogger);
			}
			else
			{
				// using WCF service
				return new MqxServerService(wcfMqxServer, mqxLogger, httpRequester);
			}
		}

		/// <summary>
		/// Indicates if a download from the jurisdiction requires a CAPTCHA to be solved.
		/// If so, then the Start.../Resume... interface needs to be used
		/// </summary>
		/// <param name="countryID"></param>
		/// <returns></returns>
		public static bool RequiresCaptcha(int countryID)
		{
			// 641 - ZQ - South Africe no longer needs CAPTCHA

			return false;
		}

		/// <summary>
		/// Logs the error message and returns a more user friendly version 
		/// </summary>
		/// <param name="errorMessage">Original mqx error message</param>
		/// <returns>User friendly error message</returns>
		public static string HandleErrorMessage(string errorMessage)
		{
			// log the error
			_logger.Error("MQX error: {0}", errorMessage);

			// if it looks like a ChromeDriver issue, then send an email to support
			if (IsChromeDriverProblem(errorMessage))
			{
				string from = Globals.EMAIL_HELP;
				string body = errorMessage;
				string subject = "MQX ChromeDriver issue";
				string to = ConfigurationManager.AppSettings["ErrorEmail"];
				string cc = "";
				string bcc = "";
				System.Net.Mail.MailMessage email = Globals.GenerateEmail(to, cc, bcc, subject, body, from);
				bool emailSent = Globals.SendEmail(email);
			}

			return CleanErrorMessage(errorMessage);
		}

		static string CleanErrorMessage(string errorMessage)
		{
			if (IsChromeDriverProblem(errorMessage))
			{
				//return string.Format(Resources.Imports.Imports.MqxErrorChromeDriver, errorMessage);
			}
			else
			{
				//return string.Format(Resources.Imports.Imports.MqxErrorGeneric, errorMessage);
			}
            return "";
		}

		static bool IsChromeDriverProblem(string errorMessage)
		{
			return errorMessage.ToLower().Contains("chromedriver");
		}

		public static byte[] DownloadDocument(string docUrl, out string extension)
		{
			HttpRequester httpRequester = new HttpRequester(null);
			HttpRequestData httpRequestData = new HttpRequestData();
			httpRequestData.SetRequest(docUrl, false);

			int maxAttempts = 1;
			byte[] docData;
			//string contentType = "application/pdf";
			// set default extension type
			extension = ".pdf";

			try
			{
				bool tryToDecode = false; // expecting binary data, so no need to try and decode text
				HttpResponseData httpResponseData = httpRequester.Request(httpRequestData, "not_needed", tryToDecode, maxAttempts);
				docData = httpResponseData.BinaryResponse;
				if (!string.IsNullOrEmpty(httpResponseData.ContentType))
				{
					string contentType = httpResponseData.ContentType;
					int idx = contentType.LastIndexOf('/');
					if (idx >= 0)
					{
						extension = "." + contentType.Substring(idx + 1);
						// make sure there is nothing after the extension
						idx = extension.IndexOf(';');
						if (idx >= 0)
						{
							extension = extension.Substring(0, idx);
						}
					}
				}
			}
			catch (Exception)
			{
				docData = null;
			}

			return docData;
		}

		/// <summary>
		/// Attempts to produce a standadised representation of an application number
		/// so we can compare application numbers from different sources
		/// </summary>
		/// <param name="countryId"></param>
		/// <param name="appNum"></param>
		/// <returns></returns>
		public static string NormalizeAppNum(int countryId, string appNum)
		{
			switch (countryId)
			{
				case 483: // BX - Benelux
					// It should be safe to make this the default action for all jurisdictions
					return appNum.TrimStart('0');

				case 509: // CR - Costa Rica
						  // this can have leading zeros on the second part of the number like: "2018-0000288"
					return Regex.Replace(appNum, "-0*", "-");

				default:
					// assume only the 1 format
					// should be safe to always remove leading zeros here - if it helps
					return appNum;
			}
		}

		/// <summary>
		/// Create MQX search criteria just to find by application number
		/// </summary>
		/// <param name="appNum"></param>
		/// <returns></returns>
		public static SearchCriteria AppNumToCrit(string appNum)
		{
			SearchCriteria crit = new SearchCriteria();

			crit.Add(new SearchCriteriaField(SearchCriteriaField.SearchField.SF_APPNUM, SearchCriteriaField.SearchOp.SO_EQUALS, appNum));

			return crit;
		}

		/// <summary>
		/// Get the mark(s) corresponding to the given appNum (or regNum if appNum failed to return results)
		/// </summary>
		/// <param name="countryID"></param>
		/// <param name="appNum"></param>
		/// <param name="regNum"></param>
		/// <param name="searchFieldUsed">Indicates field used to return results, or SF_NULL if unable to return results</param>
		/// <returns></returns>
		public static GetMarksResult GetMarks(int countryID, string appNum, string regNum, out SearchCriteriaField.SearchField searchFieldUsed)
		{
			searchFieldUsed = SearchCriteriaField.SearchField.SF_NULL;
			GetMarksResult markResults = null;
			IMqxService mqxService = GetService();

			string cleanAppNum = CleanNumForPto(countryID, appNum);
			if (string.IsNullOrEmpty(cleanAppNum) && !HasRegNumSearch(countryID))
			{
				// we assume that the appNum has ended up in the regNum field
				cleanAppNum = CleanNumForPto(countryID, regNum);
			}

			if (!string.IsNullOrEmpty(cleanAppNum))
			{
				SearchCriteria crit = new SearchCriteria();
				crit.Add(new SearchCriteriaField(SearchCriteriaField.SearchField.SF_APPNUM, SearchCriteriaField.SearchOp.SO_EQUALS, cleanAppNum));
				markResults = mqxService.GetMarks(countryID, crit);
			}

			if (markResults != null && markResults.Success && markResults.Hits.Count > 0)
			{
				// search by application number seemed to have been successfull
				searchFieldUsed = SearchCriteriaField.SearchField.SF_APPNUM;
			}
			else if (HasRegNumSearch(countryID))
			{
				string cleanRegNum = CleanNumForPto(countryID, regNum);
				if (!string.IsNullOrEmpty(cleanRegNum))
				{
					SearchCriteria crit = new SearchCriteria();
					crit.Add(new SearchCriteriaField(SearchCriteriaField.SearchField.SF_REGNUM, SearchCriteriaField.SearchOp.SO_EQUALS, cleanRegNum));
					markResults = mqxService.GetMarks(countryID, crit);
					if (markResults != null && markResults.Success && markResults.Hits.Count > 0)
					{
						// search by registration number seemed to have been successfull
						searchFieldUsed = SearchCriteriaField.SearchField.SF_REGNUM;
					}
				}
			}
			
			return markResults;
		}

		static bool HasRegNumSearch(int countryID)
		{
			// TODO: There are probably others that need adding to this list
			switch (countryID)
			{
				case 473: // Australia
				case 608: // EUCTM
				case 602: // New Zealand
					return false;

				default:
					return true;
			}
		}

		static string CleanNumForPto(int countryID, string num)
		{
			// TODO: does this need to be application or registration number specific?

			if (num == null)
			{
				// nothing to clean
				return num;
			}

			string cleanNum = num.Trim();

			// logic copied from SettingsController.GetTrademarkOfficeMark()
			switch (countryID)
			{
				case 474:   // Austria
				case 553:   // Iceland
				case 649:   // Switzerland
				case 559:   // Ireland
				case 658:   // Tunisia
					cleanNum = Regex.Replace(cleanNum, @"[^a-zA-Z0-9\/]", string.Empty);
					break;

				case 483:   // Benelux
					cleanNum = cleanNum.TrimStart('0');
					cleanNum = Regex.Replace(cleanNum, @"[^a-zA-Z0-9]", string.Empty);
					break;

				case 505:   // Colombia
					break;

				case 509:   // Costa Rica
				case 571:   // Latvia
					cleanNum = Regex.Replace(cleanNum, @"[^-a-zA-Z0-9]", string.Empty);
					break;

				case 577:   // Lithuania
					cleanNum = Regex.Replace(cleanNum, @"[^ -a-zA-Z0-9]", string.Empty);
					break;

				default:
					cleanNum = Regex.Replace(cleanNum, @"[^a-zA-Z0-9]", string.Empty);
					break;
			}

			return cleanNum;
		}

		/// <summary>
		/// Merge a single string field from src to dest
		/// </summary>
		/// <param name="src"></param>
		/// <param name="srcFieldName"></param>
		/// <param name="dest"></param>
		/// <param name="destFieldName">if null, uses srcFieldName</param>
		/// <param name="overwriteIfAlreadyExists"></param>
		public static void MergePtoMarkValue(PtoMark src, string srcFieldName, PtoMark dest, string destFieldName, bool overwriteIfAlreadyExists)
		{
			if (destFieldName == null)
			{
				destFieldName = srcFieldName;
			}

			string value = src.StringFieldValue(srcFieldName);
			MergePtoMarkValue(value, dest, destFieldName, overwriteIfAlreadyExists);
		}

		/// <summary>
		/// Merge value into dest
		/// </summary>
		/// <param name="value"></param>
		/// <param name="dest"></param>
		/// <param name="destFieldName"></param>
		/// <param name="overwriteIfAlreadyExists"></param>
		public static void MergePtoMarkValue(string value, PtoMark dest, string destFieldName, bool overwriteIfAlreadyExists)
		{
			// This is where it would have made more sense if the string fields were a dictionary and not a list!

			// null means it doesn't exist, "" means it exists but has am empty value
			if (value != null)
			{
				foreach (StringPair pair in dest.NameValuePairs)
				{
					if (String.Compare(pair.First, destFieldName, true) == 0)
					{
						// already exists
						if (overwriteIfAlreadyExists)
						{
							pair.Second = value;
						}
						return;
					}
				}

				// doesn't already exist
				dest.AddStringField(destFieldName, value);
			}
		}

		/// <summary>
		/// Convert a returned EPO details click-through url
		/// to a click-through url for the legal status page
		/// </summary>
		/// <param name="epoDetailsUrl"></param>
		/// <returns></returns>
		public static string EpoUrlToLegaStatusUrl(string epoDetailsUrl)
		{
			// convert "https://register.epo.org/application?number=EP20161502"
			// to      "https://register.epo.org/application?number=EP20161502&lng=en&tab=legal"
			return epoDetailsUrl + "&lng=en&tab=legal";
		}

		/// <summary>
		/// Convert a returned EPO details click-through url
		/// to a click-through url for the citations page
		/// </summary>
		/// <param name="epoDetailsUrl"></param>
		/// <returns></returns>
		public static string EpoUrlToCitationsUrl(string epoDetailsUrl)
		{
			// convert "https://register.epo.org/application?number=EP20161502"
			// to      "https://register.epo.org/application?number=EP20161502&lng=en&tab=citations"
			return epoDetailsUrl + "&lng=en&tab=citations";
		}

		/// <summary>
		/// Convert a returned EPO details click-through url
		/// to a click-through url for the patent family page
		/// </summary>
		/// <param name="epoDetailsUrl"></param>
		/// <returns></returns>
		public static string EpoUrlToPatentFamilyUrl(string epoDetailsUrl)
		{
			// convert "https://register.epo.org/application?number=EP20161502"
			// to      "https://register.epo.org/application?number=EP20161502&lng=en&tab=family"
			return epoDetailsUrl + "&lng=en&tab=family";
		}

		/// <summary>
		/// Convert a returned EPO details click-through url
		/// to a click-through url for the all documents page
		/// </summary>
		/// <param name="epoDetailsUrl"></param>
		/// <returns></returns>
		public static string EpoUrlToAllDocumentsUrl(string epoDetailsUrl)
		{
			// convert "https://register.epo.org/application?number=EP20161502"
			// to      "https://register.epo.org/application?number=EP20161502&lng=en&tab=doclist"
			return epoDetailsUrl + "&lng=en&tab=doclist";
		}
	}
}