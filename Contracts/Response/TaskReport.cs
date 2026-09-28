namespace AttendanceAPI.Contracts.Response
{
    public class TaskReport
    {
        public string Taskid { get; set; }
        public string TaskTitle { get; set; }
        public string TaskStatus { get; set; }
        public string TaskPriority { get; set; }
        public DateTime? TaskDueDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public List<TaskTimeLogResponse> TimeLogs { get; set; } = new();
    }
}