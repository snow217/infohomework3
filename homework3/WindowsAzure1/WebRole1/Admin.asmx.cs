using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using Microsoft.WindowsAzure;
using WorkerRole1.WebRole1;

namespace WebRole1
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Admin : System.Web.Services.WebService
    {

        private static Storage myStorage = new Storage();
        private static PerformanceCounter performanceCounterRAM;
        private static PerformanceCounter performanceCounterCPU;
        Crawler myCrawler = new Crawler("www.cnn.com");

        public Admin()
        {
            performanceCounterRAM = new PerformanceCounter("Memory", "% Committed Bytes In Use");
            performanceCounterCPU = new PerformanceCounter("Processor", "% Processor Time", "_Total");      
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string RamUsed()
        {
            string answer = ((int)performanceCounterRAM.NextValue()).ToString() + '%';
            return new JavaScriptSerializer().Serialize(answer);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string CPUUsed()
        {
            string answer = ((int)performanceCounterCPU.NextValue()).ToString() + '%';
            return new JavaScriptSerializer().Serialize(answer);
        }

        [WebMethod]
        public List<string> TestRobotDisAllowed(string url)
        {
            getRobot test = new getRobot(url);
            List<string> disAllowed = test.GetDisAllowed().ToList();
            return disAllowed;
        }

        [WebMethod]
        public List<string> TestRobotSiteMap(string url)
        {
            getRobot test = new getRobot(url);
            return test.GetSiteMap().ToList(); 
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string TestHtmlTitle(string url)
        {
            string answer;
            try
            {
                getHTML test = new getHTML("http://" + url + "/");
                answer = test.GetTitle();
            }
            catch
            {
                answer = "T T invalid URL!!!";
            }
            return new JavaScriptSerializer().Serialize(answer);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string TestHtmlLdate(string url)
        {
            DateTime answer;
            try
            {
                getHTML test = new getHTML("http://" + url + "/");
                answer = test.GetLDate();
            }
            catch
            {
                answer = default(DateTime);
            }
            return new JavaScriptSerializer().Serialize(answer.ToString());
        }


        [WebMethod]
        public void ClearIndex()
        {
            myStorage.DeleteAll();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void StartCrawling()
        {
            myStorage.AddQueue("getStart", true);
            myCrawler.readyCrawling();
            myCrawler.StartCrawling();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void StopCrawling()
        {
            myStorage.AddQueue("done", true);
            myCrawler.StartCrawling();
        }

        [WebMethod]
        public string GetPageTitle(string url)
        {
           return myStorage.GetTable("www.cnn.com", url);
        }
               
        [WebMethod]
        public string commandmessage()
        {
            if (myStorage.PeekFromQueue(true).Equals("getStart"))
                return "true";
            else
            {
                return "false";
            }
        }

        [WebMethod]
        public string urlqueueTest()
        {
            return myStorage.getMsgFromQueue(false);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string getQueueSize()
        {
            string answer = myStorage.geturlQueuesize().ToString();
            return new JavaScriptSerializer().Serialize(answer);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string getIndexSize()
        {
            string answer = myCrawler.getCount().ToString();
            return new JavaScriptSerializer().Serialize(answer);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string getErrorList()
        {
            string answer = string.Join(",", myCrawler.getError());
            return new JavaScriptSerializer().Serialize(answer);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string getAllcount()
        {
            string answer = string.Join(",", myCrawler.getallcount());
            return new JavaScriptSerializer().Serialize(answer);
        }
    }
}
