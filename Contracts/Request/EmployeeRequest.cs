namespace AttendanceAPI.Contracts.Request
{
    public class EmployeeRequest
    {
        public string ManagerId { get; set; }
        public string EmployeeId { get; set; }
        public string ZkUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CompanyCode { get; set; }
        public string Phone { get; set; }
        public string DepartmentCode { get; set; }
        public int DesignationCode { get; set; }
        public int StandardHour { get; set; }
        public string? Designation { get; set; }
        //public string CardNumber { get; set; } //
        //public string EmployeeStatus { get; set; }        
        //public string FcmToken { get; set; } //
        public IList<Guid>? RoleId { get; set; }
        public bool IsAppUser { get; set; }
        public bool IsActive { get; set; }
        public DateTime JoinDate { get; set; }
    }
}
