
namespace AttendanceAPI.Entities
{
    public class StandardHour
    {
        public int GroupCode { get; set; }
        public int WeekDay { get; set; }
        public string WeekDayName { get; set; }
        public string Hours { get; set; }
        public int Minutes { get; set; }
        public string? TimeIn { get; set; }
        public string? TimeOut { get; set; }
        public int? HalfDayMinutes { get; set; }
        public string? HalfDayHourOnLateArrival { get; set; }
        public StandardHourGroup StandardHourGroup { get; set; }

        public virtual ICollection<AttendanceRecord> AttendanceRecords { get; set; }
    }
}
