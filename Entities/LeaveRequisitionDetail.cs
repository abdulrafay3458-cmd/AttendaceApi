namespace AttendanceAPI.Entities
{
    public class LeaveRequisitionDetail
    {
        public int Id { get; set; }
        public string DisplayId { get; set; }
        public int MasterId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string LeaveTypeCode { get; set; }
        public decimal NoOfDays { get; set; }
        public string? Purpose { get; set; }
        public string? AdjustmentType { get; set; }
        public int PurposeCode { get; set; }
        public string? RejectionReason { get; set; }

        public LeaveRequisitionMaster LeaveRequisitionMaster { get; set; }
    }
}
