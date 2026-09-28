namespace AttendanceAPI.Contracts.Request
{
    public class ChangePasswordRequest
    {
        public string? RequesterCode { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
