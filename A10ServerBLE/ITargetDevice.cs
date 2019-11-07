using System;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace A10ServerBLE
{
    public struct DeviceCommand
    {
        public float interval;
        public int direction;
    }

    public interface ITargetDevice
    {
        void AddQueue(DeviceCommand command);

        void ClearQueue();


        void init(GattCharacteristic characteristic);

        void Open();
        void Close();

        void Start();

        void InitPosition();

        void Sync(float currentTime);

        void PublishCommand(DeviceCommand command);
        byte ResolveSpeed(float interval, int direction);
    }
}
