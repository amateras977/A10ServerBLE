using System;
using System.Diagnostics;

using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;

using A10ServerBLE;

using System.Collections.Generic;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace A10ServerBLE.TargetDevice
{
    public class VorzeA10Piston : ITargetDevice
    {

        private static Queue<DeviceCommand> commandQueue = new Queue<DeviceCommand>();

        private float executingCommandInterval = 0f;
        private float nextExecuteTime = 0f;

        private float lastTime = 0f;

        // A10Piston Minimum allowable command interval
        // (If below, the command may be ignored.)
        private float minimumInterval = 0.2f;

        private GattCharacteristic characteristic;

        public VorzeA10Piston()
    	{
    	}

        public void init(GattCharacteristic characteristic)
        {
            // nls_command
            this.characteristic = characteristic;
        }

        public void AddQueue(DeviceCommand command)
        {
            command.direction = command.direction * -1;
            commandQueue.Enqueue(command);
            Logger.log($"VorzeA10Piston AddQueue interval: {command.interval}, direction: {command.direction}");
        }

        public void ClearQueue()
        {
            commandQueue.Clear();

            // reset timers
            executingCommandInterval = 0f;
            nextExecuteTime = 0f;
        }

        public void Close()
        {
            // NOP
        }


        public async void InitPosition()
        {
            await Task.Delay(TimeSpan.FromMilliseconds((int) minimumInterval * 100));

            // Forced transition to the front
            await this.characteristic.WriteValueAsync(new byte[] { 3, 0, 60 }.AsBuffer());
        }

        public async void Open()
        {
            await this.characteristic.WriteValueAsync(new byte[] { 3, 1, 0 }.AsBuffer());
            
            this.InitPosition();
        }

        public async void PublishCommand(DeviceCommand command)
        {
            // 10-60 (The closer to 0, the slower.)
            //
            // 10 or more recommended. 
            // Is less than 10 not recommended, the burden on the motor is very heavy
            // 
            byte speed = ResolveSpeed(command.interval, command.direction);

            // 0-200 (Front is 0.)
            //
            byte position = (byte)(command.direction > 0 ? 200 : 0);

            //  Logger.log($"onPublish speed: {speed}, positon: {position}");
            await this.characteristic.WriteValueAsync(new byte[] { 3, position, speed }.AsBuffer());
        }

        public byte ResolveSpeed(float interval, int direction)
        {
            // Convert back to stroke speed from the interval.
            //
            // Front and back stroke speed is asymmetric.
            // (From back to front is slower than front to back.)
            //
            byte speed = 10; // defaults. minimum.

            if (interval <= 0.1f)
            {
                speed = 60;
            }
            else if (interval <= 0.12f)
            {
                speed = 55;

            }
            else if (interval <= 0.2f)
            {
                speed = 50;
            }
            else if (interval <= 0.3f)
            {
                speed = direction > 0 ? (byte)20 : (byte)30;
            }
            else if (interval <= 0.55f)
            {
                speed = direction > 0 ? (byte)15 : (byte)20;
            }
            else if (interval <= 0.70f)
            {
                speed = direction > 0 ? (byte)13 : (byte)20;
            }
            else
            {
                speed = 10;
            }

            return speed;
        }

        public async void Start()
        {
            var sw = new Stopwatch();
            sw.Start();

            float lastTime = 0f;
            while (true)
            {

                float currentTime = (float)sw.ElapsedMilliseconds / 1000;

                Sync(currentTime);

                lastTime = currentTime;
                await Task.Delay(TimeSpan.FromMilliseconds(1));
            }
        }

        public void Sync(float currentTime)
        {
            if (currentTime >= nextExecuteTime)
            {
                float diff = lastTime - currentTime;
                if (diff > 0)
                {
                    Logger.log($" currentTime: {currentTime}, lastTime: {lastTime}, diff: {diff}");
                }
                lastTime = currentTime;
                DeviceCommand command;
                if (commandQueue.Count > 0)
                {
                    command = commandQueue.Dequeue();
                    this.PublishCommand(command);

                    executingCommandInterval = command.interval <= minimumInterval ? minimumInterval : command.interval;
                    Logger.log($" currentTime: {currentTime}, nextExecuteTime: {nextExecuteTime}, diff: {nextExecuteTime - currentTime}, interval: {command.interval}");


                    nextExecuteTime = currentTime + executingCommandInterval;
                }
            }
        }
    }
}
