using Microsoft.WindowsAzure.Storage.Table;

namespace WebRole1
{
    class Entity : TableEntity
    {
        public Entity(string domain, string url)
        {
            this.PartitionKey = domain;
            this.RowKey = url;
        }

        public Entity() { }

        public string url { get; set; }

        public string title { get; set; }

        public string date { get; set; }

    }
}
