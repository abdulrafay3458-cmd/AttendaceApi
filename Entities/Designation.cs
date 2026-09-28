namespace AttendanceAPI.Entities
{
    public class Designation
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}
