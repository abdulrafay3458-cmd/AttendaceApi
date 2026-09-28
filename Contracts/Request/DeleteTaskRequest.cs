namespace AttendanceAPI.Contracts.Request
{
    public class DeleteTaskRequest
    {
        public string TaskId { get; set; }
        public string LeadCode { get; set; }
    }
}
