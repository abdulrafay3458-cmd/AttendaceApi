namespace AttendanceAPI.Contracts.Request
{
    public class ManualAttendanceRequest
    {
        public string EmployeeCode { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
    }
}
