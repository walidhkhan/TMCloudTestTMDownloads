using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMCloud.Mqx;
using TMCloud.Mqx.Core;
using TMCloud.Mqx.Proxy;
using TestTMDownloads.Models.Mqx;
using System.Net;
using System.Net.Mail;
using System.Configuration;
 



namespace TestTMDownloads
{
    class Program
    {
        static Logger _logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            IMqxService mqxService = MqxHelper.GetService();

            _logger.Info("******************************************************");
            //_logger.Info("Processing total jurisdictions where TM Downloads Testing occured: " + mqxService.GetNumHits.Count());


            string subject = "";
            string body = "";
            // United States of America TM Download Testing
            int countryID = 668;
            string appNums = "77111111";
            string regNums = "";
            string countryName = "United States of America";
            SearchCriteria searchCriteria = GetSearchCriteria(countryID, appNums, regNums);
            //GetNumHitsResult result = mqxService.GetNumHits(countryID, searchCriteria);
            GetMarksResult marksResult = mqxService.GetMarks(countryID, searchCriteria);
            if (!marksResult.Success)
            {
                body += $"<p style='color: red'><b>An error occured during the testing for {countryName} trademark downloads using the sample application number: {appNums}.</b></p>";
            }
            else if (marksResult.Success)
            {
                body += $"<p>The trademark downloads testing for {countryName} has successfully passed, using the sample application number: {appNums}.</p>";
            }

            // Canada TM Download Testing
            countryID = 498;
            appNums = "1210057";
            regNums = "";
            countryName = "Canada";
            searchCriteria = GetSearchCriteria(countryID, appNums, regNums);
            //GetNumHitsResult result = mqxService.GetNumHits(countryID, searchCriteria);
            marksResult = mqxService.GetMarks(countryID, searchCriteria);
            if (!marksResult.Success)
            {
                body += $"<p style='color: red'><b>An error occured during the testing for {countryName} trademark downloads using the sample application number: {appNums}.</b></p>";
            }
            else if (marksResult.Success)
            {
                body += $"<p>The trademark downloads testing for {countryName} has successfully passed, using the sample application number: {appNums}.</p>";
            }

            // EUTM Download Testing
            countryID = 608;
            appNums = "003648839";
            regNums = "";
            countryName = "EUTM";
            searchCriteria = GetSearchCriteria(countryID, appNums, regNums);
            //result = mqxService.GetNumHits(countryID, searchCriteria);
            marksResult = mqxService.GetMarks(countryID, searchCriteria);
            if (!marksResult.Success)
            {
                body += $"<p style='color: red'><b>An error occured during the testing for {countryName} trademark downloads using the sample application number: {appNums}.</b></p>";
            }
            else if (marksResult.Success)
            {
                body += $"<p>The trademark downloads testing for {countryName} has successfully passed, using the sample application number: {appNums}.</p>";
            }

            // Germany Download Testing
            countryID = 538;
            appNums = "396167020";
            regNums = "";
            countryName = "Germany";
            searchCriteria = GetSearchCriteria(countryID, appNums, regNums);
            //result = mqxService.GetNumHits(countryID, searchCriteria);
            marksResult = mqxService.GetMarks(countryID, searchCriteria);
            if (!marksResult.Success)
            {
                body += $"<p style='color: red'><b>An error occured during the testing for {countryName} trademark downloads using the sample application number: {appNums}.</b></p>";
            }
            else if (marksResult.Success)
            {
                body += $"<p>The trademark downloads testing for {countryName} has successfully passed, using the sample application number: {appNums}.</p>";
            }

            // United Kingdom Download Testing
            countryID = 666;
            appNums = "UK00915441389";
            regNums = "";
            countryName = "United Kingdom";
            searchCriteria = GetSearchCriteria(countryID, appNums, regNums);
            //result = mqxService.GetNumHits(countryID, searchCriteria);
            marksResult = mqxService.GetMarks(countryID, searchCriteria);
            if (!marksResult.Success)
            {
                body += $"<p style='color: red'><b>An error occured during the testing for {countryName} trademark downloads using the sample application number: {appNums}.</b></p>";
            }
            else if (marksResult.Success)
            {
                body += $"<p>The trademark downloads testing for {countryName} has successfully passed, using the sample application number: {appNums}.</p>";
            }

            // Mexico Download Testing
            countryID = 589;
            appNums = "1305492";
            regNums = "";
            countryName = "Mexico";
            searchCriteria = GetSearchCriteria(countryID, appNums, regNums);
            //result = mqxService.GetNumHits(countryID, searchCriteria);
            marksResult = mqxService.GetMarks(countryID, searchCriteria);
            if (!marksResult.Success)
            {
                body += $"<p style='color: red'><b>An error occured during the testing for {countryName} trademark downloads using the sample application number: {appNums}.</b></p>";
            }
            else if (marksResult.Success)
            {
                body += $"<p>The trademark downloads testing for {countryName} has successfully passed, using the sample application number: {appNums}.</p>";
            }

