namespace AttendanceAPI.Contracts.Request
{
    public class EmployeeDTO
    {
        public string employeeId { get; set; }
        public string employeeCode { get; set; }
        public string CompanyCode { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string department { get; set; }
        public string designation { get; set; }
        public string faceImageBase64 { get; set; }
        public List<string> role { get; set; }

        public bool? imageApproved { get; set; }
        public bool? isDeviceRegistered { get; set; }
        public bool? AnyDeviceRegistered { get; set; }
    }
}
