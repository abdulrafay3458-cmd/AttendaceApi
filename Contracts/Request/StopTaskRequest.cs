namespace AttendanceAPI.Contracts.Request
{
    public class StopTaskRequest
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string TaskId { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
