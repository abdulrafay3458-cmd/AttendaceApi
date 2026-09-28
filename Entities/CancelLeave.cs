namespace AttendanceAPI.Entities
{
    public class CancelLeave
    {
        public Guid Id { get; set; }
        public string EmployeeCode { get; set; }
        //public DateTime FromDate { get; set; }
        //public DateTime ToDate { get; set; }
        public int LeaveId { get; set; }
        public DateTime LeaveDate { get; set; }
        public string ApproverEmployeeCode { get; set; }
        public string? Purpose { get; set; }
        public string State { get; set; }
        public DateTime SubmissionDate { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string? Reason { get; set; }
        public DateTime? RejectDate { get; set; }
        public LeaveRequisitionMaster Leave { get; set; }
    }
}
