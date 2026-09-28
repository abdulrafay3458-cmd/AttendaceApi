namespace AttendanceAPI.Entities
{
    public class LeaveRequisitionMaster
    {
        public int Id { get; set; }
        public string DisplayId { get; set; }
        public string EmployeeCode { get; set; }
        public string Status { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string ApproverEmployeeCode { get; set; }
        public int? PreviousHajjYear { get; set; }
        public DateTime? RejectionDate { get; set; }
      //  public bool? WithoutPriorFlag { get; set; }

        public  Employee Employee { get; set; }
        public ICollection<LeaveRequisitionDetail> LeaveRequisitionDetails { get; set; }
        public ICollection<CancelLeave> CancelLeaves { get; set; }
        public ICollection<LeaveRequisitionBalance> LeaveRequisitionBalances { get; set; }
    }
}
