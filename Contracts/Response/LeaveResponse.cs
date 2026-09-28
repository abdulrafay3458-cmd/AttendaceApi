namespace AttendanceAPI.Contracts.Response
{
    public class LeaveResponse
    {
        public string EmployeeCode { get; set; }
        public string ManagerCode { get; set; }
        public string ManagerName { get; set; }
        public string EmployeeName { get; set; }
    }
}
