namespace AttendanceAPI.Contracts.Response
{
    public class DeviceResponse
    {
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string Location { get; set; }
        public string Status { get; set; } // online, offline, error
        public DateTime? LastSync { get; set; }
        public int MachineNumber { get; set; }
    }
}
