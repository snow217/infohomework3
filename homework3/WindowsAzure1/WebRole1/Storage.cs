using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WebRole1
{
    public class Storage
    {
        private CloudStorageAccount storageAccount;
        private CloudQueueClient queueClient;
        private CloudTableClient tableClient;

        public Storage()
        {
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            queueClient = storageAccount.CreateCloudQueueClient();
            tableClient = storageAccount.CreateCloudTableClient();
        }

        public string getMsgFromQueue(Boolean command)
        {
            CloudQueue queue;
            if (command == true)
            {
                queue = queueClient.GetQueueReference("hw3commandqueue");
            }
            else
            {
                queue = queueClient.GetQueueReference("hw3urlqueue");
            }
            CloudQueueMessage message = queue.GetMessage(TimeSpan.FromMinutes(5));
            if (message != null)
            {
                return message.AsString;
            }
            return "T T Failed!";
        }

        public void AddQueue(string url, Boolean command)
        {
            CloudQueue queue;
            if (command == true)
            {
                queue = queueClient.GetQueueReference("hw3commandqueue");
            }
            else
            {
                queue = queueClient.GetQueueReference("hw3urlqueue");
            }

            queue.CreateIfNotExists();
            CloudQueueMessage message = new CloudQueueMessage(url);
            queue.AddMessage(message);
        }

        public string PeekFromQueue(Boolean command)
        {
            CloudQueue queue;
            if (command == true)
            {
                queue = queueClient.GetQueueReference("hw3commandqueue");
            }
            else
            {
                queue = queueClient.GetQueueReference("hw3urlqueue");
            }
            CloudQueueMessage message = queue.PeekMessage();
            if (message != null)
            {
                return message.AsString;
            }
            return "T T Failed!";
        }

        public string GetTable(string domain, string url)
        {
            CloudTable table = tableClient.GetTableReference("hw3table");
            table.CreateIfNotExists();
            TableOperation retrieveOperation = TableOperation.Retrieve<Entity>(domain, url);
            TableResult retrievedResult = table.Execute(retrieveOperation);
            if (retrievedResult.Result != null)
                return ((Entity)retrievedResult.Result).title;
            else
                return "T T! Failed!";

        }

      /*  public string AddToTable(Entity urlEntity)
        {
            CloudTable table = tableClient.GetTableReference("hw3table");

            TableOperation insertOperation = TableOperation.Insert(urlEntity);
            var result = table.Execute(insertOperation);
            if (result != null)
            {
                return "Success";
            }
            else
            {
                return "Failed";
            }
        }
              */

        public void DeleteAll()
        {
            CloudQueue queue = queueClient.GetQueueReference("hw3urlqueue");
            queue.Delete();

            queue = queueClient.GetQueueReference("hw3commandqueue");
            queue.Delete();

            CloudTable table = tableClient.GetTableReference("hw3table");
            table.Delete();
        }

        public int geturlQueuesize()
        {
            CloudQueue queue = queueClient.GetQueueReference("hw3urlqueue");
            queue.FetchAttributes();
            int? cachedMessageCount = queue.ApproximateMessageCount;
            return (int)cachedMessageCount;
        }
    }
}