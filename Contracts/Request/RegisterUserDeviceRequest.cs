namespace AttendanceAPI.Contracts.Request
{
    public class RegisterUserDeviceRequest
    {
        public string? EmployeeId { get; set; }
        public string DeviceHash { get; set; }
        public string? DeviceManufacturer { get; set; }
        public string? DeviceModel { get; set; }
    }
}
