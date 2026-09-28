namespace AttendanceAPI.Contracts.Request
{
    public class AssignTaskRequest
    {
        public string AssignedBy { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public string TaskTitle { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        public string TaskPreference { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public string Priority { get; set; } = "medium";
        public string? Client { get; set; }
    }

    public class AssignTaskMultipleRequest
    {
        public string AssignedBy { get; set; }
        public List<string> AssignedTo { get; set; } // Multiple users
        public string TaskTitle { get; set; }
        public string TaskDescription { get; set; }
        public string DueDate { get; set; }
        public string Priority { get; set; }
        public string Client { get; set; }
        public string TaskPreference { get; set; }
    }
}
