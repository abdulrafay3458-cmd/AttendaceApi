namespace AttendanceAPI.Entities
{
    public class LeaveRequisitionBalance
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
        public int MasterId { get; set; }
        public decimal CurrentBalance { get; set; }

        public LeaveRequisitionMaster LeaveRequisitionMaster { get; set; }
    }
}
