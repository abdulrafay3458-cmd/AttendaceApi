namespace AttendanceAPI.Contracts.Response
{
    public class DayWise
    {
        public string StandardHours { get; set; }
        public string OverTime { get; set; }
        public string WorkingHours { get; set; }
        public bool IsLate { get; set; }
    }
}
