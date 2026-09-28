namespace AttendanceAPI.Entities
{
    public class Department
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string? LeadCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime? InactiveFromDate { get; set; }
        public string CompanyCode { get; set; }

        public ICollection<Employee> Employees { get; set; }
        public Company Company { get; set; }
    }
}
