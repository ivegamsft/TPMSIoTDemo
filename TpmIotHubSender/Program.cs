using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Common;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using System.Configuration;
using Newtonsoft.Json;
using TpmsDemoClasses;
using System.Threading;

namespace TpmIotHubSender
{
    class Program
    {
        
        // Instantiate the CancellationTokenSource.

        static void Main(string[] args)
        {
            string factoryId = Environment.MachineName;
            IotCar.CreateCar(factoryId);
            Console.WriteLine("Exited!\n");
        }
    }
}