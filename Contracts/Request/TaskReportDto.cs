namespace AttendanceAPI.Contracts.Request
{
    public class TaskReportDto
    {
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public bool IsLeave { get; set; }
        public List<TaskRecord> TaskList { get; set; }
    }

    public class TaskRecord
    {
        public string TaskTitle { get; set; }
        public string TaskStatus { get; set; }
        public double? TotalHoursWorked { get; set; }
    }
}
