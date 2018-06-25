using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Microsoft.ServiceBus.Messaging;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;


namespace iot_emulator
{
    class Program
    {
        //Assign a name for your database 
        private static readonly string evenstHubConnection = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
        private static readonly string useCosmos = ConfigurationManager.AppSettings["UseCosmos"];
        private static readonly string cosmosDBAccount = ConfigurationManager.AppSettings["CosmosDBAccount"];
        private static readonly string cosmosDBKey = ConfigurationManager.AppSettings["CosmosDBKey"];
        private static readonly string cosmosDBDatabase = ConfigurationManager.AppSettings["CosmosDBDatabase"];
        private static readonly string cosmosDBCollection = ConfigurationManager.AppSettings["CosmosDBCollection"];

        private static EventHubClient _eventHubClient;
        private static DocumentClient _cosmosClient;
        private static Database _cosmosDatabase;
        private static DocumentCollection _cosmosCollection;

        static int _deviceCount = 12;
        static int _timeToSleep = 1000;

        static void Main(string[] args)
        {
            string jsonMessage;
            EventSinkSetup();

            //Build a collection of device emulators
            List<IotDevice> _devices = new List<IotDevice>();
            for(int i = 1; i <= _deviceCount; i++)
            {
               _devices.Add(new IotDevice(i));
            }

            //Run until someone presses the escape key
            Console.WriteLine("Press ESC to stop");
            do
            {
                while (!Console.KeyAvailable)
                {
                    //Iterate the devices to get a reading.
                    foreach (IotDevice device in _devices)
                    {
                        //Get the reading object and send it to event hub.
                        IotDeviceReading reading = device.GetNextReading();
                        SendPayload(reading);
                        Console.WriteLine(JsonConvert.SerializeObject(reading));
                        
                    }
                    Thread.Sleep(_timeToSleep);
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

        }

        private static void SendPayload(IotDeviceReading reading)
        {
            string jsonMessage;
            if (useCosmos != "true")
            {
                jsonMessage = JsonConvert.SerializeObject(reading);
                _eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(jsonMessage)));
            }
            else {
                _cosmosClient.CreateDocumentAsync(_cosmosCollection.DocumentsLink, reading).Wait();
            }
        }

        private static void EventSinkSetup()
        {
            if (useCosmos != "true")
            {
                _eventHubClient = EventHubClient.CreateFromConnectionString(evenstHubConnection);
            }
            else
            {
                //Connect to the cosmosdb endpoint
                _cosmosClient = new DocumentClient(new Uri(cosmosDBAccount), cosmosDBKey);

                //Run a linq query to see if the database exists
                var query = _cosmosClient.CreateDatabaseQuery()
                    .Where(db => db.Id == cosmosDBDatabase);

                //if the linq query returns nothing create the database
                var databases = query.ToArray();
                if (databases.Any())
                {
                    _cosmosDatabase = databases.First();
                }
                else
                {
                    _cosmosDatabase = _cosmosClient.CreateDatabaseAsync(new Database { Id = cosmosDBDatabase }).Result;
                }

                //Run a linq query to see if the collection exists
                var collections = _cosmosClient.CreateDocumentCollectionQuery(_cosmosDatabase.SelfLink)
                     .Where(col => col.Id == cosmosDBCollection).ToArray();

                //If the linq query returns nothing create the collection
                if (collections.Any())
                {
                    _cosmosCollection = collections.First();
                }
                else
                {
                    DocumentCollection c = new DocumentCollection { Id = cosmosDBCollection };
                    _cosmosCollection = _cosmosClient.CreateDocumentCollectionAsync(_cosmosDatabase.SelfLink, c).Result;
                }

            }
        }
        
    }
}
