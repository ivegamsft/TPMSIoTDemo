using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TpmsIotHubRemoveCars
{
    
    
    class Program
    {
        static string _factoryName = string.Empty;
        static string iotHubConnString = ConfigurationManager.AppSettings["Microsoft.IotHub.ConnectionString"];
        // Define the connection string to connect to IoT Hub

        static void Main(string[] args)
        {
            RegistryManager registryManager = RegistryManager.CreateFromConnectionString(iotHubConnString);
            //Remove the previous devices
            var devices = registryManager.GetDevicesAsync(1000).Result;
            int deviceCount = 0;
            foreach (Device d in devices)
            {
                Console.WriteLine(string.Format("Removing device-{0}", d.Id));
                registryManager.RemoveDeviceAsync(d).Wait();
                deviceCount++;
            }
            Console.WriteLine(string.Format("Removed {0} devices", deviceCount.ToString()));
            Console.Read();
        }
    }
}
