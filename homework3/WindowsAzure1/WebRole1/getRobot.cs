using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace WebRole1
{
    public class getRobot
    {
        private string line;
        private List<string> Lines;

        public getRobot(string url)
        {
            line = "";
            Lines = new List<string>();
            WebClient client = new WebClient();

            try
            {
                using (StreamReader r = new StreamReader(client.OpenRead("http://" + url + "/robots.txt")))
                {
                    while ((line = r.ReadLine()) != null)
                    {
                        Lines.Add(line);
                    }
                }
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                }
            }
        }

        public Queue<string> GetDisAllowed()
        {
            Queue<string> current = new Queue<string>();
            try
            {
                foreach (string line in Lines)
                {
                    if (line.Contains("Disallow"))
                    {
                        string[] word = line.Split(':');
                        current.Enqueue(word[1].Trim());
                    }
                }
            }
            catch
            {
                current.Enqueue("T T fail to get disallowed");
            }
            return current;
        }

        public Queue<string> GetSiteMap()
        {
            Queue<string> current = new Queue<string>();

            try
            {
                foreach (string line in Lines)
                {
                    if (line.Contains("Sitemap"))
                    {
                        string[] word = line.Split(new string[] { "Sitemap:" }, StringSplitOptions.None);
                        string links = word[1].Trim();
                        current.Enqueue(links);
                    }
                }
            }catch{
                current.Enqueue("T T fail. Can not find sitemap");
            }
            return current;
        }
    }
}