using AttendanceAPI.Contracts.Request;
using AttendanceAPI.Contracts.Response;
using AttendanceAPI.Entities;
using System;

namespace AttendanceAPI.Interface
{
    public interface IAttendenceServices
    {
        Task<List<AttendanceRecord>> GetAttendence(string Employeecode);
        Task<List<AttendanceHistory>> GetEmployeeAttendanceAsync(string employeeId, int days);
        Task<List<AttendanceRecord>> GetAttendanceByEmployeeAndDateRangeAsync(string employeeId, DateTime startDate, DateTime endDate);
        Task<bool> AttendenceExisitAsync(string empCode, DateTime date);
        Task<AttendanceRecord> GetTodayAttendanceAsync(string employeeId);
        Task<ChartDataResponse> ChartRecords(DateTime dateTime, string empCode, DateTime endDate);
        Task<AttendanceRecord> AddLeaveDay(AttendanceRecord record);
        Task<DayWise> DayWiseStandardHour(string empCode, DateTime dateTime);
        Task<AttendanceRecord> MarkManualCheckIn(ManualAttendanceRequest request);
        Task<AttendanceRecord> MarkManualCheckOut(ManualAttendanceRequest request);
    }
}
