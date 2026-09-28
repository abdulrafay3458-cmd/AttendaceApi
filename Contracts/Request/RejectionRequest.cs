namespace AttendanceAPI.Contracts.Request
{
    public class RejectionRequest
    {
        public int ApproverId { get; set; }
        public string Reason { get; set; }
    }
}
