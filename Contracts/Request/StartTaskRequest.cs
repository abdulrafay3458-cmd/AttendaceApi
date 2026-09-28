namespace AttendanceAPI.Contracts.Request
{
    public class StartTaskRequest
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string TaskId { get; set; } = string.Empty;
    }
}
