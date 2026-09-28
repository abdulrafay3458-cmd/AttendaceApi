using System.Drawing;
using AttendanceAPI.Contracts.Request;
using AttendanceAPI.Contracts.Response;
using Azure.Core;
using Org.BouncyCastle.Asn1.Ocsp;

namespace AttendanceAPI.Interface
{
    public interface ITaskService
    {
        Task<TaskResponse> AssignTaskAsync(AssignTaskRequest request);
        Task<List<TaskResponse>> AssignTaskToMultipleAsync(AssignTaskMultipleRequest request);
        Task<List<EmployeeTaskReport>> GenerateTaskReportAsync(string reportType, DateTime startDate, DateTime endDate, string? managerId);
        Task<List<TaskExportDataModel>> GetTaskReportDataForExport(DateTime startDate, DateTime endDate, string? managerId, string? employeeId = null);
        Task<List<TaskResponse>> GetEmployeeTasksAsync(string userCode);
        Task<List<TaskResponse>> GetActiveEmployeeTasksAsync(string userCode);
        Task<List<PendingTask>> GetPendingTasks(string userId);
        Task<List<TaskResponse>> GetTeamLeadTasksAsync(string teamLeadCode);
        Task<List<EmployeeTaskReport>> GenerateTaskReport(DateTime startDate, DateTime endDate, string? managerId);
        Task<List<EmployeeTaskReport>> GenerateTaskYearlyReportAsync(int year, string? managerId);
        Task<List<EmployeeTaskReport>> GenerateTaskMonthlyReportAsync(int year, int month, string? managerId);
        byte[] GenerateExcelReport(List<TaskExportDataModel> data, string viewType, DateTime startDate, DateTime endDate, string? employeeName = null);
        Task<List<EmployeeTaskReport>> GenerateTaskDailyReportAsync(DateTime date, string? managerId);
        Task<TaskResponse> UpdateTaskAsync(UpdateTaskRequest request);
        Task DeleteTaskAsync(DeleteTaskRequest request);
        Task<TaskResponse?> GetTaskByIdAsync(Guid taskId);
        Task<TaskResponse> StartTaskAsync(string employeeId, string taskId);
        Task<TaskResponse> StopTaskAsync(string employeeId, string taskId, string notes);
        Task<TaskResponse> CompleteTaskAsync(string employeeId, string taskId);
        Task<TaskSummaryResponse> GetEmpTaskSummaryAsync(string empCode);
        Task CheckPreviousDayTaskDuration(string empId);
        //(DateTime startDate, DateTime endDate) CalculateDateRange(TaskExportRequest request);
        string DetermineViewType(DateTime StartDate, DateTime EndDate);
        string GenerateFileName(TaskExportRequest request, string? employeeName);







    }
}
