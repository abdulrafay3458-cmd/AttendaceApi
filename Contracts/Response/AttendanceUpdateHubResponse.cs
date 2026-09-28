namespace AttendanceAPI.Contracts.Response
{
    public class AttendanceUpdateHubResponse
    {
        public string EmployeeName { get; set; }
        public string? DeviceName { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
    }
}
