namespace AttendanceAPI.Entities
{
    public class AttendanceRecord
    {
        public string EmployeeCode { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Fortnight { get; set; }
        public int Day { get; set; }
        public int? StandardHourCode { get; set; }
        public int? StandardHourWeekday { get; set; }
        public DateTime AttendanceDate { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string? CheckInSource { get; set; } // zkteco, app, manual
        public string? CheckOutSource { get; set; }
        public double? CheckInLatitude { get; set; }
        public double? CheckInLongitude { get; set; }
        public double? CheckOutLatitude { get; set; }
        public double? CheckOutLongitude { get; set; }
        public string? CheckInPhoto { get; set; }
        public string? CheckOutPhoto { get; set; }
        public string? CheckInAddress { get; set; }
        public string? CheckOutAddress { get; set; }
        public string? TotalHours { get; set; }
        public string? Status { get; set; } // present, late, absent
        public Guid? CheckInDeviceId { get; set; }
        public Guid? CheckOutDeviceId { get; set; }

        public MachineDevice CheckInDevice { get; set; }
        public MachineDevice CheckOutDevice { get; set; }
        public Employee Employee { get; set; }
        public StandardHour StandardHour { get; set; }
        public ICollection<AttendanceLeave> Leaves { get; set; }


    }
}
