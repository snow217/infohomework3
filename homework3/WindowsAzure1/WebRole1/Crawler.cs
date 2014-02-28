using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Threading;
using WebRole1;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Configuration;

namespace WorkerRole1
{
    namespace WebRole1
    {
        public class Crawler
        {
            private List<string> errorList;
            private CloudTableClient tableClient;
            private HashSet<string> visitedUrl;
            private Queue<string> disAllow;

            private int count;
            private int errorcount;
            private string myurl;
            private getRobot robot;
            private getSiteMap siteMap;
            private Storage myStorage;

            public Crawler(string url)
            {
                errorList = new List<string>();
                myurl = url;
                robot = new getRobot(url);
                CloudStorageAccount storageAccount;storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
                tableClient = storageAccount.CreateCloudTableClient();
                visitedUrl = new HashSet<string>();
                siteMap = new getSiteMap(robot.GetSiteMap());
                disAllow = robot.GetDisAllowed();
                myStorage = new Storage();
            }

            public int getCount()
            {
                return count;
            }

            public int getallcount()
            {
                return count + errorcount;
            }

            public List<string> getError()
            {
                return errorList;
            }

            public void readyCrawling()
            {
                siteMap.Addqueue(myStorage);
            }

            public void StartCrawling()
            {
                string commandmessage = myStorage.PeekFromQueue(true);
                if (commandmessage.Equals("getStart"))
                {
                    string url = myStorage.getMsgFromQueue(false);
                    visitedUrl.Add(url);
                    getHTML parser = new getHTML(url);
                    Entity urlentity = new Entity( myurl, "PageUrl");
                    CloudTable table = tableClient.GetTableReference("hw3table");

                    TableOperation insertOperation = TableOperation.Insert(urlentity);
                    table.Execute(insertOperation);


                    foreach (string rightUrl in parser.GetAllLink())
                    {
                        foreach (string rule in disAllow.ToList())
                        {
                            if ((!rightUrl.Contains(rule)) && (!visitedUrl.Contains(rightUrl)))
                            {
                                myStorage.AddQueue(rightUrl, false);
                                count++;
                            }
                            else
                            {
                                errorList.Add(rightUrl);
                            }
                        }
                    }
                }
                else if (commandmessage.Equals("done"))
                {
                    myStorage.getMsgFromQueue(true);
                    errorcount++;
                }
              
            }
        }

    }
}