namespace AttendanceAPI.Entities
{
    public class AttendenceHistory
    {
        public Guid AttendenceHistoryId { get; set; }
        public string EmployeeCode { get; set; }
        public DateTime AttendanceDate { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string? Source { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Location { get; set; }
        public Guid? DeviceId { get; set; }
        public string? Photo { get; set; }
    }
}
