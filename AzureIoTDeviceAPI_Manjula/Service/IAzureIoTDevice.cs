using Microsoft.Azure.Devices.Client;

namespace AzureIoTDeviceAPI_Manjula.Service
{
    public interface IAzureIoTDevice
    {
        Task<string> AddDeviceAsync(string deviceName);
        Task<List<string>> GetAllDevices();
        Task<string> RemoveDeviceAsync(string deviceName);
        Task<string> UpdateDeviceAsync(string deviceName);
        Task<string> SendDeviceToCloudMessagesAsync(string connectionstring);
        Task<string> DesiredPropertiesUpdate(string deviceName);
        Task<string> ReportConnectivity(string deviceName);
    }
}
