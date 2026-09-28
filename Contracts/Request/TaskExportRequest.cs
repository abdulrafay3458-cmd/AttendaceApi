namespace AttendanceAPI.Contracts.Request
{
    public class TaskExportRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ManagerId { get; set; }
        public string? EmployeeId { get; set; }
        public string ViewType { get; set; }
    }
}
