namespace AttendanceAPI.Entities
{
    public class TaskOvertime
    {
        public Guid Id { get; set; }

        public Guid UserTaskId { get; set; }

        public string EmployeeId { get; set; }

        public DateTime OvertimeDate { get; set; }

        public double OvertimeHours { get; set; }

        public string? Reason { get; set; }

        // pending, approved, rejected
        public string Status { get; set; } = "pending";

        public string? ApprovedBy { get; set; }

        public DateTime? ApprovedAt { get; set; }
        public string? RejectionBy { get; set; }
        public string? RejectionReason { get; set; }

        public DateTime CreatedAt { get; set; }

        public UserTask UserTask { get; set; }        
    }

}
