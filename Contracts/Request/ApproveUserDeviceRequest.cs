namespace AttendanceAPI.Contracts.Request
{
    public class ApproveUserDeviceRequest
    {
        public string requestId { get; set; }
        public bool isApprove { get; set; }
        public string? reason { get; set; }
    }
}
