namespace AttendanceAPI.Contracts.Response
{
    public class TeamMemberDto
    {
        public string EmployeeId { get; set; }
        public string MachineId { get; set; }
        public string Name { get; set; }
        public string CompanyCode { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Department { get; set; }
        public string LeadCode { get; set; }
        public string DesignationName { get; set; }
        public int DesignationCode { get; set; }
        public DateTime? JoinDate { get; set; }   // nullable
        public int StandardHourCode { get; set; }
        public bool isActive { get; set; }
        public string? Role { get; set; }
        public string? RoleId { get; set; }
        public bool IsAppUser { get; set; }
    }
}
