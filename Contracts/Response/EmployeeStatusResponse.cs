namespace AttendanceAPI.Contracts.Response
{
    public class EmployeeStatus
    {
        public string EmployeeId { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string Department { get; set; }
        public string Status { get; set; } = ""; // present, absent, late, on-leave
        public DateTime? CheckInTime { get; set; }
        public TimeSpan? LateBy { get; set; }
        public string ManagerId { get; set; } = "";
    }
}
