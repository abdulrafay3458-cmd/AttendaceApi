using AttendanceAPI.Contracts.Request;
using AttendanceAPI.Data;
using AttendanceAPI.Entities;
using AttendanceAPI.Interface;
using DocumentFormat.OpenXml.InkML;
using Google;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Services
{
    public class TaskOvertimeService : ITaskOvertimeService
    {
        private readonly AppDbContext _context;

        public TaskOvertimeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TaskOvertime> AddOvertimeAsync(
            AddOvertimeRequest request,
            string employeeId)
        {

            var task = await _context.Tasks
                .FirstOrDefaultAsync(x =>
                    x.Id == request.UserTaskId &&
                    x.IsActive &&
                    !x.IsDelete);

            if (task == null)
            {
                throw new Exception("Task not found.");
            }
            

            if (request.OvertimeHours <= 0)
            {
                throw new Exception(
                    "Overtime hours must be greater than zero.");
            }
            //var existingRecord = _context.TaskOvertimes.FirstOrDefault(a => a.UserTaskId == request.UserTaskId && a.OvertimeDate == request.OvertimeDate);
           
            //if (existingRecord != null)
            //{
            //    throw new Exception(
            //        "Overtime hours already exisit.");
            //}
            var overtime = new TaskOvertime
            {
                Id = Guid.NewGuid(),

                UserTaskId = request.UserTaskId,

                EmployeeId = task.AssignedTo,

                OvertimeDate = request.OvertimeDate,

                OvertimeHours = request.OvertimeHours,

                Reason = request.Reason,

                Status = "pending",

                CreatedAt = DateTime.UtcNow
            };

            _context.TaskOvertimes.Add(overtime);

            await _context.SaveChangesAsync();

            return overtime;
        }

        public async Task<List<TaskOvertime>> GetMyOvertimeAsync(
            Guid TaskId)
        {
            return await _context.TaskOvertimes
                .Include(x => x.UserTask)
                //.Include(x => x.Employee)
                .Where(x => x.UserTaskId == TaskId)
                .OrderByDescending(x => x.OvertimeDate)
                .ToListAsync();
        }

        public async Task<List<TaskOvertime>> GetTaskOvertimeAsync(
            Guid userTaskId)
        {
            return await _context.TaskOvertimes
               // .Include(x => x.Employee)
                .Include(x => x.UserTask)
                .Where(x => x.UserTaskId == userTaskId)
                .OrderByDescending(x => x.OvertimeDate)
                .ToListAsync();
        }

        public async Task<bool> UpdateOvertimeAsync(AddOvertimeRequest request)
        {
            var overtime = await _context.TaskOvertimes
                .FirstOrDefaultAsync(a =>
                    a.UserTaskId == request.UserTaskId &&
                    a.OvertimeDate == request.OvertimeDate);

            if (overtime == null)
            {
                return false;
            }

            overtime.OvertimeHours = request.OvertimeHours;
            overtime.Reason = request.Reason;
            _context.TaskOvertimes.Update(overtime);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<string> DeleteOvertimeAsync(Guid overtimeId)
        {
            var overtime = await _context.TaskOvertimes
                .FirstOrDefaultAsync(x => x.Id == overtimeId);

            if (overtime == null)
            {
                return "Overtime record not found.";
            }

            _context.TaskOvertimes.Remove(overtime);

            await _context.SaveChangesAsync();

            return "Overtime deleted successfully.";
        }

        //public async Task<List<AddOvertimeRequest>> GetPendingRequest(string leadCode)
        //{
        //    var userTasks = await _context.Tasks.Where(a => a.AssignedBy == leadCode && a.IsDelete == false && a.CompletedAt == null).ToListAsync();

        //    return (from overtime in _context.TaskOvertimes
        //            join task in userTasks on overtime.UserTaskId equals task.Id
        //            join emp in _context.Employees on overtime.EmployeeId equals emp.Code
        //            where overtime.Status == "pending"
        //            select new AddOvertimeRequest
        //            {
        //                EmployeeId = overtime.EmployeeId,
        //                EmployeeName = emp.Name,
        //                OvertimeDate = overtime.OvertimeDate,
        //                OvertimeHours = overtime.OvertimeHours,
        //                Reason = overtime.Reason

        //            }).ToList();
        //}
        public async Task<List<OvertimeSummaryDto>> GetPendingRequest(string leadCode)
        {
            var pendingOvertimes = await (
                from overtime in _context.TaskOvertimes
                join task in _context.Tasks
                    on overtime.UserTaskId equals task.Id
                join employee in _context.Employees
                    on overtime.EmployeeId equals employee.Code
                where overtime.Status == "pending"
                      && task.AssignedBy == leadCode
                      && !task.IsDelete
                      && task.CompletedAt == null
                select new
                {
                    overtime.Id,
                    overtime.EmployeeId,
                    employee.Name,
                    overtime.OvertimeDate,
                    overtime.OvertimeHours,
                    overtime.Reason,
                    //task.Id,
                    task.Title
                }
            ).ToListAsync();

            var grouped = pendingOvertimes
                .GroupBy(x => new { x.EmployeeId, x.Name, Date = x.OvertimeDate.Date })
                .Select(g => new OvertimeSummaryDto
                {
                    EmployeeId = g.Key.EmployeeId,
                    EmployeeName = g.Key.Name,
                    OvertimeDate = g.Key.Date,
                    TotalHours = g.Sum(x => x.OvertimeHours),
                    Entries = g.Select(x => new OvertimeEntryDto
                    {
                        OvertimeId = x.Id,
                        TaskId = x.Id,
                        TaskTitle = x.Title,
                        OvertimeHours = x.OvertimeHours,
                        Reason = x.Reason
                    }).ToList()
                })
                .OrderBy(x => x.OvertimeDate)
                .ThenBy(x => x.EmployeeName)
                .ToList();

            return grouped;
        }

        public async Task<(bool Success, string? Error)> ApprovedOvertimeRequestAsync(string leadCode, Guid taskId,DateTime overTimeDate)
        {
            try
            {
                var approverCode = await _context.Tasks.FirstOrDefaultAsync(a => a.Id == taskId);
                var request = await _context.TaskOvertimes
                    .Where(a => a.UserTaskId == taskId && a.OvertimeDate == overTimeDate && a.Status == "pending").ToListAsync();

                if (request == null)
                {
                    return (false, "Overtime request not found.");
                }

                foreach (var data in request) 
                {
                    data.Status = "Approved";
                    data.ApprovedBy = leadCode;
                    data.ApprovedAt = DateTime.UtcNow;
                    _context.TaskOvertimes.Update(data);
                }
                
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        public async Task<(bool Success, string? Error)> RejectOvertimeRequestAsync(string leadCode, Guid taskId, string reason, DateTime overTimeDate)
        {
            try
            {
                var approverCode = await _context.Tasks.FirstOrDefaultAsync(a => a.Id == taskId);
                var request = await _context.TaskOvertimes
                    .Where(a => a.UserTaskId == taskId && a.Status == "pending" && a.OvertimeDate == overTimeDate).ToListAsync();
                if (request == null)
                {
                    return (false, "Overtime request not found.");
                }
                foreach (var data in request) 
                {
                    data.Status = "Rejected";
                    data.RejectionBy = leadCode;
                    data.RejectionReason = reason;
                    _context.TaskOvertimes.Update(data);
                }                
                await _context.SaveChangesAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }

}
