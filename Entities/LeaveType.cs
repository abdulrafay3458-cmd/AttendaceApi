namespace AttendanceAPI.Entities
{
    public class LeaveType
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public int? NoOfDays { get; set; }

        public ICollection<AttendanceLeave> Leaves { get; set; }
    }
}
