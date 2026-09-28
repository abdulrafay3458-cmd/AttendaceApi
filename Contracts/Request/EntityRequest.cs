namespace AttendanceAPI.Contracts.Request
{
    public class EntityRequest
    {
        public string entityCode { get; set; }
        public string entityName { get; set; }
        public string entityLegalName { get; set; }
        public string entityShortName { get; set; }
        public string entityCountryCode { get; set; }
        public string entitySuitNo { get; set; }
        public string entitySteetNO { get; set; }
        public string entityPostalCode { get; set; }
        public string entityTown { get; set; }
        public string entityProvince { get; set; }
        public string entityCountry { get; set; }
        public string entityEmail { get; set; }
        public string entityPhone { get; set; }
        public string? entityIndustryCode { get; set; }
        public string? entityFax { get; set; }
        public string? entityGlCode { get; set; }
        public string? entityFiscalYear { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public string? entityParentCode { get; set; }
        public string? entityChildCode { get; set; }
        public bool? entityVendor { get; set; }
        public bool? entityStatus { get; set; }
    }
}
