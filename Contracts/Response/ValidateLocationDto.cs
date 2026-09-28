namespace AttendanceAPI.Contracts.Response
{
    public class ValidateLocationDto
    {
        public Guid TaskId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
