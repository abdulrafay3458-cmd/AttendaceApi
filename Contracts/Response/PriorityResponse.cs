namespace AttendanceAPI.Contracts.Response
{
    public class PriorityResponse
    {
        public string Title { get; set; } = "";
        public int MinHours { get; set; }
        public int MaxHours { get; set; }
    }
}
