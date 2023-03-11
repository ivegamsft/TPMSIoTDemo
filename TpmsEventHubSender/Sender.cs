using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Text;
using System.Threading;
using TPMSIoTDemo.Common;

namespace TPMSIoTDemo.EventHub
{
    public class Sender
    {
        private static readonly string eventHubConnString = ConfigurationManager.AppSettings["Microsoft.EventHub.SenderConnectionString"];
        private static readonly string EventHubName = ConfigurationManager.AppSettings["Microsoft.EventHub.EventHubName"];
        private static readonly EventHubProducerClient EhClient = new EventHubProducerClient(eventHubConnString, EventHubName);
        private static string _factoryName = string.Empty;

        public static void GenerateCars(string FactoryName, int MaxCars)
        {
            _factoryName = FactoryName;
            
            Console.WriteLine("Starting cars");
            for (int i = 0; i < MaxCars; i++)
            {
                Car newCar = CarFactory.CreateCar(_factoryName);
                Console.WriteLine("Created a new car {0} of type {1} and class {2}", newCar.Id, newCar.VehicleType, newCar.VehicleClass);
            }
            // Keep sending.
            while (true)
            {
                int currentSpeed = 0;
                //Speed it up
                Console.WriteLine("Speed up the cars");
                VehicleTireReading currentReading;
                foreach (Car currentCar in CarFactory.Cars)
                {
                    int maxSpeed = currentCar.CalcMaxTireSpeed();
                    do
                    {
                        currentReading = currentCar.ReadTires();
                        WriteToConsole(currentReading, currentCar);

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
                            SendCarDataToEventHub(currentCar, currentReading);
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
                            WriteToConsole(currentReading, currentCar);
                            SendCarDataToEventHub(currentCar, currentReading);
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

        static void WriteToConsole(VehicleTireReading currentReading, Car currentCar)
        {
            Console.WriteLine("ReadingId={0};Factory={1};VehicleType={2};VehicleType={3},Speed={4};Miles={5}", currentReading.ReadingId, currentCar.FactoryName, currentCar.VehicleType, currentCar.VehicleClass, currentReading.CurrentSpeed, currentReading.CurrrentDistanceTraveled);
        }

        static void SendCarDataToEventHub(Car CurrentCar, VehicleTireReading TireData)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(TireData);
                EventData ehData = new EventData(Encoding.UTF8.GetBytes(jsonData));
                ehData.Properties.Add("VehicleType", TireData.VehicleType.ToString());
                ehData.Properties.Add("VehicleId", TireData.VehicleId.ToString());
                ehData.Properties.Add("ReadingId", TireData.ReadingId.ToString());
                var events = new Collection<EventData>
                {
                    ehData
                };
                //Send data 1 by 1
                EhClient.SendAsync(events).ContinueWith(c =>
                {
                    Console.WriteLine($"Sent Data for Vehicle={CurrentCar.Id};ReadingId={TireData.ReadingId}");
                }).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured writing to event hub" + ex.ToString());
            }
        }
    }
}
