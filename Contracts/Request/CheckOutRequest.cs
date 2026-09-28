namespace AttendanceAPI.Contracts.Request
{
    public class CheckOutRequest
    {
        public string EmployeeId { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? CheckOutAddress { get; set; }
        public string? PhotoBase64 { get; set; } = string.Empty;
    }
}
