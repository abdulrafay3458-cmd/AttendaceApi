namespace AttendanceAPI.Contracts.Response
{
    public class PendingTask
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; } // string in HH:mm format
        public string Priority { get; set; } = "Low";
        public DateTime AssignedDate { get; set; }
    }
}
