namespace AttendanceAPI.Entities
{
    public class TaskTimeLogs
    {
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public bool IsExtraWork { get; set; }
        public DateTime LogDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? StopTime { get; set; }
        public double? HoursWorked { get; set; }
        public string? Notes { get; set; }
        public string EmployeeCode { get; set; }
        public UserTask Tasks { get; set; }
        public Employee Employee { get; set; }
    }
}
