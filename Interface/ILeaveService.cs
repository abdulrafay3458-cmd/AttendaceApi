using AttendanceAPI.Contracts.Request;
using AttendanceAPI.Contracts.Response;
using AttendanceAPI.Entities;

namespace AttendanceAPI.Interface
{
    public interface ILeaveService
    {
        Task<IList<LeaveRequestResponse>> GetLeaveRequestByIdAsync(string employeeId);
        Task<LeaveRejectResponse> GetLeaveRequestForRejectionByIdAsync(int Id, RejectLeaveRequest request);
        Task<Employee> GetEmployeeByIdAsync(string Id);
        Task<LeaveRequestResponse> Applyleaves(ApplyLeaveRequest applyLeaveRequest);
        Task<List<LeaveTypeResponse>> GetLeaveTypes();
        Task<IList<LeaveRequestResponse>> GetPendingLeaveRequestsForApproverAsync(string managerId);
        Task<LeaveRequestResponse> GetApproveLeaveByIdAsync(int Id, ApproveLeaveRequest request);
        Task<IList<LeaveRequestResponse>> GetLeaveRequestsByEmployeeAsync(string approverId);
        Task<bool> GetCheckTodayLeave(string employeeId, string companyCode);
        Task<LeaveStatsResponse> GetLeaveStatisticsAsync(string employeeCode);
        Task<CancelLeave> CancelLeave(CancelLeaveRequest cancelLeave);
        Task<CancelLeave> ApproveCancelLeaves(string id, string approverId);
        Task<IList<CancelLeaveResponse>> GetPendingCancelLeaves(string approverId);
        Task<List<DateTime>> GetCancelLeaveDatesAsync(int no);
        Task<CancelLeave> GetCancelLeaveRequestForRejectionByIdAsync(string Id, RejectLeaveRequest request);
    }
}
