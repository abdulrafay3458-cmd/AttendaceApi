namespace AttendanceAPI.Contracts.Request
{
    public class ForgetPasswordRequest
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string OTP { get; set; }

    }
}
