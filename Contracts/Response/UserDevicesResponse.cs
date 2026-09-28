namespace AttendanceAPI.Contracts.Response
{
    public class UserDevicesResponse
    {
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string? DeviceManufacturer { get; set; }
        public string? DeviceModel { get; set; }
    }
}
