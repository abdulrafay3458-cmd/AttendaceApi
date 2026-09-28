
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AttendanceAPI.Entities
{
    public class Employee
    {
        public string Code { get; set; }
        public string ZkUserId { get; set; }
        public string Name { get; set; }
        public string? LeadCode { get; set; }
        public string CompanyCode { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string DepartmentCode { get; set; }
        public int DesignationCode { get; set; }
        public int StandardHourCode { get; set; }
        public string? CardNumber { get; set; }
        public bool IsActive { get; set; }
        public string? FcmToken { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime? LeaveDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? FaceImage { get; set; }
        public bool IsAppUser { get; set; }
        public ICollection<AttendanceRecord> AttendanceRecord { get; set; }
        public StandardHourGroup StandardHourGroup { get; set; }
        public Company Company { get; set; }
      //  public ICollection<Notification> Notifications { get; set; }
        public ICollection<LeaveBalance> LeaveBalances { get; set; }
        public ICollection<AttendanceLeave> AttendanceLeaves { get; set; }
        public AppUser AppUser { get; set; }
        public Designation Designation { get; set; }
        public FaceImage EmployeeFaceImage { get; set; }
        public UserDevice EmpDevice { get; set; }
        public Department Department { get; set; }
        public ICollection<TaskTimeLogs> TaskTimeLogs { get; set; }
        public ICollection<RegistrationImage> RegistrationImage { get; set; }
        public ICollection<LeaveRequisitionMaster> LeaveRequisitionMasters { get; set; }
    }
}
