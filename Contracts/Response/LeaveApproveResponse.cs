namespace AttendanceAPI.Contracts.Response
{
    public class LeaveApproveResponse
    {
        public string EmployeeCode { get; set; }
        public int Id { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string LeaveType { get; set; }
        public string AdjustmentType { get; set; }
        public decimal NoOfDays { get; set; }
    }
}
