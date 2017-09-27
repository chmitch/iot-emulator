using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Microsoft.ServiceBus.Messaging;


namespace iot_emulator
{
    class Program
    {
        //Assign a name for your database 
        private static readonly string evenstHubConnection = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];

        private static EventHubClient HubClient;

        static int _deviceCount = 12;
        static int _timeToSleep = 1000;

        static void Main(string[] args)
        {
            string jsonMessage;
            HubSetup();

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
                        jsonMessage = JsonConvert.SerializeObject(reading);
                        Console.WriteLine(jsonMessage);
                        HubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(jsonMessage)));
                    }
                    Thread.Sleep(_timeToSleep);
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

        }

        private static void HubSetup()
        {
            HubClient = EventHubClient.CreateFromConnectionString(evenstHubConnection);
        }
    }
}
