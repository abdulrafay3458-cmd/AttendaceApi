namespace AttendanceAPI.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using AttendanceAPI.Services;
    using AttendanceAPI.Contracts.Request;

    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly ZKTecoService _zkService;
        private readonly DeviceService deviceService;

        public DeviceController( ZKTecoService zkService, DeviceService deviceService)
        {
            _zkService = zkService;
            this.deviceService = deviceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDevices()
        {
            var devices = await deviceService.GetDevicesAsync();
            return Ok(devices); 
        }
        [HttpPost("Update")]
        public async Task<IActionResult> EditDevices(DeviceRequest deviceRequest)
        {
            try
            {
                var device = await deviceService.EditDevices(deviceRequest);
                return Ok(new
                {
                    message = device,
                    success = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteDevices([FromQuery] string deviceId)
        {
            try
            {
                var device = await deviceService.DeleteDevices(deviceId);
                return Ok(new
                {
                    message = device,
                    success = true
                });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("AddNew")]
        public async Task<IActionResult> AddNewDevices(DeviceRequest deviceRequest)
        {
            try
            {
                var device = await deviceService.AddNewDevice(deviceRequest);
                return Ok(new
                {
                    message = device,
                    success = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("sync-users")]
        public async Task<IActionResult> SyncUsersFromDevice([FromBody] SyncUsersRequest request)
        {
            try
            {
                var employees = await _zkService.SyncUsersFromDevice(Guid.Parse(request.DeviceId));
                return Ok(new
                {
                    success = true,
                    message = $"Synced {employees.Count} users",
                    count = employees.Count,
                    employees
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("reconnect")]
        public async Task<IActionResult> ReconnectDevices()
        {
            try
            {
                await _zkService.ConnectToDevices();
                return Ok(new { success = true, message = "Reconnecting to devices" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("disconnectalldevices")]
        public async Task<IActionResult> DisconnectAllDevices()
        {
            try
            {
                await _zkService.DisconnectAll();
                return Ok(new { success = true, message = "Disconnected all devices" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    public class SyncUsersRequest
    {
        public string DeviceId { get; set; } = string.Empty;
    }

    //public class DeviceRequest
    //{
    //    public string DeviceName { get; set; }
    //    public string IpAddress { get; set; }
    //    public int MachineNumber { get; set; }
    //    public string Location { get; set; }
    //    public int PortNumber { get; set; }
    //    public bool IsActive { get; set; }
    //    public string DeviceId { get; set; }
    //    public string CompanyCode { get; set; }
    //}
}