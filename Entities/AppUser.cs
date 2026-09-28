namespace AttendanceAPI.Entities
{
    public class AppUser
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string EmployeeCode { get; set; }
        public bool IsActive { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
        public Employee Employee { get; set; }
        public RefreshToken RefreshToken { get; set; }
        //public Task Assigner { get; set; }
        //public Task Assigned { get; set; }
    }
}
