using AzureIoTDeviceAPI_Manjula.Service;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Common;
using Microsoft.Azure.Devices.Common.Exceptions;
using Microsoft.Rest;
using AzureIoTDeviceAPI_Manjula.Service;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System.Text;

namespace AzureIoTDeviceAPI_Manjula.BL
{
    public class AzureIoTDevice : IAzureIoTDevice
    {

        private DeviceClient s_deviceClient;
        DeviceClient Client = null;
        static string DeviceConnectionString = "HostName=iothubtestdemo1.azure-devices.net;SharedAccessKeyName=iothubtestdemo1;SharedAccessKey=J9+yF1/v+WRMxpgu3S7UoZy5oz6bdb/Irw4yCLGEHTQ=";
        RegistryManager registryManager = RegistryManager.CreateFromConnectionString(DeviceConnectionString);
        public async Task<string> AddDeviceAsync(string deviceId)
        {
            string connectionstring = string.Empty;
            Device device;
            try
            {

                string newDeviceId = deviceId;

                device = await registryManager.AddDeviceAsync(new Device(newDeviceId));

                connectionstring = string.Format("HostName={0};DeviceId={1};SharedAccessKey={2}",
                "iothubtestdemo1.azure-devices.net", device.Id, device.Authentication.SymmetricKey.PrimaryKey);

                return "Device Created :- " + connectionstring;
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceId);
                return "Already existing device:";
            }
            //return $"Generated device key:{device.Authentication.SymmetricKey.PrimaryKey}";
        }
        public async Task<List<string>> GetAllDevices()
        {
            string connectionstring = string.Empty;
            try
            {
                var devices = await registryManager.GetDevicesAsync(100);
                List<string> allDevices = new List<string>();
                foreach (var device in devices)
                {
                    connectionstring = string.Format("HostName={0};DeviceId={1};SharedAccessKey={2}", "iothubtestdemo1.azure-devices.net", device.Id, device.Authentication.SymmetricKey.PrimaryKey);
                    allDevices.Add($"Id {device.Id} - Status {device.Status} - Reason {device.StatusReason} - Connection string - {connectionstring}");
                }
                return allDevices;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<string> UpdateDeviceAsync(string deviceName)
        {
            try
            {
                //Console.WriteLine("Update device - Enter Device Name");
                //var deviceName = Console.ReadLine();

                var d = await registryManager.GetDeviceAsync(deviceName);

                if (d != null)
                {
                    d.Status = DeviceStatus.Disabled;
                    d.StatusReason = "Disabled for test";

                    var dd = await registryManager.UpdateDeviceAsync(d);
                    return "Device status Updated";
                }
                else
                {
                    return "Device not found - Enter valid DeviceName";
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> RemoveDeviceAsync(string deviceName)
        {
            try
            {
                var deviceExists = await registryManager.GetDeviceAsync(deviceName);

                if (deviceExists != null)
                {
                    await registryManager.RemoveDeviceAsync(deviceName);
                    return deviceName + " is removed";
                }
                else
                {
                    return "Device not found - Enter valid DeviceName";
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //o	Sending telemetry messages from device to IoT Hub  
        public async Task<string> SendDeviceToCloudMessagesAsync(string connectionString)
        {
            try
            {

                /* string connectionString = string.Format("HostName=manjulaIoTTest.azure-devices.net;DeviceId={0};SharedAccessKey=GWmijvSchtl8eoezabXR3cBa46cbaL9Nb7nwazGwKW0=",
                               DeviceName);*/

                DeviceClient s_deviceClient = DeviceClient.CreateFromConnectionString(connectionString, Microsoft.Azure.Devices.Client.TransportType.Mqtt);
                double minTemperature = 20;
                double minHumidity = 60;
                Random rand = new Random();

                // while (true)
                {
                    double currentTemperature = minTemperature + rand.NextDouble() * 15;
                    double currentHumidity = minHumidity + rand.NextDouble() * 20;

                    // Create JSON message  

                    var telemetryDataPoint = new
                    {

                        temperature = currentTemperature,
                        humidity = currentHumidity
                    };

                    string messageString = "";

                    messageString = JsonConvert.SerializeObject(telemetryDataPoint);

                    var message = new Microsoft.Azure.Devices.Client.Message(Encoding.ASCII.GetBytes(messageString));

                    // Add a custom application property to the message.  
                    // An IoT hub can filter on these properties without access to the message body.  
                    //message.Properties.Add("temperatureAlert", (currentTemperature > 30) ? "true" : "false");  

                    // Send the telemetry message  
                    await s_deviceClient.SendEventAsync(message);
                    return DateTime.Now + " > Sending message: " + messageString;
                    // await Task.Delay(1000 * 10);

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<string> DesiredPropertiesUpdate(string DeviceName, string desiredPropertyValue)
        {

            var registryManager = RegistryManager.CreateFromConnectionString(DeviceConnectionString);
            var twin = await registryManager.GetTwinAsync(DeviceName);
            twin.Properties.Desired["devicedesiredproperty"] = desiredPropertyValue;
            await registryManager.UpdateTwinAsync(twin.DeviceId, twin, twin.ETag);
            return "Desired Property Updated";


            //    var twin = await registryManager.GetTwinAsync(DeviceName);
            //    var patch =
            //        @"{
            //    tags: {
            //        location: {
            //            region: 'US',
            //            plant: 'Redmond43'
            //        }
            //    }
            //}";
            //    await registryManager.UpdateTwinAsync(twin.DeviceId, patch, twin.ETag);

            //    var query = registryManager.CreateQuery(
            //      "SELECT * FROM devices WHERE tags.location.plant = 'Redmond43'", 100);
            //    var twinsInRedmond43 = await query.GetNextAsTwinAsync();
            //    string returnmsg1 = string.Empty;
            //    returnmsg1 = "Devices in Redmond43: "+
            //      string.Join(", ", twinsInRedmond43.Select(t => t.DeviceId));

            //    query = registryManager.CreateQuery("SELECT * FROM devices WHERE tags.location.plant = 'Redmond43' AND properties.reported.connectivity.type = 'cellular'", 100);
            //    var twinsInRedmond43UsingCellular = await query.GetNextAsTwinAsync();
            //    string returnmsg2 = string.Empty;
            //    returnmsg2 = "Devices in Redmond43 using cellular network: "+
            //      string.Join(", ", twinsInRedmond43UsingCellular.Select(t => t.DeviceId));
            //    return returnmsg1+ System.Environment.NewLine + returnmsg2 + System.Environment.NewLine +
            //        "Desired Properties updated";
        }

        //reported properties
        /* public async Task<string> InitClient(string DeviceName)
         {
             try
             {                
                 Client = DeviceClient.CreateFromConnectionString(DeviceConnectionString, DeviceName,
                   Microsoft.Azure.Devices.Client.TransportType.Mqtt);
                 //Console.WriteLine("Retrieving twin");
                 await Client.GetTwinAsync();
                 return "Retrieving twin";
             }
             catch (Exception ex)
             {
                return "Error in sample:" + ex.Message;
             }
         }*/
        public async Task<string> ReportConnectivity(string connectionString, string DeviceName, string reportPropertyValue)
        {
            try
            {
                ////Console.WriteLine("Sending connectivity data as reported property");
                //Client = DeviceClient.CreateFromConnectionString(connectionString, DeviceName,
                // Microsoft.Azure.Devices.Client.TransportType.Mqtt);
                ////Console.WriteLine("Retrieving twin");
                //await Client.GetTwinAsync();
                //TwinCollection reportedProperties, connectivity;
                //reportedProperties = new TwinCollection();
                //connectivity = new TwinCollection();
                //connectivity["type"] = "cellular";
                //reportedProperties["connectivity"] = connectivity;
                //await Client.UpdateReportedPropertiesAsync(reportedProperties);
                //return "Reported Properties Updated";

                var Client = DeviceClient.CreateFromConnectionString(connectionString, Microsoft.Azure.Devices.Client.TransportType.Mqtt);

                Microsoft.Azure.Devices.Shared.TwinCollection reportedProperties;
                reportedProperties = new Microsoft.Azure.Devices.Shared.TwinCollection();
                reportedProperties["connectivity"] = reportPropertyValue;
                await Client.UpdateReportedPropertiesAsync(reportedProperties);

                return "Report Property Updated";
            }
            catch (Exception ex)
            {
                return "Error in sample: " + ex.Message;
            }
        }
    }
}
