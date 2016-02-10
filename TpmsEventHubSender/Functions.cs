using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using TpmsDemoClasses;
using System.Threading;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage;

namespace TpmsCarEventHubSender
{
    public class Functions
    {

        static string eventHubConnString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.SenderConnectionString");
        static string storageConnString = CloudConfigurationManager.GetSetting("AzureStorage.ConnectionString");
        static string eventHubName = CloudConfigurationManager.GetSetting("AzureStorage.AccountName");
        static string consumerGroupName = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConsumerGroup");
        static EventHubClient ehClient = EventHubClient.CreateFromConnectionString(eventHubConnString, "tiredata");
        static string _factoryName = string.Empty;

        public static void GenerateCars(string FactoryName, int MaxCars)
        {
            _factoryName = FactoryName;
            // Keep sending.
            while (true)
            {
                 Console.WriteLine("Starting cars");
                for (int i = 0; i < MaxCars; i++)
                {
                    Car newCar = CarFactory.CreateCar(_factoryName);
                    Console.WriteLine("Created a new car {0} of type {1}", newCar.Id, newCar.TypeOfCar);
                }
                while (true)
                {
                    int currentSpeed = 0;
                    int maxSpeed = 0;
                    VehicleTireReading currentReading = null;
                    //Speed it up
                    Console.WriteLine("Speed up the cars");
                    foreach (Car currentCar in CarFactory.Cars)
                    {
                        maxSpeed = currentCar.CalcMaxSpeed();
                        do
                        {
                            currentReading = currentCar.ReadTires();
                            writeToConsole(currentReading, currentCar);

                            //If we have a flat, stop the car
                            if (currentReading.HasFlat)
                            {
                                maxSpeed = 0;
                                currentSpeed = 0;
                                currentCar.Stop();
                            }
                            else
                            {
                                currentCar.Move(new Random().Next(1, 10));
                                sendCarDataToEventHub(currentCar, currentReading);
                                currentSpeed = currentReading.CurrentSpeed;
                                //Sleep to simulate traffic
                                if (currentSpeed == new Random().Next(0, maxSpeed))
                                {
                                    Thread.Sleep(new Random().Next(500, 3000));
                                    currentCar.AddAirToTires(new Random().Next(0, 3), new Random().Next(0, 5));
                                    break;
                                }
                            }
                        }
                        while (currentSpeed < maxSpeed);
                    }

                    Thread.Sleep(new Random().Next(500, 3000));
                    //Now slow it down
                    Console.WriteLine("Slow down the car");
                    foreach (Car currentCar in CarFactory.Cars)
                    {
                        do
                        {
                            currentCar.Slow(new Random().Next(1, 10));
                            currentReading = currentCar.ReadTires();
                            if (!currentReading.HasFlat)
                            {
                                writeToConsole(currentReading, currentCar);
                                sendCarDataToEventHub(currentCar, currentReading);
                                currentSpeed = currentCar.CurrentSpeed;
                                if (currentSpeed == 0)
                                {

                                    Thread.Sleep(new Random().Next(500, 3000));
                                    currentCar.RemoveAirFromTires(new Random().Next(0, 3), new Random().Next(0, 5));
                                    break;
                                }
                            }
                        }
                        while (currentSpeed > 0);
                    }
                }
            }
        }

        static void writeToConsole(VehicleTireReading currentReading, Car currentCar)
        {
            Console.WriteLine("ReadingId={0};Factory={1};VehicleType={2};Speed={3};Miles={4}", currentReading.ReadingId, currentCar.FactoryName, currentCar.TypeOfCar, currentReading.CurrentSpeed, currentReading.CurrrentDistanceTraveled);
        }

        static void sendCarDataToEventHub(Car CurrentCar, VehicleTireReading TireData)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(TireData);
                EventData ehData = new EventData(Encoding.UTF8.GetBytes(jsonData));
                ehData.Properties.Add("VehicleType", TireData.TypeOfCar);
                ehData.Properties.Add("VehicleId", TireData.VehicleId.ToString());
                ehData.Properties.Add("ReadingId", TireData.ReadingId);
                ehClient.Send(ehData);
                Console.WriteLine("Sent Data for Vehicle={0};ReadingId={1}", CurrentCar.Id, TireData.ReadingId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured writing to event hub" + ex.ToString());
            }
        }
    }
}
