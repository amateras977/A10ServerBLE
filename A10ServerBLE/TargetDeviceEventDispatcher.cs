using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace A10ServerBLE
{
    public class TargetDeviceEventDispatcher : ITargetDevice
    {
        private TargetDeviceSearcher searcher;
        public void init(TargetDeviceSearcher searcher)
        {
            this.searcher = searcher;
        }

        public void AddQueue(DeviceCommand command)
        {

            List<Task> tasks = new List<Task>();
            foreach(var device in searcher.ResolvedDevices)
            {
                tasks.Add( Task.Run(() => device.AddQueue(command)) );
            }

            if(tasks != null && tasks.Count > 0)
            {
                Task.WaitAll(tasks.ToArray());
            }
        }

        public void ClearQueue()
        {
            List<Task> tasks = new List<Task>();
            foreach(var device in searcher.ResolvedDevices)
            {
                tasks.Add( Task.Run(() => device.ClearQueue()) );
            }

            if(tasks != null && tasks.Count > 0)
            {
                Task.WaitAll(tasks.ToArray());
            }
        }

        public void Close()
        {
            List<Task> tasks = new List<Task>();
            foreach(var device in searcher.ResolvedDevices)
            {
                tasks.Add( Task.Run(() => device.Close()) );
            }

            if(tasks != null && tasks.Count > 0)
            {
                Task.WaitAll(tasks.ToArray());
            }
        }

        void ITargetDevice.init(GattCharacteristic characteristic)
        {
            throw new NotImplementedException();
        }

        public void InitPosition()
        {
            List<Task> tasks = new List<Task>();
            foreach(var device in searcher.ResolvedDevices)
            {
                tasks.Add( Task.Run(() => device.InitPosition()) );
            }

            if(tasks != null && tasks.Count > 0)
            {
                Task.WaitAll(tasks.ToArray());
            }
        }

        public void Open()
        {
            List<Task> tasks = new List<Task>();
            foreach(var device in searcher.ResolvedDevices)
            {
                tasks.Add( Task.Run(() => device.Open()) );
            }

            if(tasks != null && tasks.Count > 0)
            {
                Task.WaitAll(tasks.ToArray());
            }
        }

        public void PublishCommand(DeviceCommand command)
        {
            List<Task> tasks = new List<Task>();
            foreach(var device in searcher.ResolvedDevices)
            {
                tasks.Add( Task.Run(() => device.PublishCommand(command)) );
            }

            if(tasks != null && tasks.Count > 0)
            {
                Task.WaitAll(tasks.ToArray());
            }
        }

        byte ITargetDevice.ResolveSpeed(float interval, int direction)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            List<Task> tasks = new List<Task>();
            foreach(var device in searcher.ResolvedDevices)
            {
                tasks.Add( Task.Run(() => device.Start()) );
            }

            if(tasks != null && tasks.Count > 0)
            {
                Task.WaitAll(tasks.ToArray());
            }
        }

        public void Sync(float currentTime)
        {
            List<Task> tasks = new List<Task>();
            foreach(var device in searcher.ResolvedDevices)
            {
                tasks.Add( Task.Run(() => device.Sync(currentTime)) );
            }

            if(tasks != null && tasks.Count > 0)
            {
                Task.WaitAll(tasks.ToArray());
            }
        }
    }

}
