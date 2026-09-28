namespace AttendanceAPI.Entities
{
    public class RefreshToken
    {
        public string UserCode { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public AppUser user { get; set; }
    }
}
