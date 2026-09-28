namespace AttendanceAPI.Entities
{
    public class RoleType
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool IsHidden { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
    }
}
