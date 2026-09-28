namespace AttendanceAPI.Contracts.Request
{
    public class DeviceRequest
    {
        public string? DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string IpAddress { get; set; }
        public int PortNumber { get; set; }
        public int MachineNumber { get; set; }
        public string? Status { get; set; }
        public string Location { get; set; }
        public bool IsActive { get; set; }
        public bool CanReadLogs { get; set; }
        public string CompanyCode { get; set; }
    }
}
