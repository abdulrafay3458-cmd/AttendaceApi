using AttendanceAPI.Contracts.Request;
using AttendanceAPI.Contracts.Response;
using AttendanceAPI.Data;
using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Interface
{
    public interface IEmployeeService
    {
        Task<TaskReportDto> GetEmployeeTodayRecord(string employeeCode, DateTime date);
        Task<List<Employee>> GetEmployeesAsync();
        Task<Employee?> GetEmployeeByIdAsync(string employeeId);
        Task<EmployeeDTO?> GetEmployeeCurrentInfo(string employeeId);
        Task<List<TeamMemberDto>> GetTeamMembersAsync(string managerId);
        Task<Employee> UpdateEmployee(EmployeeRequest employee);
        Task<Employee?> GetEmployeeByZkUserIdAsync(string zkUserId);
        Task<List<TeamLead>> GetTeamLeadsAsync();
        Task<bool> GetImageStatusAsync(string employeeId);
        Task<Dictionary<string, string>> RegisterUserDeviceAsync(RegisterUserDeviceRequest request);
        Task<bool> ApproveUserDeviceAsync(ApproveUserDeviceRequest request, string approvedBy);
        Task<List<UserDevicesResponse>> GetRequestsForNewUserDevicesAsync();
        Task<List<UserDevicesResponse>> GetAllUsersWithTheirDevicesAsync();
        Task<bool> RemoveCurrentUserDeviceAsync(string EmpId);
        Task<List<WorkingDays>> GetWorkingDays(string employeeCode);
        Task<bool> IsAuthorizeUserAndDevice(string employeeCode, string deviceHash);
    }
}
