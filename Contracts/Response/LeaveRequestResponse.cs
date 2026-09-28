namespace AttendanceAPI.Contracts.Response
{
    public class LeaveRequestResponse
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string LeaveType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalDays { get; set; }
        public string? Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "pending"; // pending, approved, rejected

        // APPROVAL WORKFLOW
        public string ApproverManagerId { get; set; } = string.Empty; // Who should approve
        public string ApproverManagerName { get; set; } = string.Empty;
        public string ApprovedBy { get; set; } = string.Empty; // Who actually approved
        public DateTime? ApprovedAt { get; set; }
        public string? RejectionReason { get; set; } = string.Empty;
        public List<DateTime> AvailableDates { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
