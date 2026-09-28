namespace AttendanceAPI.Contracts.Response
{
    public class ZKTecoDevice
    {
        public Guid DeviceId { get; set; } = Guid.NewGuid();
        public string DeviceName { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public int Port { get; set; } = 4370;
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = "offline"; // online, offline, error
        public DateTime? LastSync { get; set; }
        public string CompanyCode { get; set; }
        public int MachineNumber { get; set; } = 1;
        public bool? isActive { get; set; }
        public bool? CanReadLogs { get; set; }
    }
}
