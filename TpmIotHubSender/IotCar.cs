﻿using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Common;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TPMSIoTDemo.Common;

namespace TPMSIoTDemo.IoTHub
{
    public class IotCar
    {
        private static string _factoryName = string.Empty;
        static readonly string iotHubConnString = ConfigurationManager.AppSettings["Microsoft.IotHub.ConnectionString"];
        static readonly string iotHubName = ConfigurationManager.AppSettings["Microsoft.IotHub.Name"];
        // Define the connection string to connect to IoT Hub
        static RegistryManager registryManager;
        static DeviceClient carClient = null;
        static Car newCar = null;
        private static CancellationTokenSource cancelToken;
        static readonly Random randomTire = new Random();
        static int numflatTires = 0;

        public static CancellationTokenSource CancelToken { get => cancelToken; set => cancelToken = value; }

        public static void CreateCar(string FactoryName)
        {
            _factoryName = FactoryName; //The car factory name that created this car
            newCar = CarFactory.CreateCar(FactoryName);
            string newCarDeviceId = string.Format("{0}-{1}-{2}", FactoryName, newCar.VehicleType.ToString(), newCar.Id.ToString());
            string deviceConnectionString = SelfRegisterAndSetConnString(newCarDeviceId).Result;
            string iotHubConString = Microsoft.Azure.Devices.IotHubConnectionStringBuilder.Create(iotHubConnString).ToString();

            carClient = DeviceClient.CreateFromConnectionString(iotHubConString, newCarDeviceId);
            // Send an event
            SendEvent(carClient, newCar, newCar.ReadTires()).Wait();
            cancelToken = new CancellationTokenSource();
            // Receive commands
            Task commands = ReceiveCommands(carClient);
            Console.WriteLine("Created a new car {0} of type {1}", newCar.Id, newCar.VehicleType);

            while (true)
            {
                double currentSpeed = 0;
                int maxSpeed = 0;
                VehicleTireReading currentReading;
                foreach (Car currentCar in CarFactory.Cars)
                {
                    maxSpeed = currentCar.CalcMaxTireSpeed();
                    do
                    {
                        if (currentCar.State == VehicleState.Moving)
                        {
                            //Speed it up
                            Console.WriteLine("Speed up the car");
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

                                SendEvent(carClient, currentCar, currentReading).Wait();
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
                    }
                    while (currentSpeed < maxSpeed);
                }

                Thread.Sleep(new Random().Next(500, 3000));
                foreach (Car currentCar in CarFactory.Cars)
                {
                    if (currentCar.State == VehicleState.Moving)
                    {
                        //Now slow it down
                        Console.WriteLine("Slow down the car");
                        do
                        {
                            currentCar.Slow(new Random().Next(1, 10));
                            currentReading = currentCar.ReadTires();
                            if (!currentReading.HasFlat)
                            {
                                WriteToConsole(currentReading, currentCar);
                                SendEvent(carClient, currentCar, currentReading).Wait();
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

        static async Task<string> SelfRegisterAndSetConnString(string DeviceId)
        {
            Task<string> deviceConnString = null;
            try
            {
                registryManager = RegistryManager.CreateFromConnectionString(iotHubConnString);
                Device newDevice = new Device(DeviceId);

                await registryManager.AddDeviceAsync(newDevice);
                newDevice = await registryManager.GetDeviceAsync(DeviceId);
#pragma warning disable CS0618 // Type or member is obsolete
                newDevice.Authentication.SymmetricKey.PrimaryKey = CryptoKeyGenerator.GenerateKey(32);
#pragma warning restore CS0618 // Type or member is obsolete
                newDevice.Authentication.SymmetricKey.SecondaryKey = CryptoKeyGenerator.GenerateKey(32);
                newDevice = await registryManager.UpdateDeviceAsync(newDevice);

                string deviceInfo = String.Format("ID={0}\nPrimaryKey={1}\nSecondaryKey={2}", newDevice.Id, newDevice.Authentication.SymmetricKey.PrimaryKey, newDevice.Authentication.SymmetricKey.SecondaryKey);
                deviceConnString = Task.FromResult(string.Format("HostName={0};DeviceId={1};SharedAccessKey={2}", iotHubName, newDevice.Id, newDevice.Authentication.SymmetricKey.PrimaryKey));

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured creating device:{0}", ex.ToString());
            }
            return deviceConnString.Result;
        }

        static async Task SendEvent(DeviceClient deviceClient, Car CurrentCar, string message)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(message);
                Microsoft.Azure.Devices.Client.Message eventMessage = new Microsoft.Azure.Devices.Client.Message(Encoding.UTF8.GetBytes(jsonData));
                eventMessage.Properties.Add("messagetype", "ACK");
                await deviceClient.SendEventAsync(eventMessage);

                Console.WriteLine("Sent Data for Vehicle={0};ReadingId={1}", CurrentCar.Id, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured writing to event hub" + ex.ToString());
            }
        }

        // Create a message and send it to IoT Hub.
        static async Task SendEvent(DeviceClient deviceClient, Car CurrentCar, VehicleTireReading TireData)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(TireData);
                Microsoft.Azure.Devices.Client.Message eventMessage = new Microsoft.Azure.Devices.Client.Message(Encoding.UTF8.GetBytes(jsonData));
                eventMessage.Properties.Add("messagetype", "TLM");
                await deviceClient.SendEventAsync(eventMessage);

                Console.WriteLine("Sent Data for Vehicle={0};ReadingId={1}", CurrentCar.Id, TireData.ReadingId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured writing to event hub" + ex.ToString());
            }
        }


        // Receive messages from IoT Hub
        static async Task ReceiveCommands(DeviceClient deviceClient)
        {
            Console.WriteLine("\nDevice waiting for commands from IoTHub...\n");
            Microsoft.Azure.Devices.Client.Message receivedMessage;
            string messageData;
            while (true)
            {
                receivedMessage = await deviceClient.ReceiveAsync(TimeSpan.FromSeconds(1));

                if (receivedMessage != null)
                {
                    messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    WriteToConsole(string.Format("\t{0}> Received message: {1}", DateTime.Now.ToLocalTime(), messageData), true);
                    switch (messageData)
                    {
                        case "stop":
                            {
                                newCar.Stop();
                                await SendEvent(carClient, newCar, "Stopped the car");
                                WriteToConsole(string.Format("Stopped car {0}", newCar.Id), true);
                                break;
                            }
                        case "nail":
                            {
                                bool isTireFlat = false;
                                int MAX_NUM_TIRES = 4;
                                do
                                {
                                    if (numflatTires == MAX_NUM_TIRES) {
                                        await SendEvent(carClient, newCar, string.Format("All tires are flat"));
                                        WriteToConsole(string.Format("All tires are flat"), true);
                                        break; 
                                    }

                                    for (int i = 0; i < 3; i++)
                                    {
                                        int tireToFlatten = randomTire.Next(0, MAX_NUM_TIRES);

                                        if (!newCar.Tires[tireToFlatten].IsFlat())
                                        {
                                            newCar.Tires[tireToFlatten].Flatten();
                                            numflatTires++;
                                            await SendEvent(carClient, newCar, string.Format("Tire {0} was flattened", tireToFlatten.ToString()));
                                            isTireFlat = true;
                                            WriteToConsole(string.Format("Tire {0} was flattened", tireToFlatten.ToString()), true);
                                            break;
                                        }
                                    }
                                }
                                while (isTireFlat != true);

                                //if (numflatTires == MAX_NUM_TIRES)
                                //{
                                //    WriteToConsole(string.Format("All tires are already flat"), true);
                                //    isTireFlat = true;
                                //}
                                break;
                            }
                        case "miles":
                            {
                                await SendEvent(carClient, newCar, string.Format("Odometer reading for car {0} is {1}", newCar.Id.ToString(), newCar.OdometerInMiles.ToString()));
                                WriteToConsole(string.Format("Odometer reading for car {0} is {1}", newCar.Id.ToString(), newCar.OdometerInMiles.ToString()), true);
                                break;
                            }
                        case "replaceflat":
                            {
                                newCar.ReplaceFlat();
                                await SendEvent(carClient, newCar, "Flat replaced on car");
                                WriteToConsole(string.Format("Flat replaced on car {0}", newCar.Id), true);
                                break;
                            }
                        default:
                            {
                                newCar.Move(10);
                                await SendEvent(carClient, newCar, string.Format("Hello from the car {0}", newCar.Id.ToString()));
                                WriteToConsole(string.Format("Hello from the car {0}", newCar.Id.ToString()), true);
                                break;
                            }
                    }


                    await deviceClient.CompleteAsync(receivedMessage);
                }
            }
        }

        private static void WriteToConsole(string Message, bool Highlight)
        {
            if (Highlight)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(Message);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine(Message);
            }

        }

        private static void WriteToConsole(VehicleTireReading currentReading, Car currentCar)
        {
            Console.WriteLine("ReadingId={0};Factory={1};VehicleType={2};Speed={3};Miles={4}", currentReading.ReadingId, currentCar.FactoryName, currentCar.VehicleType, currentReading.CurrentSpeed, currentReading.CurrrentDistanceTraveled);
        }
    }
}
