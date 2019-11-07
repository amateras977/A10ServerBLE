using System;
using System.Collections.Generic;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;

namespace A10ServerBLE
{
    public class TargetDeviceSearcher
    {

        private BluetoothLEAdvertisementWatcher advWatcher;

        public List<ITargetDevice> ResolvedDevices { get; } = new List<ITargetDevice>();

        public TargetDeviceSearcher()
        {
        }

        public void Start()
        {
            this.advWatcher = new BluetoothLEAdvertisementWatcher();
            this.advWatcher.SignalStrengthFilter.SamplingInterval = TimeSpan.FromMilliseconds(1000);
            this.advWatcher.Received += this.Watcher_Received;
            Logger.log("Start BLEDevice Scan");
            this.advWatcher.Start();
        }

        private async void Watcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {

            BluetoothLEDevice device = await BluetoothLEDevice.FromBluetoothAddressAsync(args.BluetoothAddress);

            if(device == null) {
                return; 
            }
            // Logger.log($"BLEDevice detected. DeviceName: {device.Name}");

            var targetDevice = TargetDeviceFactory.factory(device);

            if(targetDevice != null)
            {
                Logger.log($"Device Resolved. DeviceName: {device.Name}");
                this.ResolvedDevices.Add(targetDevice);
            }
        }
    }
}
