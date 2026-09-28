namespace AttendanceAPI.Entities
{
    public class MachineDevice
    {
        public Guid Id { get; set; }
        public string CompanyCode { get; set; }
        public string Name { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string Location { get; set; }
        public string Status { get; set; } // online, offline, error
        public DateTime? LastSync { get; set; }
        public int MachineNumber { get; set; }
        public bool IsActive { get; set; }
        public bool CanReadLogs { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Company OwnerCompany { get; set; }
        public ICollection<AttendanceRecord> DeviceForCheckIn { get; set; }
        public ICollection<AttendanceRecord> DeviceForCheckOut { get; set; }
    }
}
