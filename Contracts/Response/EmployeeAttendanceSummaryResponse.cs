
namespace AttendanceAPI.Contracts.Response
{
    public class EmployeeAttendanceSummary
    {
        public string EmployeeId { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string Department { get; set; }
        public string Designation { get; set; }
        public int TotalDays { get; set; }
        public int PresentDays { get; set; }
        public int LateDays { get; set; }
        public int AbsentDays { get; set; }
        public int LeaveDays { get; set; }
        public double TotalHoursWorked { get; set; }
        public double AverageHoursPerDay { get; set; }
        //public DateTime CheckInTime { get; set; }
        //public DateTime CheckOutTime { get; set; }
        //public string CheckInSource { get; set; }
        //public string CheckOutSource { get; set; }
        //public string CheckInAddress { get; set; }
        //public string CheckOutAddress { get; set; }
        //public string AttendanceStatus { get; set; }
        public int OnTimeDays { get; set; }
        public List<LateRecord> LateRecords { get; set; } = new();
        public List<PresentRecord> PresentRecords { get; set; } = new();
        public List<AbsentRecord> AbsentRecords { get; set; } = new();
        public List<CheckInRecordsGroup> CheckInRecords { get; set; } = new();

    }
    public class AbsentRecord
    {
        public DateTime Date { get; set; }
        public string Reason { get; set; } = "";
        public bool OnLeave { get; set; }
    }
    public class LateRecord
    {
        public DateTime Date { get; set; }
        public TimeSpan CheckInTime { get; set; }
        public TimeSpan LateBy { get; set; }
        public string Reason { get; set; } = "";
    }
    public class PresentRecord
    {
        public DateTime Date { get; set; }
        public bool IsOnTime { get; set; }
    }

    public class CheckInRecordsGroup
    {
        public DateTime Date { get; set; }
        public List<CheckInRecords> Records { get; set; } = new();
    }

    public class CheckInRecords {
        public DateTime Date { get; set; }
        public DateTime? CheckInTime { get; set; }
        public string? Source { get; set; }
        public string? Location { get; set; }
        public DateTime? CheckOutTime { get; set; }
        //public string? CheckOutSource { get; set; }
        //public string? CheckOutLocation { get; set; }
        //public string? Status { get; set; }
    }
}
