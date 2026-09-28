namespace AttendanceAPI.Entities
{
    public class AttendanceLeave
    {
        public string EmployeeCode { get; set; }
        public int   Year { get; set; }
        public int   Month { get; set; }
        public int   Day { get; set; }
        public int   Fortnight { get; set; }
        public DateTime  Date { get; set; }
        public string TypeCode { get; set; }
        public string? Hours { get; set; }
        public int? Minutes {  get; set; }
        public int? RequisitionNumber {  get; set; }

        public Employee Employee { get; set; }
        public LeaveType LeaveType { get; set; }
        public AttendanceRecord AttendanceRecord { get; set; }


    }
}
