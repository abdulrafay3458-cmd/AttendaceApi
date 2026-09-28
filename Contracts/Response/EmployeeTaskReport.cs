namespace AttendanceAPI.Contracts.Response
{
    public class EmployeeTaskReport
    {
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public List<TaskReport> Tasks { get; set; }
    }
}
