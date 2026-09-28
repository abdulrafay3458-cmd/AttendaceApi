
namespace AttendanceAPI.Contracts.Response
{
    public class AttendanceReport
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ReportType { get; set; } = ""; // daily, weekly, monthly, yearly
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<EmployeeAttendanceSummary> EmployeeSummaries { get; set; } = new();
        public ReportStatistics Statistics { get; set; } = new();
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
        public string GeneratedBy { get; set; } = "";
    }
    public class ReportStatistics
    {
        public int TotalEmployees { get; set; }
        public double OverallAttendanceRate { get; set; }
        public double AverageLateRate { get; set; }
        public int TotalAbsences { get; set; }
        public int TotalLateArrivals { get; set; }
        public int TotalOnTime { get; set; }
    }
}
