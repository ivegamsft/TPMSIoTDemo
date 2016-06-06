using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Configuration;
using Newtonsoft.Json;
using TpmsDemoClasses;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure;

namespace TpmsPressureReaderTest
{
    public class TpmsPressureProcessor : IEventProcessor
    {
        int processedMsgCount = 0;
        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            return Task.FromResult<object>(null);
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine("Partition {0} open.", context.Lease.PartitionId);
            return Task.FromResult<object>(null);
        }

        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            Console.WriteLine("Reading messages on partition {0}.", context.Lease.PartitionId);
            Console.WriteLine("Starting at message {0}.", context.Lease.SequenceNumber.ToString());
            if (messages != null)
            {
                foreach(var msg in messages)
                {
                    if (msg != null)
                    {
                        processedMsgCount++;
                        Console.WriteLine("Message header");
                        Console.WriteLine("PartitionKey={0};SequenceNumber={1};Offset={2};SizeInBytes={3};EnqueuedTime={4}", msg.PartitionKey, msg.SequenceNumber, msg.Offset, msg.SerializedSizeInBytes.ToString(), msg.EnqueuedTimeUtc.ToString());
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
                        var jsonData = Encoding.UTF8.GetString(msg.GetBytes());
                        Console.WriteLine("Raw message={0}", jsonData);
                        try {

                            var reading = JsonConvert.DeserializeObject<VehicleTireReading>(jsonData);
                            if (reading != null)
                            {
                                Console.WriteLine("Reading {0} for vehicle ID:{1} read {2}", reading.ReadingId, reading.VehicleId, reading.ReadingTimeStamp.ToString());
                                Console.WriteLine("HasFlat:{0}", reading.HasFlat);
                                if (reading.HasFlat)
                                {
                                    //send to the isflat queue
                                    ChangeFlatQMsg carInfo = new ChangeFlatQMsg();
                                    carInfo.VehicleId = reading.VehicleId;
                                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("AzureStorage.ConnectionString"));
                                    CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                                    CloudQueue queue = queueClient.GetQueueReference("changeflatforcar");
                                    // Create the queue if it doesn't already exist
                                    queue.CreateIfNotExists();
                                    var queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(carInfo));
                                    queue.AddMessage(queueMessage);
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
                        }
                        catch (Exception ex)
                        {
                            //Console.WriteLine("An error occured :", ex.ToString());
                            //If we hit an error, checkpoint so we dont try and  process it again
                            context.CheckpointAsync(msg);
                            //Probably a servializatiion error
                        }
                    }
                    //After every 1000 messages, checkpoint
                    if (processedMsgCount == 1000)
                    {
                        Console.WriteLine("Processed 1000 msgs. Checkpointing at {0}", msg.SequenceNumber.ToString());
                        //CHECKPOINT TO READ FROM THE LAST READ POINT INSTEAD OF THE BEGINNING
                        context.CheckpointAsync(msg);
                        processedMsgCount = 0;
                    }
                }
            }
            
            return Task.FromResult<object>(null);
        }
    }
    class Program
    {
        static string eventHubConnString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ReaderConnectionString"];
        static string storageConnString = ConfigurationManager.AppSettings["AzureStorage.ConnectionString"];
        static string eventHubName = ConfigurationManager.AppSettings["Microsoft.ServiceBus.EventHubName"];
        static string consumerGroupName = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConsumerGroup"];
        
        static void Main(string[] args)
        {
            try
            {
                //Use a unique name if you are running more than one
                var processorId = string.Format("Reader:{0}-{1}", Environment.MachineName, Guid.NewGuid());
                //var processorId = Environment.MachineName;

                var host = new EventProcessorHost(processorId, eventHubName, consumerGroupName, eventHubConnString, storageConnString);
                //host.RegisterEventProcessorAsync<TpmsPressureProcessor>();
                host.RegisterEventProcessorAsync<TpmsPressureProcessor>().Wait();
                Console.WriteLine("Begin reading messages with processor id [{0}].", processorId);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An erorr occured registering th reader " + ex.ToString());
            }
        }
    }
}
