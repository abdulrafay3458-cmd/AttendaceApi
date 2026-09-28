namespace AttendanceAPI.Contracts.Response
{
    public class AttendanceHistory
    {

        public DateTime AttendanceDate { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string? CheckInSource { get; set; } // zkteco, app, manual
        public string? DeviceName { get; set; }
        public string? TotalHours { get; set; }
        public string? Status { get; set; } // present, late, absent
        public string Location { get; set; }
    }
}
