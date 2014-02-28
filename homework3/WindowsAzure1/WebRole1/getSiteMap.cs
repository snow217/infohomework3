using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;

namespace WebRole1
{
    public class getSiteMap
    {
        private Queue<string> allXMLs;
        private string getXML;

        public getSiteMap(Queue<string> SiteMaps)
        {
            allXMLs = SiteMaps;
            getXML = "";
        }

        public void Addqueue(Storage myStorage)
        {

            while (allXMLs.Count != 0)
            {
                string xml = allXMLs.Dequeue();

                if (xml.Contains("http"))
                {
                    using (var webclient = new WebClient())
                    {
                        getXML = webclient.DownloadString(xml);
                    }

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(getXML);

                    var results = xmlDoc.GetElementsByTagName("loc");

                    foreach (XmlElement result in results)
                    {
                        string links = result.InnerText;
                        if (links.Contains("xml"))
                        {
                            allXMLs.Enqueue(links);
                        }
                        else
                        {
                            myStorage.AddQueue(links.Trim(), false);
                        }
                    }
                }
            }
        }
    }
}
