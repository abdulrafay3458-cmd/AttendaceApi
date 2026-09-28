namespace AttendanceAPI.Contracts.Request
{
    public class CheckInRequest
    {
        public string EmployeeId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? CheckInAddress { get; set; }
        public string? PhotoBase64 { get; set; } = string.Empty;
    }
}
