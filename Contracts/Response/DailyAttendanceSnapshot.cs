namespace AttendanceAPI.Contracts.Response
{
    public class DailyAttendanceSnapshot
    {
        public DateTime Date { get; set; }
        public List<EmployeeStatus> EmployeeStatuses { get; set; } = new();
        public int TotalPresent { get; set; }
        public int TotalAbsent { get; set; }
        public int TotalLate { get; set; }
        public int TotalOnLeave { get; set; }
    }
}
