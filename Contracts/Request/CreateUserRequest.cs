namespace AttendanceAPI.Contracts.Request
{
    public class CreateUserRequest
    {
        public string? ZkUserId { get; set; }
        public string? CompanyCode { get; set; }
        public string? EmployeeCode { get; set; }

        public string? Email { get; set; }
        public string? Phone { get; set; }

        public string? Department { get; set; }

        public int DesignationCode { get; set; }
        public int StandardHour { get; set; }

        public string? CardNumber { get; set; }

        public bool IsActive { get; set; }
        public bool IsAppUser { get; set; }

        public DateTime JoinDate { get; set; }

        public string? ManagerId { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? Designation { get; set; }

        public string? FcmToken { get; set; }

        public string? Role { get; set; }
        public string? RoleId { get; set; }
    }

}
