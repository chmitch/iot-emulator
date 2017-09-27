using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iot_emulator
{
    class IotDeviceReading
    {
        public string ReadingGuid { get; set; }
        public int DeviceID { get; set; }
        //public string AlarmState { get; set; }
        public DateTime ReadingTime {get; set;}
        public double BodyTemperature { get; set; }
        public int Systolic { get; set; }
        public int Diastolic { get; set; }
        public int Pulse { get; set; }
        public int RespirationRate { get; set; }

        public int VolumeAlarm { get; set; }
 
        //This is a percentage acceptable values 1 or less.
        //Real range .95 - .99
        public double PulseOximetry { get; set; }

        //This is going to be a percentage, acceptabel value 0.0 - 1.0
        public double RemainingVolume { get; set; }

    }
}
