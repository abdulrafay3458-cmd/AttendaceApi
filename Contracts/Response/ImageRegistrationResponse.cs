namespace AttendanceAPI.Contracts.Response
{
    public class ImageRegistrationResponse
    {
            public Guid Code { get; set; }
            public string EmployeeCode { get; set; }
            public string EmployeeName { get; set; }
            public string AdminCode { get; set; }
            public DateTime RequestDate { get; set; }
            public DateTime? ApproveDate { get; set; }
            public DateTime? RejectionDate { get; set; }
            public string? RejectionReason { get; set; }
            public string FaceImage { get; set; }
            public bool Status { get; set; }
            //public bool HistoryStatus { get; set; }
            public string CompanyCode { get; set; }
            //public Employee Employee { get; set; }
        
    }
}
