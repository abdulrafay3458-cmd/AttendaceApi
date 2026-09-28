namespace AttendanceAPI.Contracts.Request
{
    public class RejectLeaveRequest
    {
        public string ApproverId { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}
