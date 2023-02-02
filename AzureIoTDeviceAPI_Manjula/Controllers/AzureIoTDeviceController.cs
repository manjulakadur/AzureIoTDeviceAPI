using AzureIoTDeviceAPI_Manjula.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices.Client;

namespace AzureIoTDeviceAPI_Manjula.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AzureIoTDeviceController : ControllerBase
    {
        private readonly IAzureIoTDevice _deviceIoTService;
        public AzureIoTDeviceController(IAzureIoTDevice deviceIoTService)
        {
            _deviceIoTService = deviceIoTService ?? throw new ArgumentNullException(nameof(deviceIoTService));
        }

        [HttpPost]
        [Route("CreateDevice")]
        public async Task<string> CreateDevice(string DeviceName)
        {
            return await _deviceIoTService.AddDeviceAsync(DeviceName);
        }

        [HttpGet]
        [Route("GetAllDevices")]
        public async Task<List<string>> GetAllDevices()
        {
            return await _deviceIoTService.GetAllDevices();
        }

        [HttpPut]
        [Route("UpdateDevice")]
        public async Task<string> UpdateDevice(string DeviceName)
        {
            return await _deviceIoTService.UpdateDeviceAsync(DeviceName);
        }

        [HttpDelete]
        [Route("DeleteDevice")]
        public async Task<string> DeleteDevice(string DeviceName)
        {
            return await _deviceIoTService.RemoveDeviceAsync(DeviceName);
        }

        [HttpPost]
        [Route("ReportedProperties")]
        public async Task<string> ReportedProperties(string connectionString, string DeviceName, string reportPropertyValue)
        {
            return await _deviceIoTService.ReportConnectivity(connectionString, DeviceName, reportPropertyValue);
        }

        [HttpPost]
        [Route("DesiredProperties")]
        public async Task<string> DesiredProperties(string DeviceName, string desiredPropertyValue)
        {
            return await _deviceIoTService.DesiredPropertiesUpdate(DeviceName, desiredPropertyValue);
        }

        [HttpPost]
        [Route("TelemetryMessage")]
        public async Task<string> TelemetryMessage(string connectionstring)
        {
            return await _deviceIoTService.SendDeviceToCloudMessagesAsync(connectionstring);
        }

    }
}
