using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Storage.Queues;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TPMSIoTDemo.Common;

namespace TPMSIoTDemo.EventHub
{
    public class Reader
    {
        private static string eventHubConnString = ConfigurationManager.AppSettings["Microsoft.EventHub.ReaderConnectionString"];
        private static string StorageConnString = ConfigurationManager.AppSettings["AzureStorage.ConnectionString"];
        private static string EventHubName = ConfigurationManager.AppSettings["Microsoft.EventHub.EventHubName"];
        private static string ConsumerGroupName = ConfigurationManager.AppSettings["Microsoft.EventHub.ConsumerGroup"];
        private static EventHubConsumerClient EhClient = new EventHubConsumerClient(ConsumerGroupName, eventHubConnString);
        //private static string _factoryName = string.Empty;

        private static int _processedMsgCount = 0;
        //private static int _batchMaximumCount = 0;
        //private static string _eventHubName = string.Empty;
        //private static string _connectionString = string.Empty;
        private static readonly string flatCarQueueName = "changeflatforcar";

        public static async Task ReadEvents()
        {
            var cancellationSource = new CancellationTokenSource();
            cancellationSource.CancelAfter(TimeSpan.FromSeconds(90));

            Console.WriteLine("Connect to event hub {0}.", EventHubName);

            Console.WriteLine("Create queue storage client {0}.", EventHubName);
            QueueClient queue = new QueueClient(StorageConnString, flatCarQueueName);
            while (true)
            {
                var events = EhClient.ReadEventsAsync(cancellationSource.Token).GetAsyncEnumerator();

                while (await events.MoveNextAsync())
                {
                    Console.WriteLine("Reading messages on partition {0}.", events.Current.Partition.PartitionId);
                    Console.WriteLine("Starting at message {0}.", events.Current.Data.SequenceNumber.ToString());
                    EventData msg = events.Current.Data;

                    if (msg != null)
                    {
                        Console.WriteLine("Message header");
                        Console.WriteLine("PartitionKey={0};SequenceNumber={1};Offset={2};EnqueuedTime={3}", msg.PartitionKey, msg.SequenceNumber, msg.Offset, msg.EnqueuedTime.ToUniversalTime().ToString());
                        Console.WriteLine("Message properties");
                        if (msg.Properties.ContainsKey("VehicleType"))
                        {
                            if (msg.Properties["VehicleType"].ToString() == CarType.SUV.ToString())
                            {
                                Console.WriteLine("Found an suv");
                            }
                        }
                        foreach (string propKey in msg.Properties.Keys)
                        {
                            Console.WriteLine("Key={0};value:{1}", propKey, msg.Properties[propKey] as string);
                        }

                        Console.WriteLine("System properties");
                        foreach (string propKey in msg.SystemProperties.Keys)
                        {
                            Console.WriteLine("Key={0};value:{1}", propKey, msg.SystemProperties[propKey] as string);
                        }

                        Console.WriteLine("Message Data");
                        //Deserialize the body
                        var jsonData = Encoding.UTF8.GetString(msg.EventBody.ToArray());

                        Console.WriteLine("Raw message={0}", jsonData);
                        try
                        {
                            VehicleTireReading reading = JsonConvert.DeserializeObject<VehicleTireReading>(jsonData, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

                            if (reading != null)
                            {
                                Console.WriteLine("Reading {0} for vehicle ID:{1} read {2}", reading.ReadingId, reading.VehicleId, reading.ReadingTimeStamp.ToString());
                                Console.WriteLine("HasFlat:{0}", reading.HasFlat);
                                if (reading.HasFlat)
                                {
                                    //send to the isflat queue
                                    ChangeFlatQMsg carInfo = new ChangeFlatQMsg
                                    {
                                        VehicleId = reading.VehicleId
                                    };
                                    // Create the queue if it doesn't already exist
                                    queue.CreateIfNotExists();
                                    var queueMessage = JsonConvert.SerializeObject(carInfo);
                                    queue.SendMessage(queueMessage);
                                    Console.WriteLine("Flat tire message added to the queue");
                                }

                                foreach (TireReading tr in reading.Readings)
                                {
                                    //Check to see if we have an under/over inflated tire. If so drop it on a queue for notification
                                    if (tr.IsUnderInflated || tr.IsOverInflated)
                                    {
                                        //TODO: Send to check pressure queue
                                    }
                                }
                                Console.WriteLine("Speed:{0}", reading.CurrentSpeed.ToString());
                                Console.WriteLine("Distance:{0}", reading.CurrrentDistanceTraveled.ToString());
                            }
                            _processedMsgCount++;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("An error occured :", ex.ToString());
                            //If we hit an error, checkpoint so we dont try and  process it again
                            //partitionEvent.CheckpointAsync(msg);
                            //Probably a servializatiion error
                        }
                    }
                    //After every 1000 messages, checkpoint
                    if (_processedMsgCount == 1000)
                    {
                        Console.WriteLine("Processed 1000 msgs. Checkpointing at {0}", msg.SequenceNumber.ToString());
                        //CHECKPOINT TO READ FROM THE LAST READ POINT INSTEAD OF THE BEGINNING
                        //context.CheckpointAsync(msg);
                        _processedMsgCount = 0;
                    }
                }
                await Task.Delay(1000);
            }
        }
    }
}
