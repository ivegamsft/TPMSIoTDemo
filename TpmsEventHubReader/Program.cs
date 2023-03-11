using System;
using System.Configuration;
using System.Threading.Tasks;

namespace TPMSIoTDemo.EventHub
{
    class Program
    {
        static void Main(string[] args)
        {
            //Reader eventHubReader = new Reader();
            Reader.ReadEvents().Wait();
        }
    }
}
