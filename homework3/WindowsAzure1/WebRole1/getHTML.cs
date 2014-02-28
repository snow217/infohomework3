using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;


namespace WebRole1
{
    public class getHTML
    {
        private string url;
        private DateTime LDate;
        private HtmlDocument HTMLdoc;

        public getHTML(string url)
        {
            try
            {
                Uri myUri = new Uri(url);
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(myUri);
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                LDate = myHttpWebResponse.LastModified;
                HTMLdoc = new HtmlWeb().Load(url);
                string[] myurl = url.Split('/');
                url = myurl[2];
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;     
                }
            }
        }

        public DateTime GetLDate()
        {
            return LDate;
        }
      
        public string GetTitle()
        {
            var doc = HTMLdoc.DocumentNode.SelectSingleNode("//head/title");
            return doc.InnerText.Trim();
        }

        public List<string> GetAllLink()
        {
            List<string> current = new List<string>();
            var linkNode = HTMLdoc.DocumentNode;
            foreach (HtmlNode link in linkNode.SelectNodes("//a[@href]"))
            {
                string linkValue = link.Attributes["href"].Value;
                if (linkValue.Contains(url))
                {
                    current.Add(linkValue.Trim());
                }
                else if (linkValue.Contains('/') && (!linkValue.Contains("http")))
                {
                    current.Add("http://" + url + linkValue.Trim());
                }
            }
            return current;
        }
    }
}