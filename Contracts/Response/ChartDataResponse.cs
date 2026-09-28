namespace AttendanceAPI.Contracts.Response
{
    public class ChartDataResponse
    {
        public string Type { get; set; }
        public int OnTime { get; set; }
        public int Late { get; set; }
    }
}
