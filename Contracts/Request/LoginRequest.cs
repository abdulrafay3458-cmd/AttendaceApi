namespace AttendanceAPI.Contracts.Request
{
    public class LoginRequest
    {
        public string EmployeeCode { get; set; }
        public string Password { get; set; }
        public string DeviceHash { get; set; }
    }
}
