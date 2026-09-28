namespace AttendanceAPI.Contracts.Response
{
    public class EmployeeLateSummaryResponse
    {
        public string EmployeeId { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string Department { get; set; } = "";
        public string ManagerId { get; set; } = "";
        public List<LateDateRecord> LateRecords { get; set; } = new();
        public int TotalLateMinutes => LateRecords.Sum(r => r.LateMinutes);
        public int LateDays => LateRecords.Count;
    }

    public class LateDateRecord
    {
        public DateTime Date { get; set; }
        public TimeSpan LateBy { get; set; }
        public int LateMinutes => (int)Math.Round(LateBy.TotalMinutes);
        public DateTime? CheckInTime { get; set; }
    }

}
