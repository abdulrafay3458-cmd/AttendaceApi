namespace AttendanceAPI.Entities
{
    public class UserRole
    {
        public string UserCode { get; set; }
        public Guid RoleId { get; set; }
        public RoleType UserRolesTypes { get; set; }
        public AppUser User { get; set; }
    }
}
