namespace AttendanceAPI.Contracts.Response
{
        public class EntityResponse
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string SuitNo { get; set; }
        public string SteetNO { get; set; }
        public string PostalCode { get; set; }
        public string Town { get; set; }
        public string LegalName { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? IndustryCode { get; set; }
        public string? Fax { get; set; }
        public string? ShortName { get; set; }
        public string? CountryCode { get; set; }
        public string? GlCode { get; set; }
        public string? FiscalYear { get; set; }
        public string? ParentCode { get; set; }
        public string? ChildCode { get; set; }
        public bool? Status { get; set; }
        public bool? Vendor { get; set; }
    }

}
