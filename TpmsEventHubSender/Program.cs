using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using TpmsDemoClasses;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Microsoft.ServiceBus.Messaging;
using System.Threading;

namespace TpmsCarEventHubSender
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        static void Main()
        {
            //Generate between 1 and 1000 cars and put them on the road
            string factoryId = "Factory-" + Guid.NewGuid().ToString();
            Functions.GenerateCars(factoryId, (new Random()).Next(1, 1000));

            var host = new JobHost();
            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }
    }
}
