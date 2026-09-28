using System.Reflection.Metadata;

namespace AttendanceAPI.Entities
{
    public class LeaveBalance
    {
        public string EmployeeCode { get; set; }
        public int Period { get; set; }
        public decimal? Balance { get; set; }
        public string LeaveType { get; set; }
        public Employee Employee { get; set; }
    }
}
