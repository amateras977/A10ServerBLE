using System;
using System.Collections.Generic;
using Windows.Devices.Bluetooth;
using A10ServerBLE.TargetDevice;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Diagnostics;

namespace A10ServerBLE
{
    public class TargetDeviceFactory
    {
        public static ITargetDevice factory(BluetoothLEDevice device)
        {
            return buildVorzeDevice(device);
        }

        private static ITargetDevice buildVorzeDevice(BluetoothLEDevice device)
        {
            string vorzeServiceUuidStr = "40ee1111-63ec-4b7f-8ce7-712efd55b90e";
            string vorzeCharactersticUuidStr = "40ee2222-63ec-4b7f-8ce7-712efd55b90e";
            string charactersticDescriptionName = "nls_command";

            IDictionary<string, Type> deviceMap = new Dictionary<string, Type>() {
                { "VorzePiston", typeof(VorzeA10Piston)},
                { "CycSA", typeof(VorzeA10Cyclone)},
                { "UFOSA", typeof(VorzeUFOSA)},

            };


            ITargetDevice concreateDevice = null;


            if (device == null) { return null; }



            GattDeviceServicesResult services = device.GetGattServicesForUuidAsync(new Guid(vorzeServiceUuidStr)).GetResults();

            var service = services != null && services.Services.Count > 0 ? services.Services[0] : null;

            if(service == null) { return null;  }

            Logger.log($"DeviceName: {service.Device.Name}");

            var characteristics = service.GetCharacteristicsForUuidAsync(new Guid(vorzeCharactersticUuidStr)).GetResults();

            if(characteristics == null || characteristics.Characteristics.Count < 1) { return null; }

            var characterstic = characteristics.Characteristics[0];
            if (characterstic != null)
            {
                foreach(var key in deviceMap.Keys)
                {
                    if (service.Device.Name.Contains(key))
                    {
                        concreateDevice = Activator.CreateInstance(deviceMap[key]) as ITargetDevice;
                        concreateDevice.init(characterstic);
                        break;
                    }
                }
            }
            return concreateDevice;
        }
    }
}