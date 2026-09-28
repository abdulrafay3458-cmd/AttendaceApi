namespace AttendanceAPI.Entities
{
    public class Company
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string? NTN { get; set; }
        public bool IsTaxApplicable { get; set; }
        public bool IsTaxExemption { get; set; }
        public string? GlCode { get; set; }
        public string? FunctionalCurrency { get; set; }
        public string? DefaultBankAccountCode { get; set; }
        public string? DefaultBankCode { get; set; }
        public ICollection<Department> Departments { get; set; }
        public ICollection<Employee> Employees { get; set; } 
        public ICollection<MachineDevice> CompanyDevices { get; set; } 
    }
}