            // Argentina Download Testing
            countryID = 470;
            appNums = "3559671";
            regNums = "";
            countryName = "Argentina";
            searchCriteria = GetSearchCriteria(countryID, appNums, regNums);
            //result = mqxService.GetNumHits(countryID, searchCriteria);
            marksResult = mqxService.GetMarks(countryID, searchCriteria);
            if (!marksResult.Success)
            {
                body += $"<p style='color: red'><b>An error occured during the testing for {countryName} trademark downloads using the sample application number: {appNums}.</b></p>";
            }
            else if (marksResult.Success)
            {
                body += $"<p>The trademark downloads testing for {countryName} has successfully passed, using the sample application number: {appNums}.</p>";
            }

            // WIPO Download Testing
            countryID = 556;
            appNums = "1411733";
            regNums = "";
            countryName = "WIPO";
            searchCriteria = GetSearchCriteria(countryID, appNums, regNums);
            //result = mqxService.GetNumHits(countryID, searchCriteria);
            marksResult = mqxService.GetMarks(countryID, searchCriteria);
            if (!marksResult.Success)
            {
                body += $"<p style='color: red'><b>An error occured during the testing for {countryName} trademark downloads using the sample application number: {appNums}.</b></p>";
            }
            else if (marksResult.Success)
            {
                body += $"<p>The trademark downloads testing for {countryName} has successfully passed, using the sample application number: {appNums}.</p>";
            }

            // Australia Download Testing
            countryID = 473;
            appNums = "1314896";
            //1314896
            regNums = "";
            countryName = "Australia";
            searchCriteria = GetSearchCriteria(countryID, appNums, regNums);
            //result = mqxService.GetNumHits(countryID, searchCriteria);
            marksResult = mqxService.GetMarks(countryID, searchCriteria);
            if (!marksResult.Success)
            {
                body += $"<p style='color: red'><b>An error occured during the testing for {countryName} trademark downloads using the sample application number: {appNums}.</b></p>";
            }
            else if (marksResult.Success)
            {
                body += $"<p>The trademark downloads testing for {countryName} has successfully passed, using the sample application number: {appNums}.</p>";
            }

            if (body.Contains("error"))
            {
                subject = "*FAILURE* | The Trademark Download Testing Results contains an unsuccessful download";
            } 
            else if (body.Contains("successfully"))
            {
                subject = "*SUCCESS* | All Trademark Download Testing Results were successfully downloaded";
            }

            //if (countryID == 668)
            //{
            //    countryName = "United States of America";
            //}
            //else if (countryID == 498)
            //{
            //    countryName = "Canada";
            //}
            //else if (countryID == 608)
            //{
            //    countryName = "EUTM";
            //}
            //else if (countryID == 538)
            //{
            //    countryName = "Germany";
            //}
            //else if (countryID == 666)
            //{
            //    countryName = "United Kingdom";
            //}
            //else if (countryID == 589)
            //{
            //    countryName = "Mexico";
            //}
            //else if (countryID == 470)
            //{
            //    countryName = "Argentina";
            //}
            //else if (countryID == 556)
            //{
            //    countryName = "WIPO";
            //}
            //else if (countryID == 473)
            //{
            //    countryName = "Australia";
            //}

            string to = ConfigurationManager.AppSettings["EmailTo"];
            string cc = "";
            string bcc = "";
            //string subject = "*ALERT* | Trademark Download Testing Results";
            //body += "";
            body = WebUtility.HtmlDecode(body);
            string from = ConfigurationManager.AppSettings["EmailFrom"];
            MailMessage Mail = Globals.GenerateEmail(to, cc, bcc, subject, body, from);
            Globals.SendEmail(Mail);
             
        }

        static SearchCriteria GetSearchCriteria(int countryID, string appNum, string regNum)
        {

            Guid accountID = Guid.Parse("E2C66543-0643-4763-AF05-58C3DB631656");
            SearchCriteria searchCriteria = new SearchCriteria();
            //CountryLaw country = _settingsService.GetCountryLawByOldCountryID(countryID);
            string searchCriteriaString = string.Empty;

            searchCriteriaString += "Country: United States of America" + ", ";
            

            if (!string.IsNullOrEmpty(appNum))
            {
                searchCriteriaString += "Application Numbers: " + appNum;
                // only a single appnum is supported
                string cleanedAppNum = appNum.Trim();
                if (cleanedAppNum.Length > 0)
                {
                    searchCriteria.Add(new SearchCriteriaField(SearchCriteriaField.SearchField.SF_APPNUM, SearchCriteriaField.SearchOp.SO_EQUALS, cleanedAppNum));
                }
            }
            // don't expect both appnum and regnum
            if (searchCriteria.SearchFields.Count == 0)
            {
                if (!string.IsNullOrEmpty(regNum))
                {
                    searchCriteriaString += "Registration Numbers: " + regNum;
                    // only a single regnum is supported
                    string cleanedRegNum = regNum.Trim();
                    if (cleanedRegNum.Length > 0)
                    {
                        searchCriteria.Add(new SearchCriteriaField(SearchCriteriaField.SearchField.SF_REGNUM, SearchCriteriaField.SearchOp.SO_EQUALS, cleanedRegNum));
                    }
                }
            }

            if (searchCriteria.SearchFields.Count == 0)
            {
                searchCriteriaString = "None";
            }

            //Session["PTOSearchCriteria"] = searchCriteriaString;

            return searchCriteria;
        }
    }
}
