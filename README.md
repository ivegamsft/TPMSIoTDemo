# TPMSIoTDemo
Tire pressure management system IoT demo

This demo provides a quick way to demo the following:
1. Send many events to an event hub
2. Read events from an event hub
3. Self-register and send to an IoT hub
4. Remove devices from an IoT Hub

### Project structure
It uses the concept of cars with tires to generate and send events. The structure of the project is as follows:
* TPMSCommon - Contains the common base classes for tires, cars and events
* TPMSEventHubReader - Contains the code to read from an event hub
* TMPS EventHubSender - Contains the code to send many car and tire readings to an event hub. The code generates a random number of cars from 1-100 and sends events to the hub
* TPMS IoTHubSender - Contains code for 1 car and tire reading to an event hub
* TMPS IotHubRemoveCars - Contiains code to remove the cars created from the IoT Hub sender

## Configuration
You must have a valid Azure subscription:
Create a new storage account
Create a new Service Bus
Create a new event hub
Create 2 SAS tokens to represent the reader and sender
Create a new IoTHub
Copy the connection strings into the app.config file in Settings folder. (Note: all of the programs point to a central config file):

	'<add key="Microsoft.ServiceBus.SenderConnectionString" value="Endpoint=sb://SERVICEBUS-NAME.servicebus.windows.net/;SharedAccessKeyName=SENDER-SAS-NAME;SharedAccessKey=SENDER-SAS-KEY" />
	<add key="Microsoft.ServiceBus.ReaderConnectionString" value="Endpoint=sb://SERVICEBUS-NAME.servicebus.windows.net/;SharedAccessKeyName=READER-SAS-NAME;SharedAccessKey=READER-SAS-KEY" />
	<add key="Microsoft.IotHub.ConnectionString" value="HostName=IOTHUB-HOSTNAME.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=IOT-HUB-OWNER-SAS-KEY" />
	<add key="Microsoft.IotHub.Name" value="IOTHUB-HOSTNAME.azure-devices.net" />
	<add key="Microsoft.ServiceBus.EventHubName" value="EVENTHUB-NAME" />
	<add key="Microsoft.ServiceBus.ConsumerGroup" value="CONSUMER-GROUP-NAME_OR-$Default" />
	<add key="AzureStorage.AccountName" value="STORAGE-ACCOUNT-NAME" />
	<add key="AzureStorage.Key" value="STORAGE-ACCOUNT-KEY" />
	<add key="AzureStorage.ConnectionString" value="STORAGE-ACCOUNT-CONNECTIONSTRING" />'

### To run the demo	

