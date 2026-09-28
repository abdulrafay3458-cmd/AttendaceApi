namespace AttendanceAPI.Entities
{
    public class Entity
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string LegalName { get; set; } = null!;
        public string ShortName { get; set; }
        public string CountryCode { get; set; }
        public string? Address { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public string? Phone { get; set; } = null!;
        public string? IndustryCode { get; set; }
        public string? Fax { get; set; }
        public string? GlCode { get; set; }
        public string? FiscalYear { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public string? ParentCode { get; set; }
        public string? ChildCode { get; set; }
        public bool? Status { get; set; }
        public bool? ClientVendor { get; set; }
        public int? AllowedRadiusMeters { get; set; }
        public ICollection<UserTask> UserTasks { get; set; }
    }
}
