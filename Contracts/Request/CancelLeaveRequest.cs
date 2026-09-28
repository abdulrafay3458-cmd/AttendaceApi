namespace AttendanceAPI.Contracts.Request
{
    public class CancelLeaveRequest
    {
        public string EmployeeId { get; set; }
        public int LeaveId { get; set; }
        public List<string> Dates { get; set; } = new List<string>();
        //public DateTime StartDate { get; set; }
        //public DateTime EndDate { get; set; }
        public string? Purpose { get; set; }
    }
}
