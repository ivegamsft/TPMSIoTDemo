using System;

namespace TPMSIoTDemo.EventHub
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        static void Main()
        {
            //Generate between 1 and 1000 cars and put them on the road
            string factoryId = "Factory-" + Guid.NewGuid().ToString();
            Sender.GenerateCars(factoryId, (new Random()).Next(1, 2));
            Console.ReadKey();
        }
    }
}
