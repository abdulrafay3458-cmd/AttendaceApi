using AttendanceAPI.Contracts.Response;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceAPI.Interface
{
    public interface IReportService
    {
        Task<AttendanceReport> GenerateDailyReportAsync(DateTime startDate, DateTime endDate, string? managerId);
        Task<AttendanceReport> GenerateCheckInReportAsync(DateTime startDate, DateTime endDate, string? managerId);
        Task<AttendanceReport> GenerateWeeklyReportAsync(DateTime reportDate, string managerId);
        Task<AttendanceReport> GenerateMonthlyReportAsync(int reportYear, int reportMonth, string managerId);
        Task<AttendanceReport> GenerateYearlyReportAsync(int reportYear, string managerId);
        Task<AttendanceReport> GenerateMonthlyReportEmployeeAsync(DateTime startDate, DateTime endDate, string employeeId);
        Task<AttendanceReport> GenerateYearlyReportEmployeeAsync(int reportYear, string employeeId);
        Task<DailyAttendanceSnapshot> GetTodaySnapshotAsync(string managerId);
        Task<List<EmployeeStatus>> GetLateEmployeesTodayAsync(string managerId);
        Task<List<EmployeeLateSummaryResponse>> GetLateEmployeesRangeGroupedAsync(string? managerId, DateTime startDate, DateTime endDate);
        Task<List<EmployeeAbsentSummaryResponse>> GetAbsentEmployeesAsync(string? managerId, DateTime startDate, DateTime endDate);
        Task<byte[]> GetExportWorkByManager(DateTime date, string reportType, string managerId);
        Task<byte[]> GetExcelCheckInReport(DateTime startDate, DateTime endDate, string reportType, string managerId);
        //Task<byte[]> GetExportWorkByEmployee(DateTime date, string reportType, string emp);
        Task<byte[]> GetExportWorkByEmployee(DateTime startDate, DateTime endDate, string reportType, string empId);
    }
}
