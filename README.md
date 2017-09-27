# iot-emulator
This is a solution I use to emulate IOT Scenarios sending data to Event Hub that includes some data bounded data randomization. 

The scenario I'm demonstrating in this example is for a medical device that might be in a patients room collecting measurements for things like SPo2, Body Temp, Blood Presure, Volume of medicine remaining.  Here the scenario isn't important, but rather just an example of how you can generate a series of values in a set range over a period of time.'

There are a few pieces to this solution:

##IotDevice

This is the class responsible for initializing values of the device, and maintaining the state of the device.  It has a constructor which sets up the initial values for a device and a "GetNextReading" method which generates the next value for the device.

##IotDeviceReading

This is a basic class that defines the structure of our reading.  It is serialized to json when sent to the evnet hub.

## Program.cs

This is a console app that builds up a collection (when are we ever getting readings from a single device), persists state of the device, and periodically generates the next set of values for the device.  The state is important because our random number generator is built to generate random numbers in an expected range, and make sure the next number generated is not too far from the last value.

