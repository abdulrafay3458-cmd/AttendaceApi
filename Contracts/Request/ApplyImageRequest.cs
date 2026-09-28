namespace AttendanceAPI.Contracts.Request
{
    public class ApplyImageRequest
    {
       // public Guid Code { get; set; }
        public string EmployeeCode { get; set; }
        public string FaceImage { get; set; }   
        public string CompanyCode { get; set; }
    }
}
