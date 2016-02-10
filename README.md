# TPMSIoTDemo
Tire pressure management system IoT demo
 
This demo provides a quick way to demo the following:
1. Send many events to an event hub
2. Read events from an event hub
3. Self-register and send to an IoT hub
4. Remove devices from an IoT Hub
 
## Project structure
It uses the concept of cars with tires to generate and send events. The structure of the project is as follows:
* TPMSCommon - Contains the common base classes for tires, cars and events
* TPMSEventHubReader - Contains the code to read from an event hub
* TMPS EventHubSender - Contains the code to send many car and tire readings to an event hub. The code generates a random number of cars from 1-100 and sends events to the hub
* TPMS IoTHubSender - Contains code for 1 car and tire reading to an event hub
* TMPS IotHubRemoveCars - Contiains code to remove the cars created from the IoT Hub sender
 
## Pre-requisites
* You should download the [IoT Hub explorer](https://github.com/Azure/azure-iot-sdks/blob/master/tools/iothub-explorer/)
* You must have a valid Azure subscription:
 
## Configuration
1. Create a new storage account
2. Create a new Service Bus
3. Create a new event hub
4. Create 2 SAS tokens to represent the reader and sender
5. Create a new IoTHub
6. Copy the connection strings into the app.config file in Settings folder. (Note: all of the programs point to a central config file):
 ```xml
	<add key="Microsoft.ServiceBus.SenderConnectionString" value="Endpoint=sb://SERVICEBUS-NAME.servicebus.windows.net/;SharedAccessKeyName=SENDER-SAS-NAME;SharedAccessKey=SENDER-SAS-KEY" />
	<add key="Microsoft.ServiceBus.ReaderConnectionString" value="Endpoint=sb://SERVICEBUS-NAME.servicebus.windows.net/;SharedAccessKeyName=READER-SAS-NAME;SharedAccessKey=READER-SAS-KEY" />
	<add key="Microsoft.IotHub.ConnectionString" value="HostName=IOTHUB-HOSTNAME.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=IOT-HUB-OWNER-SAS-KEY" />
	<add key="Microsoft.IotHub.Name" value="IOTHUB-HOSTNAME.azure-devices.net" />
	<add key="Microsoft.ServiceBus.EventHubName" value="EVENTHUB-NAME" />
	<add key="Microsoft.ServiceBus.ConsumerGroup" value="CONSUMER-GROUP-NAME_OR-$Default" />
	<add key="AzureStorage.AccountName" value="STORAGE-ACCOUNT-NAME" />
	<add key="AzureStorage.Key" value="STORAGE-ACCOUNT-KEY" />
	<add key="AzureStorage.ConnectionString" value="STORAGE-ACCOUNT-CONNECTIONSTRING" />
```

## To run the demo	
For the event hub Sender demo, right-click on the TpmsEventHubSender Project and select Debug > Start new instance. This will start the cars and send events to the hub

For the event hub Reader demo, right-click on the TpmsEventHubReader Project and select Debug > Start new instance. This will read the events from the hub. You can start as many of these projects as you have resources for. Note that depending on the number of partitions you have created, will determine the number of readers that can read simuteously.
 
For the IoT Hub sender demo, right-click on the TpmsEventHubReader Project and select Debug > Start new instance. This will create 1 car, self register it wiht IoT Hub and start sending events. Using the IoT Hub explorer, you should see the new car registered and you can monitor the events.

For command and control of the car, you can send the following from the IoT Explorer:
* Any string - The car will write the message to the console and begin to move
* Stop - The car will stop moving
* Nail - A random tire will be flattened and the car will slowly come to stop
* Miles - The odometer reading of the car
* ReplaceFlat - The tire with the flat will be replaced. Send a string to make the car go again

