namespace AttendanceAPI.Contracts.Response
{
    public class TaskExportDataModel
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string TaskId { get; set; } = string.Empty;
        public string TaskTitle { get; set; } = string.Empty;
        public DateTime LogDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? StopTime { get; set; }
        public double? HoursWorked { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string DayOfWeek { get; set; } = string.Empty;
        public int WeekNumber { get; set; }
        public string Month { get; set; } = string.Empty;
        public int Year { get; set; }
    }
}
