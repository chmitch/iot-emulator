using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iot_emulator
{
    class IotDevice
    {
        private IotDeviceReading _reading;
        private SampleDoubleGenerator _pulseOximetry;
        private SampleDoubleGenerator _pulse;
        private SampleDoubleGenerator _systolic;
        private SampleDoubleGenerator _diastolic;
        private SampleDoubleGenerator _bodyTemperature;
        private SampleDoubleGenerator _respirationRate;

        private double _flowRate;
        private double _remainingVolume;
 
        private int _refilWaitCount = 120;

        private int _volReset = 0;

        public IotDevice(int DeviceID)
        {
            SampleDoubleGenerator startingVolume = new SampleDoubleGenerator(.2, .9);
            
            _reading = new IotDeviceReading();
            _reading.DeviceID = DeviceID;
            _flowRate = .0001;

            //My random generator does peaks not floors. So inverting these numbers 1 - value will be the sent value
            _pulseOximetry = new SampleDoubleGenerator(0.01, 0.05);//, .01, 1000);
            _pulse = new SampleDoubleGenerator(60, 100);
            _systolic = new SampleDoubleGenerator(120, 140);//, 200, 100);
            _diastolic = new SampleDoubleGenerator(60, 80);//, 100, 100);
            _bodyTemperature = new SampleDoubleGenerator(95, 101);//, 105, 100);
            _respirationRate = new SampleDoubleGenerator(10, 30);//, 60, 100);
            _remainingVolume = startingVolume.GetNextValue();
        }

        public IotDeviceReading GetNextReading()
        {
            _reading.ReadingGuid = Guid.NewGuid().ToString();
            _reading.ReadingTime = DateTime.UtcNow;
            _reading.PulseOximetry = 1.0 - Math.Round(_pulseOximetry.GetNextValue(),2);
            _reading.Pulse = (int)_pulse.GetNextValue();
            _reading.Systolic = (int)_systolic.GetNextValue();
            _reading.Diastolic = (int)_diastolic.GetNextValue();
            _reading.BodyTemperature = Math.Round(_bodyTemperature.GetNextValue(),1);
            _reading.RespirationRate = (int)_respirationRate.GetNextValue();
            _remainingVolume = (_remainingVolume <= .0001?0:_remainingVolume - _flowRate);
            _reading.RemainingVolume = Math.Round(_remainingVolume, 4);
            if (_reading.RemainingVolume < .18)
            {
                if (_reading.RemainingVolume < .01)
                {
                    //Volume is really low.
                    _reading.VolumeAlarm = 2;
                }
                else
                {
                    //volume will be low soon
                    _reading.VolumeAlarm = 1;
                }
            }
            else
            //Volume is fine
            { _reading.VolumeAlarm = 0; }


            //hack to reset the volume after a specified wait period after its drained, otherwise all
            //medication will eventually be empty forever.  This simulates someone refilling the medication.
            if (_remainingVolume == 0)
            {
                if (_volReset < _refilWaitCount)
                    _volReset++;
                else
                {
                    _remainingVolume = 1.0;
                    _volReset = 0;
                }
            }


            return _reading;
        }

 
        
    }
}
