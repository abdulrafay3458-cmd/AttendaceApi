namespace AttendanceAPI.Entities
{
    public class UserDevice
    {
        public string EmployeeCode { get; set; }
        public string DeviceHash { get; set; }
        public string? DeviceManufacturer { get; set; }
        public string? DeviceModel { get; set; }
        public bool? isApproved { get; set; }
        public DateTime? RequestedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string? RejectedBy { get; set; }
        public string? RejectedReason { get; set; }

        public Employee DeviceEmployee { get; set; }
    }
}
