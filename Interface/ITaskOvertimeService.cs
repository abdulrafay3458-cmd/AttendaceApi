using AttendanceAPI.Contracts.Request;
using AttendanceAPI.Entities;

namespace AttendanceAPI.Interface
{
    public interface ITaskOvertimeService
    {
        Task<TaskOvertime> AddOvertimeAsync(AddOvertimeRequest request,string employeeId);
        Task<bool> UpdateOvertimeAsync(AddOvertimeRequest request);
        Task<List<TaskOvertime>> GetMyOvertimeAsync(Guid userTaskId);
        Task<List<TaskOvertime>> GetTaskOvertimeAsync(Guid userTaskId);
        Task<string> DeleteOvertimeAsync(Guid overtimeId);
        Task<List<OvertimeSummaryDto>> GetPendingRequest(string leadCode);
        Task<(bool Success, string? Error)> ApprovedOvertimeRequestAsync(string leadCode , Guid TaskId,DateTime overTimeDate );
        Task<(bool Success, string? Error)> RejectOvertimeRequestAsync(string leadCode , Guid TaskId,string reason,DateTime overTimeDate);


    }
}
