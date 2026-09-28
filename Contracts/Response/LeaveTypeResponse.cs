namespace AttendanceAPI.Contracts.Response
{
    public class LeaveTypeResponse
    {
        public string LeaveCode { get; set; }
        public string LeaveTitle { get; set; }
        public int?  LeaveDays { get; set; }
    }
}
