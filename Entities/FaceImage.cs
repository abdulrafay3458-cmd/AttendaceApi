namespace AttendanceAPI.Entities
{
    public class FaceImage
    {
        public Guid Id { get; set; }
        public string EmpCode { get; set; }
        public string? EmpFaceImage { get; set; }

        public Employee Employee { get; set; }
    }
}
