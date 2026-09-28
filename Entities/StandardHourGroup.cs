namespace AttendanceAPI.Entities
{
    public class StandardHourGroup
    {
        public int Code { get; set; }
        public string Title { get; set; }
        
        public ICollection<Employee> Employees { get; set; }
        public ICollection<StandardHour> EmployeeStandardHour { get; set; }
    }
}
