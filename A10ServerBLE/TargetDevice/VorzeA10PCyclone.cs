
using System;
using System.Diagnostics;

using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;

using A10ServerBLE;

using System.Collections.Generic;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace A10ServerBLE.TargetDevice
{
    public class VorzeA10Cyclone : ITargetDevice
    {

        private static Queue<DeviceCommand> commandQueue = new Queue<DeviceCommand>();

        private float executingCommandInterval = 0f;
        private float nextExecuteTime = 0f;
        private long queueClearedCount = 0;
        private long stopCount = 0;

        private float lastTime = 0f;

        // A10Piston Minimum allowable command interval
        // (If below, the command may be ignored.)
        private float minimumInterval = 0.2f;

        private GattCharacteristic characteristic;

        public VorzeA10Cyclone()
        {
        }

        public void init(GattCharacteristic characteristic)
        {
            // nls_command
            this.characteristic = characteristic;
        }

        public void AddQueue(DeviceCommand command)
        {
            // Logger.log("AddQueue interval: {interval}, direction: {direction}");
            command.direction = command.direction * -1;
            commandQueue.Enqueue(command);
        }

        public void ClearQueue()
        {
            commandQueue.Clear();

            // reset timers
            executingCommandInterval = 0f;
            nextExecuteTime = 0f;

            queueClearedCount += 1;
        }

        public void Close()
        {
            // NOP
        }


        public async void InitPosition()
        {
            await Task.Delay(TimeSpan.FromMilliseconds((int)minimumInterval * 100));

            Logger.log("VorzeUFOSA InitPosition()");
            // Stop.
            await this.characteristic.WriteValueAsync(new byte[] { 1, 1, 0 }.AsBuffer());
        }

        public async void Open()
        {
            await this.characteristic.WriteValueAsync(new byte[] { 1, 1, 0 }.AsBuffer());

            this.InitPosition();
        }

        public async void PublishCommand(DeviceCommand command)
        {
            // 0-100(7bits. The closer to 0, the slower.)
            //
            byte speed = ResolveSpeed(command.interval, command.direction);

            int directionFlag = command.direction >= 1 ? (1 << 7) : 0;

            Logger.log($"VorzeUFOSA PublishCommand speed: {speed}, directionFlag: {directionFlag}, direction: {command.direction}");
            await this.characteristic.WriteValueAsync(new byte[] { 1, 1, (byte)(speed + directionFlag) }.AsBuffer());
        }

        public byte ResolveSpeed(float interval, int direction)
        {
            byte speed = 10;

            if (interval <= 0.1f)
            {
                speed = 100;
            }
            else if (interval <= 0.9f)
            {
                speed = (byte)(int)(100 * (1.0f - interval));
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
                /*
                float diff = lastTime - currentTime;
                if (diff > 0)
                {
                    Logger.log($" currentTime: {currentTime}, lastTime: {lastTime}, diff: {diff}");
                }
                */
                lastTime = currentTime;
                DeviceCommand command;
                if (commandQueue.Count > 0)
                {
                    command = commandQueue.Dequeue();
                    this.PublishCommand(command);

                    executingCommandInterval = command.interval <= minimumInterval ? minimumInterval : command.interval;
                    // Logger.log($" currentTime: {currentTime}, nextExecuteTime: {nextExecuteTime}, diff: {nextExecuteTime - currentTime}, interval: {command.interval}");


                    nextExecuteTime = currentTime + executingCommandInterval;
                }
                else
                {
                    if (nextExecuteTime == 0)
                    {
                        if (stopCount <= queueClearedCount)
                        {
                            this.InitPosition();
                            stopCount += 1;
                        }
                    }
                }
            }
        }
    }
}
