namespace AttendanceAPI.Contracts.Request
{
    public class UpdateTaskRequest
    {
        public Guid TaskId { get; set; } = Guid.Empty;
        public string AssignedBy { get; set; } = string.Empty;
        public List<string> AssignedTo { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        public string TaskPreference { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public string Priority { get; set; } = "medium";
        public string? ClientCode { get; set; }
    }
}
