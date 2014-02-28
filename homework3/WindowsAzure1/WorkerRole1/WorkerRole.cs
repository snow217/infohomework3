using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using WebRole1;
using WorkerRole1.WebRole1;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        Storage myStorage = new Storage();
        List<string> availableLinks = new List<string>();
        HashSet<string> visitedUrl = new HashSet<string>();
        private string myURL = "www.cnn.com";

        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("WorkerRole1 entry point called", "Information");

            while (true)
            {
                Thread.Sleep(10000);
                Trace.TraceInformation("Working", "Information");

            }
       }
    }

}
