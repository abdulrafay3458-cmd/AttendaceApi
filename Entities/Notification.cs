namespace AttendanceAPI.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string UserCode { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; } // attendance, leave, alert
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CheckType { get; set; } // CheckIn / CheckOut
        public DateTime? CheckTime { get; set; }
        public string? Location { get; set; } // nullable

      //  public ICollection<Employee> Employee { get; set; }
    }
}
