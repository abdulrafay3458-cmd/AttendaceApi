namespace AttendanceAPI.Contracts.Request
{
    public class SmtpSettings
    {
        public string ServerOutgoing { get; set; }
        public string ServerIncoming { get; set; }
        public int Port { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string Password { get; set; }
        public int IncomingPort { get; set; }
    }
}
