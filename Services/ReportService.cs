namespace AttendanceAPI.Services
{
    using System.ComponentModel;
    using System.Drawing;
    using AttendanceAPI.Contracts.Response;
    using AttendanceAPI.Data;
    using AttendanceAPI.Entities;
    using AttendanceAPI.Interface;
 
    using ClosedXML.Excel;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Serilog;

    public class ReportService : IReportService
    {
        private readonly ILogger<ReportService> _logger;
        private readonly TimeSpan _standardStartTime = new TimeSpan(9, 0, 0); // 9:00 AM
        private readonly int _graceMinutes = 30;
  
        private readonly IConfiguration _configuration;

        public ReportService(ILogger<ReportService> logger)
        {
            _logger = logger;
        }

        // Generate Daily Report
        public async Task<AttendanceReport> GenerateDailyReportAsync(DateTime startDate, DateTime endDate, string? managerId)
        {
            //var startDate = date.Date;
            //var endDate = startDate.AddDays(1).AddSeconds(-1);

            using (var context = new AppDbContext())
            {
                //var ManagerId = await context.AppUsers.FirstOrDefaultAsync(m => m.Code == managerId);
                //var emp = ManagerId.EmployeeCode;

                return await GenerateReportAsync("daily", startDate, endDate, managerId);
            }

        }

        // Generate Weekly Report
        public async Task<AttendanceReport> GenerateWeeklyReportAsync(DateTime date, string? managerId)
        {
            var startDate = date.Date.AddDays(-(int)date.DayOfWeek);
            var endDate = startDate.AddDays(7).AddSeconds(-1);

            using (var context = new AppDbContext())
            {
                //var ManagerId = await context.AppUsers.FirstOrDefaultAsync(m => m.Code == managerId);
                //var emp = ManagerId.EmployeeCode;

                return await GenerateReportAsync("weekly", startDate, endDate, managerId);

            }

        }

        // Generate Monthly Report
        public async Task<AttendanceReport> GenerateMonthlyReportAsync(int year, int month, string? managerId)
        {
            var startDate = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            DateTime lastDay = new DateTime(year, month, daysInMonth);
            var endDate = lastDay;

            using (var context = new AppDbContext())
            {
                //var ManagerId = await context.AppUsers.FirstOrDefaultAsync(m => m.Code == managerId);
                //var emp = ManagerId.EmployeeCode;

                return await GenerateReportAsync("monthly", startDate, endDate, managerId);

            }

        }

        // Generate Yearly Report
        public async Task<AttendanceReport> GenerateYearlyReportAsync(int year, string? managerId)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = startDate.AddYears(1).AddSeconds(-1);

            using (var context = new AppDbContext())
            {
                //var ManagerId = await context.AppUsers.FirstOrDefaultAsync(m => m.Code == managerId);
                //var emp = ManagerId.EmployeeCode;

                return await GenerateReportAsync("yearly", startDate, endDate, managerId);

            }

        }


        // Generate Daily Check In/Out Report
        public async Task<AttendanceReport> GenerateCheckInReportAsync(DateTime startDate, DateTime endDate, string? managerId)
        {
            using (var context = new AppDbContext())
            {
                return await GenerateReportAsync("daily", startDate, endDate, managerId);
            }
        }

        // Core Report Generation
        private async Task<AttendanceReport> GenerateReportAsync(string reportType, DateTime startDate, DateTime endDate, string? managerId)
        {
            Log.Information($"Generating {reportType} report from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");

            using (var context = new AppDbContext())
            {

                var employees = await context.Employees.Where(e => string.IsNullOrEmpty(managerId) || e.LeadCode == managerId).ToListAsync();


                var report = new AttendanceReport
                {
                    ReportType = reportType,
                    StartDate = startDate,
                    EndDate = endDate,
                    GeneratedBy = managerId ?? "system"
                };

               
                var totalDays = 0;

                foreach (var employee in employees)
                {
                    var lastRecord = new AttendanceRecord();
                    

                    totalDays = GetWorkingDays(startDate.Date, endDate.Date);
                    var summary = await GenerateEmployeeSummaryAsync(employee, startDate, endDate.Date, totalDays);
                    report.EmployeeSummaries.Add(summary);
                }

                // Calculate overall statistics
                report.Statistics = CalculateStatistics(report.EmployeeSummaries, totalDays, employees.Count);

                Log.Information($"Report generated: {report.EmployeeSummaries.Count} employees");
                return report;
            }
        }

        private int GetWorkingDays(DateTime startDate, DateTime endDate)
        {
            int workingDays = 0;

            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday &&
                    date.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays++;
                }
            }

            return workingDays;
        }

        // Generate Employee Summary
        private async Task<EmployeeAttendanceSummary> GenerateEmployeeSummaryAsync(Employee employee, DateTime startDate, DateTime endDate, int totalDays)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    //var records = await _storage.GetAttendanceByEmployeeAndDateRangeAsync(
                    //    employee.Code.ToString(), startDate, endDate);

                    var records = await context.AttendanceRecords
                               .Where(r => r.EmployeeCode == employee.Code && r.AttendanceDate.Date >= startDate.Date && r.AttendanceDate.Date <= endDate.Date).ToListAsync();

                    var attendHistory = await context.AttendenceHistories
                                .Where(r => r.EmployeeCode == employee.Code
                                         && r.AttendanceDate.Date >= startDate.Date
                                         && r.AttendanceDate.Date <= endDate.Date)
                                .OrderByDescending(a => a.AttendanceDate)
                                .ToListAsync();
                    //var leaveRequests = await _storage.GetLeaveRequestsByEmployeeAsync(employee.Code.ToString());

                    //var approvedLeaves = leaveRequests
                    //    .Where(l => l.Status == "approved" &&
                    //               l.StartDate <= endDate &&
                    //               l.EndDate >= startDate)
                    //    .ToList();

                    var approvedLeaveRequest = await (from lm in context.LeaveRequisitionMaster
                                                      join ld in context.LeaveRequisitionDetails
                                                         on lm.Id equals ld.MasterId
                                                      where lm.EmployeeCode == employee.Code && lm.ApproverEmployeeCode == employee.LeadCode
                                                         && lm.Status == "A" && ld.FromDate.Date <= endDate.Date && ld.ToDate.Date >= startDate.Date
                                                      orderby lm.SubmissionDate descending
                                                      select ld).ToListAsync();

                    var summary = new EmployeeAttendanceSummary
                    {
                        EmployeeId = employee.Code.ToString(),
                        EmployeeName = employee.Name,
                        Department = employee.DepartmentCode.ToString(),
                        Designation = employee.DesignationCode.ToString(),
                        TotalDays = totalDays
                    };

                    var lateRecords = new List<LateRecord>();
                    var absentRecords = new List<AbsentRecord>();
                    var presentRecords = new List<PresentRecord>();

                    // Check In/Out Logs
                    var checkInRecords = attendHistory
                        .GroupBy(a => a.AttendanceDate.Date)
                        .Select(g => new CheckInRecordsGroup
                        {
                            Date = g.Key,
                            Records = g
                                .Select(a => new CheckInRecords
                                {
                                    Date = a.AttendanceDate,
                                    CheckInTime = a.CheckInTime,
                                    CheckOutTime = a.CheckOutTime,
                                    Location = a.Location,
                                    Source = a.Source
                                })
                                .OrderBy(r => r.CheckInTime ?? r.CheckOutTime ?? r.Date)
                                .ThenBy(r => r.CheckInTime)
                                .ThenBy(r => r.CheckOutTime)
                                .ToList()
                        })
                        .OrderByDescending(g => g.Date)
                        .ToList();

                    double totalHours = 0;

                    // Check each day
                    for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                    {
                        // Skip weekends (optional - adjust based on your requirements)
                        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                            continue;

                        var record = records.FirstOrDefault(r => r.AttendanceDate.Date == date);

                        var onLeave = approvedLeaveRequest.Any(l => date >= l.FromDate.Date && date <= l.ToDate.Date);

                        if (onLeave)
                        {
                            summary.LeaveDays++;
                        }
                        else if (record != null)
                        {
                            summary.PresentDays++;

                            // Check In/Out Logs
                            //foreach (var a in attendHistory)
                            //{
                            //    checkInRecords.Add(new CheckInRecords
                            //    {
                            //        Date = a.AttendanceDate,
                            //        CheckInTime = a.CheckInTime,
                            //        CheckOutTime = a.CheckOutTime,
                            //        Location = a.Location,
                            //        Source = a.Source
                            //    });
                            //}

                            // Check In Records Work
                            //checkInRecords.Add(new CheckInRecords
                            //{
                            //    Date = record.AttendanceDate,
                            //    CheckInTime = record.CheckInTime,
                            //    CheckOutTime = record.CheckOutTime,
                            //    CheckInLocation = record.Location,
                            //    //CheckOutLocation = record.CheckOutAddress,
                            //    CheckInSource = record.Source,
                            //    //CheckOutSource = record.CheckOutSource,
                            //    //Status = record.Status
                            //});

                            // Parse total hours
                            //if (!string.IsNullOrEmpty(record.TotalHours) &&
                            //    double.TryParse(record.TotalHours, out var hours))
                            //{
                            //    totalHours += hours;
                            //}

                            // Check if late
                            //var checkInTime = record.CheckInTime.Value.TimeOfDay;
                            //var checkInTime = record.CheckInTime?.TimeOfDay ?? TimeSpan.Zero;
                            var checkInTime = record.CheckInTime.HasValue
                             ? new TimeSpan(record.CheckInTime.Value.Hour, record.CheckInTime.Value.Minute, 0)
                             : TimeSpan.Zero;

                            var lateThreshold = _standardStartTime.Add(TimeSpan.FromMinutes(_graceMinutes));
                            var isLate = checkInTime > lateThreshold;

                            if (checkInTime > lateThreshold)
                            {
                                summary.LateDays++;
                                lateRecords.Add(new LateRecord
                                {
                                    Date = date,
                                    CheckInTime = checkInTime,
                                    LateBy = checkInTime - lateThreshold
                                });
                            }
                            else
                            {
                                summary.OnTimeDays++;
                            }

                            presentRecords.Add(new PresentRecord
                            {
                                Date = date,
                                IsOnTime = !isLate
                            });
                        }
                        else
                        {
                            summary.AbsentDays++;
                            absentRecords.Add(new AbsentRecord
                            {
                                Date = date,
                                OnLeave = false
                            });
                        }
                    }

                    summary.TotalHoursWorked = totalHours;
                    summary.AverageHoursPerDay = summary.PresentDays > 0 ? totalHours / summary.PresentDays : 0;
                    summary.LateRecords = lateRecords;
                    summary.AbsentRecords = absentRecords;
                    summary.PresentRecords = presentRecords;
                    summary.CheckInRecords = checkInRecords;

                    return summary;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // Calculate Report Statistics
        private ReportStatistics CalculateStatistics(List<EmployeeAttendanceSummary> summaries, int totalDays, int count)
        {
            var workingDays = totalDays; // Adjust if excluding weekends

            return new ReportStatistics
            {
                TotalEmployees = count,
                OverallAttendanceRate =
                    summaries.Count == 0 || workingDays == 0
                        ? 0
                        : Math.Round(
                            (summaries.Sum(s => s.PresentDays)
                             / (double)(summaries.Count * workingDays)) * 100,
                            2
                          ),
                AverageLateRate =
                    workingDays == 0
                        ? 0
                        : Math.Round(
                            summaries.Sum(s => s.LateDays) / (double)workingDays,
                            2
                          ),
                //OverallAttendanceRate = summaries.Count > 0
                //    ? (summaries.Sum(s => s.PresentDays) / (double)(summaries.Count * workingDays)) * 100
                //    : 0,

                //AverageLateRate = summaries.Count > 0
                //    ? (summaries.Sum(s => s.LateDays) / (double)(summaries.Count * workingDays)) * 100
                //    : 0,
                TotalAbsences = summaries.Sum(s => s.AbsentDays),
                TotalLateArrivals = summaries.Sum(s => s.LateDays),
                TotalOnTime = summaries.Sum(s => s.OnTimeDays)
            };
        }


        // Get Employee Monthly Report
        public async Task<AttendanceReport> GenerateMonthlyReportEmployeeAsync(DateTime startDate, DateTime endDate, string? empId)
        {
            //var startDate = new DateTime(reportYear, reportMonth, 1);
            //int daysInMonth = DateTime.DaysInMonth(reportYear, reportMonth);
            //DateTime lastDay = new DateTime(reportYear, reportMonth, daysInMonth);
            //var endDate = lastDay;

            using (var context = new AppDbContext())
            {
                return await GenerateReportEmployeeAsync("monthly", startDate, endDate, empId);

            }

        }

        private async Task<AttendanceReport> GenerateReportEmployeeAsync(string reportType, DateTime startDate, DateTime endDate, string? emp)
        {

            Log.Information($"Generating {reportType} report from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");

            using (var context = new AppDbContext())
            {

                var employee = await context.Employees.FirstOrDefaultAsync(e => e.Code == emp);

                var report = new AttendanceReport
                {
                    ReportType = reportType,
                    StartDate = startDate,
                    EndDate = endDate,
                    GeneratedBy = emp ?? "system"
                };

                var totalDays = 0;

                //var lastRecord = new AttendanceRecord();
                //if (reportType == "yearly")
                //{
                //    lastRecord = await context.AttendanceRecords.Where(c => c.EmployeeCode == employee.Code && c.Year == startDate.Year).OrderByDescending(c => c.AttendanceDate).FirstOrDefaultAsync();

                //}
                //else
                //{
                //    lastRecord = await context.AttendanceRecords.Where(c => c.EmployeeCode == employee.Code && c.Year == startDate.Year && c.Month == startDate.Month).OrderByDescending(c => c.AttendanceDate).FirstOrDefaultAsync();
                //}

                //if (lastRecord == null)
                //{
                //    throw new Exception("No record found");
                //}
                //endDate = lastRecord.AttendanceDate.Date;
                //report.EndDate = endDate;

                if (reportType == "monthly" && endDate.Month == DateTime.Now.Month)
                {
                    endDate = DateTime.Now.Date;
                }

                //totalDays = GetWorkingDays(startDate.Date, reportType == "monthly" ? endDate.Date: DateTime.Now.Date);
                totalDays = GetWorkingDays(startDate.Date, endDate.Date);
                //var summary = await GenerateEmployeeSummaryAsync(employee, startDate, reportType == "monthly" ? endDate.Date : DateTime.Now.Date, totalDays);
                var summary = await GenerateEmployeeSummaryAsync(employee, startDate, endDate.Date, totalDays);
                report.EmployeeSummaries.Add(summary);
                //}

                // Calculate overall statistics
                report.Statistics = CalculateStatistics(report.EmployeeSummaries, totalDays, 1);

                //_logger.LogInformation($"✅ Report generated: {report.EmployeeSummaries.Count} employees");
                Log.Information($"Report generated: {report.EmployeeSummaries.Count} employees");
                return report;
            }
        }

        // Get Employee Yearly Report
        public async Task<AttendanceReport> GenerateYearlyReportEmployeeAsync(int year, string? empId)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = startDate.AddYears(1).AddSeconds(-1);

            using (var context = new AppDbContext())
            {
                //var ManagerId = await context.AppUsers.FirstOrDefaultAsync(m => m.Code == managerId);
                //var emp = ManagerId.EmployeeCode;

                return await GenerateReportEmployeeAsync("yearly", startDate, endDate, empId);

            }
        }

        // Get Today's Attendance Snapshot
        public async Task<DailyAttendanceSnapshot> GetTodaySnapshotAsync(string? managerId)
        {
            using (var context = new AppDbContext())
            {
                var today = DateTime.Today;

                var employees = await context.Employees.Where(e => e.LeadCode == managerId).ToListAsync();

                var snapshot = new DailyAttendanceSnapshot
                {
                    Date = today
                };

                foreach (var employee in employees)
                {
                    var record = await GetTodayAttendanceAsync(employee.Code.ToString());

                    var onLeave = await (from lm in context.LeaveRequisitionMaster
                                         join ld in context.LeaveRequisitionDetails
                                            on lm.Id equals ld.MasterId
                                         where lm.EmployeeCode == employee.Code && lm.ApproverEmployeeCode == employee.LeadCode
                                            && lm.Status == "A" && ld.FromDate.Date <= today.Date && ld.ToDate.Date >= today.Date
                                         select 1).AnyAsync();

                    var status = new EmployeeStatus
                    {
                        EmployeeId = employee.Code.ToString(),
                        EmployeeName = employee.Name,
                        Department = employee.DepartmentCode.ToString(),
                        ManagerId = employee.LeadCode ?? ""
                    };

                    if (onLeave)
                    {
                        status.Status = "on-leave";
                        snapshot.TotalOnLeave++;
                    }
                    else if (record?.CheckInTime != null)
                    {
                        status.CheckInTime = record.CheckInTime;
                        var checkInTime = record.CheckInTime.Value.TimeOfDay;
                        var lateThreshold = _standardStartTime.Add(TimeSpan.FromMinutes(_graceMinutes));

                        if (checkInTime > lateThreshold)
                        {
                            status.Status = "late";
                            status.LateBy = checkInTime - lateThreshold;
                            snapshot.TotalLate++;
                        }
                        else
                        {
                            status.Status = "present";
                        }
                        snapshot.TotalPresent++;
                    }
                    else
                    {
                        status.Status = "absent";
                        snapshot.TotalAbsent++;
                    }

                    snapshot.EmployeeStatuses.Add(status);
                }

                return snapshot;
            }
        }
        // Get Range Late Employee List
        public async Task<List<EmployeeLateSummaryResponse>> GetLateEmployeesRangeGroupedAsync(string? managerId, DateTime startDate, DateTime endDate)
        {
            using (var context = new AppDbContext())
            {
                var start = startDate.Date;
                var end = endDate.Date;

                // Get all employees under this manager
                var employees = await context.Employees
                    .Where(e => e.LeadCode == managerId)
                    .ToListAsync();

                var lateSummaries = new List<EmployeeLateSummaryResponse>();
                var lateThreshold = _standardStartTime.Add(TimeSpan.FromMinutes(_graceMinutes));

                foreach (var employee in employees)
                {
                    var lateRecords = new List<LateDateRecord>();

                    for (var date = start; date <= end; date = date.AddDays(1))
                    {
                        // Check if employee is on leave for this date
                        var onLeave = await (
                            from lm in context.LeaveRequisitionMaster
                            join ld in context.LeaveRequisitionDetails
                                on lm.Id equals ld.MasterId
                            where lm.EmployeeCode == employee.Code
                                  && lm.ApproverEmployeeCode == employee.LeadCode
                                  && lm.Status == "A"
                                  && ld.FromDate.Date <= date
                                  && ld.ToDate.Date >= date
                            select 1
                        ).AnyAsync();

                        if (onLeave)
                            continue;

                        // Get attendance for this specific date
                        var record = await GetAttendanceByDateAsync(
                            employee.Code.ToString(),
                            date);

                        if (record?.CheckInTime != null)
                        {
                            var checkInTime = record.CheckInTime.Value.TimeOfDay;

                            if (checkInTime > lateThreshold)
                            {
                                var lateBy = checkInTime - lateThreshold;

                                lateRecords.Add(new LateDateRecord
                                {
                                    Date = date,
                                    LateBy = lateBy,
                                    CheckInTime = record.CheckInTime
                                });
                            }
                        }
                    }

                    // If employee has any late records, add them to summaries
                    if (lateRecords.Any())
                    {
                        lateSummaries.Add(new EmployeeLateSummaryResponse
                        {
                            EmployeeId = employee.Code.ToString(),
                            EmployeeName = employee.Name,
                            Department = employee.DepartmentCode.ToString(),
                            ManagerId = employee.LeadCode ?? "",
                            LateRecords = lateRecords.OrderBy(r => r.Date).ToList()
                        });
                    }
                }

                // Sort by total late minutes (most late first)
                return lateSummaries
                    .OrderByDescending(e => e.TotalLateMinutes)
                    .ToList();
            }
        }

        // Keep the existing GetAttendanceByDateAsync helper method
        private async Task<AttendanceRecord?> GetAttendanceByDateAsync(string employeeCode, DateTime date)
        {
            using (var context = new AppDbContext())
            {
                return await context.AttendanceRecords
                    .Where(a => a.EmployeeCode == employeeCode
                        && a.AttendanceDate == date.Date)
                    .FirstOrDefaultAsync();
            }
        }

        // Get Today Late Employees List
        public async Task<List<EmployeeStatus>> GetLateEmployeesTodayAsync(string? managerId)
        {
            var snapshot = await GetTodaySnapshotAsync(managerId);
            return snapshot.EmployeeStatuses
                .Where(s => s.Status == "late")
                .OrderByDescending(s => s.LateBy)
                .ToList();
        }

        // Get Absent Employees List
        public async Task<List<EmployeeAbsentSummaryResponse>> GetAbsentEmployeesAsync(string? managerId, DateTime startDate, DateTime endDate)
        {
            using (var context = new AppDbContext())
            {
                var start = startDate.Date;
                var end = endDate.Date;

                // Get all employees under this manager
                var employees = await context.Employees
                    .Where(e => e.LeadCode == managerId)
                    .ToListAsync();

                // Get Holiday Dates
                var holidayList = await context.Holiday.Where(a => a.HolidayDate.Date >= start && a.HolidayDate.Date <= end).ToListAsync();
                var holidayDates = holidayList.Select(a => a.HolidayDate).ToHashSet();


                var absentSummaries = new List<EmployeeAbsentSummaryResponse>();

                foreach (var employee in employees)
                {

                    var getWeekend = await (from a in context.Employees
                                     join b in context.StandardHours
                                     on a.StandardHourCode equals b.GroupCode
                                     where a.Code == employee.Code && b.Hours == "00:00"
                                     select b)
                                     .ToListAsync();

                    var absentRecords = new List<AbsentDateRecord>();

                    for (var date = start; date <= end; date = date.AddDays(1))
                    {
                        // Check if employee is on leave for this date
                        var onLeave = await (
                            from lm in context.LeaveRequisitionMaster
                            join ld in context.LeaveRequisitionDetails
                                on lm.Id equals ld.MasterId
                            where lm.EmployeeCode == employee.Code
                                  && lm.ApproverEmployeeCode == employee.LeadCode
                                  && lm.Status == "A"
                                  && ld.FromDate.Date <= date
                                  && ld.ToDate.Date >= date
                            select 1
                        ).AnyAsync();

                        if (onLeave)
                            continue;

                        // Get attendance for this specific date
                        var record = await GetAttendanceByDateAsync(
                            employee.Code.ToString(),
                            date);

                        // Get Day
                        var dayName = date.ToString("ddd");
                        var isWeekEnd = getWeekend.Where(a => a.WeekDayName.Equals(dayName, StringComparison.OrdinalIgnoreCase)).Select(b => new { b.Hours });

                        if (record?.CheckInTime == null && !holidayDates.Contains(date.Date) && !isWeekEnd.Any(a => a.Hours == "00:00"))
                        {
                            absentRecords.Add(new AbsentDateRecord
                            {
                                Date = date,
                                Status = "absent",
                            });
                        }
                    }

                    // If employee has any absent records, add them to summaries
                    if (absentRecords.Any())
                    {
                        absentSummaries.Add(new EmployeeAbsentSummaryResponse
                        {
                            EmployeeId = employee.Code.ToString(),
                            EmployeeName = employee.Name,
                            Department = employee.DepartmentCode.ToString(),
                            ManagerId = employee.LeadCode ?? "",
                            AbsentRecords = absentRecords.OrderBy(r => r.Date).ToList()
                        });
                    }
                }

                return absentSummaries.ToList();
            }
        }

        public async Task<AttendanceRecord?> GetTodayAttendanceAsync(string employeeId)
        {
            using (var context = new AppDbContext())
            {
                //var records = await GetAttendanceRecordsAsync();
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);
                var todayEmployeeRecords = await context.AttendanceRecords
                    .Where(r => r.EmployeeCode == employeeId && r.AttendanceDate >= today && r.AttendanceDate < tomorrow).ToListAsync();

                // Filter only today's records for the given employee
                //var todayEmployeeRecords = records
                //    .Where(r => r.EmployeeCode == employeeId && r.AttendanceDate.Date == today)
                //    .ToList();

                // If employee has no record today → return null
                if (!todayEmployeeRecords.Any())
                    return null;

                // Safely get earliest check-in
                var earliestCheckIn = todayEmployeeRecords
                    .Where(r => r.CheckInTime.HasValue)
                    .OrderBy(r => r.CheckInTime.Value)
                    .Select(r => r.CheckInTime)
                    .FirstOrDefault(); // returns null if no checkin

                // Safely get latest check-out
                var latestCheckOut = todayEmployeeRecords
                    .Where(r => r.CheckOutTime.HasValue)
                    .OrderByDescending(r => r.CheckOutTime.Value)
                    .Select(r => r.CheckOutTime)
                    .FirstOrDefault(); // returns null if no checkout

                // Get the main attendance record (first one)
                var attendance = todayEmployeeRecords.First();

                // Assign safely
                attendance.CheckInTime = earliestCheckIn;
                attendance.CheckOutTime = latestCheckOut;

                if (earliestCheckIn.HasValue && latestCheckOut.HasValue)
                {
                    TimeSpan totalDuration = latestCheckOut.Value - earliestCheckIn.Value;
                    string totalDurationText = totalDuration.ToString(@"hh\:mm");

                    attendance.TotalHours = totalDurationText;

                }

                //await context.SaveChangesAsync();
                return attendance;
            }
        }


        public async Task<byte[]> GetExcelCheckInReport(DateTime sDate, DateTime eDate, string reportType, string managerId)
        {
            try
            {
                DateTime startDate = new DateTime();
                DateTime endDate = new DateTime();
                var report = new AttendanceReport();

                    startDate = sDate.Date;
                    endDate = eDate.Date;
                    report = await GenerateReportAsync(reportType, startDate, endDate, managerId);

                    //else if (reportType == "Monthly")
                    //{
                    //    startDate = new DateTime(date.Year, date.Month, 1);
                    //    int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
                    //    DateTime lastDay = new DateTime(date.Year, date.Month, daysInMonth);
                    //    endDate = lastDay;
                    //    report = await GenerateReportAsync(reportType, startDate, endDate, managerId);

                    //}
                    //else if (reportType == "Yearly")
                    //{
                    //    startDate = new DateTime(date.Year, 1, 1);
                    //    //endDate = startDate.AddYears(1).AddSeconds(-1);
                    //    endDate = DateTime.Today;
                    //    report = await GenerateReportAsync(reportType, startDate, endDate, managerId);
                    //}
                    return await GenerateExcelCheckInReport(report);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<byte[]> GetExportWorkByManager(DateTime date, string reportType, string managerId)
        {
            try
            {
                DateTime startDate = new DateTime();
                DateTime endDate = new DateTime();
                var report = new AttendanceReport();
                if (reportType == "Daily")
                {
                    startDate = date.Date;
                    endDate = startDate.AddDays(1).AddSeconds(-1);
                    report = await GenerateReportAsync(reportType, startDate, endDate, managerId);
                }

                else if (reportType == "Monthly")
                {
                    startDate = new DateTime(date.Year, date.Month, 1);
                    int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
                    DateTime lastDay = new DateTime(date.Year, date.Month, daysInMonth);
                    endDate = lastDay;
                    report = await GenerateReportAsync(reportType, startDate, endDate, managerId);

                }
                else if (reportType == "Yearly")
                {
                    startDate = new DateTime(date.Year, 1, 1);
                    //endDate = startDate.AddYears(1).AddSeconds(-1);
                    endDate = DateTime.Today;
                    report = await GenerateReportAsync(reportType, startDate, endDate, managerId);
                }
                return await GenerateExcelReport(report);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        private async Task<byte[]> GenerateExcelCheckInReport(AttendanceReport attendanceReport)
        {
            using (var context = new AppDbContext())
            {
                var empData = await context.Employees
                    .FirstOrDefaultAsync(a => a.Code == attendanceReport.GeneratedBy);

                var empName = empData?.Name ?? "Unknown";

                using var workbook = new XLWorkbook();
                var ws = workbook.Worksheets.Add("Check In and Out Report");

                ws.Cell("A1").Value = "CHECK IN AND OUT REPORT";
                ws.Range("A1:J1").Merge();

                ws.Cell("A1").Style.Font.Bold = true;
                ws.Cell("A1").Style.Font.FontSize = 22;
                ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("A1").Style.Fill.BackgroundColor = XLColor.LightGray;

                ws.Cell("A3").Value = $"Today: {DateTime.Today:dd-MM-yyyy}";
                ws.Cell("C3").Value = $"From: {attendanceReport.StartDate:dd-MM-yyyy HH:mm}";
                ws.Cell("F3").Value = $"To: {attendanceReport.EndDate:dd-MM-yyyy HH:mm}";

                var reportType = string.IsNullOrEmpty(attendanceReport.ReportType)
                    ? "Today"
                    : char.ToUpper(attendanceReport.ReportType[0]) +
                      attendanceReport.ReportType.Substring(1);

                ws.Cell("A5").Value = "Report Type:";
                ws.Cell("B5").Value = reportType;

                ws.Cell("D5").Value = "Generated By:";
                ws.Cell("E5").Value = empName;

                ws.Cell("G5").Value = "Generated At:";
                ws.Cell("H5").Value = attendanceReport.GeneratedAt.ToString("dd-MM-yyyy HH:mm");

                ws.Range("A3:H5").Style.Font.Bold = true;

                ws.Cell("A8").Value = "EMPLOYEE STATISTICS";
                ws.Range("A8:B8").Merge();

                ws.Cell("A8").Style.Font.Bold = true;
                ws.Cell("A8").Style.Font.FontSize = 16;
                ws.Cell("A8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("A8").Style.Fill.BackgroundColor = XLColor.LightGray;

                ws.Cell("A10").Value = "Total Employees";
                ws.Cell("B10").Value = attendanceReport.Statistics.TotalEmployees;

                ws.Cell("A11").Value = "Total On Time";
                ws.Cell("B11").Value = attendanceReport.Statistics.TotalOnTime;

                ws.Cell("A12").Value = "Total Late Arrivals";
                ws.Cell("B12").Value = attendanceReport.Statistics.TotalLateArrivals;

                ws.Cell("A13").Value = "Total Absences";
                ws.Cell("B13").Value = attendanceReport.Statistics.TotalAbsences;

                ws.Range("A10:A13").Style.Font.Bold = true;

                int headerRow = 17;

                ws.Cell(headerRow, 1).Value = "Code";
                ws.Cell(headerRow, 2).Value = "Name";
                ws.Cell(headerRow, 3).Value = "Attendance Date";
                ws.Cell(headerRow, 4).Value = "Check In";
                ws.Cell(headerRow, 5).Value = "Check Out";
                ws.Cell(headerRow, 6).Value = "Source";
                ws.Cell(headerRow, 7).Value = "Location";

                var headerRange = ws.Range(headerRow, 1, headerRow, 7);

                headerRange.Style.Font.Bold = true;
                headerRange.Style.Font.FontColor = XLColor.White;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(0, 112, 192);
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                int row = headerRow + 1;

                if (attendanceReport.EmployeeSummaries?.Any() == true)
                {
                    // Sort employees by name (optional - keeps employee rows grouped)
                    var sortedEmployees = attendanceReport.EmployeeSummaries
                        .OrderBy(e => e.EmployeeName)
                        .ToList();

                    foreach (var employee in sortedEmployees)
                    {
                        if (employee.CheckInRecords?.Any() == true)
                        {
                            var flattenedRecords = attendanceReport.EmployeeSummaries
                                .Where(e => e.CheckInRecords?.Any() == true)
                                .SelectMany(e => e.CheckInRecords
                                    .Where(g => g.Records?.Any() == true)
                                    .SelectMany(g => g.Records.Select(r => new
                                    {
                                        Employee = e,
                                        GroupDate = g.Date,
                                        Record = r,
                                        EffectiveTime = r.CheckInTime ?? r.CheckOutTime ?? g.Date
                                    })))
                                .OrderBy(x => x.Employee.EmployeeId)
                                .ThenBy(x => x.GroupDate)
                                .ThenBy(x => x.EffectiveTime)
                                .ThenBy(x => x.Record.CheckInTime)
                                .ThenBy(x => x.Record.CheckOutTime)
                                .ToList();

                            foreach (var item in flattenedRecords)
                            {
                                ws.Cell(row, 1).Value = employee.EmployeeId ?? "";
                                ws.Cell(row, 2).Value = employee.EmployeeName ?? "";
                                ws.Cell(row, 3).Value = item.GroupDate.ToString("dd-MM-yyyy");
                                ws.Cell(row, 4).Value = item.Record.CheckInTime?.ToString("hh:mm tt") ?? "";
                                ws.Cell(row, 5).Value = item.Record.CheckOutTime?.ToString("hh:mm tt") ?? "";
                                ws.Cell(row, 6).Value = item.Record.Source ?? "";
                                ws.Cell(row, 7).Value = item.Record.Location ?? "";

                                row++;
                            }
                        }
                        else
                        {
                            ws.Cell(row, 1).Value = employee.EmployeeId ?? "";
                            ws.Cell(row, 2).Value = employee.EmployeeName ?? "";
                            ws.Cell(row, 3).Value = attendanceReport.StartDate.ToString("dd-MM-yyyy");
                            ws.Cell(row, 7).Value = "Absent";

                            ws.Cell(row, 7).Style.Font.FontColor = XLColor.Red;
                            ws.Cell(row, 7).Style.Font.Bold = true;

                            row++;
                        }
                    }
                }

                var dataRange = ws.Range(headerRow, 1, row - 1, 7);

                dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Border.InsideBorderColor = XLColor.LightGray;
                dataRange.Style.Border.OutsideBorderColor = XLColor.Black;

                dataRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                dataRange.Style.Alignment.WrapText = true;

                dataRange.Style.Alignment.Indent = 1;

                ws.Column(2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Columns().AdjustToContents();

                ws.Column(1).Width = Math.Max(ws.Column(1).Width, 12);
                ws.Column(2).Width = Math.Max(ws.Column(2).Width, 25);
                ws.Column(3).Width = Math.Max(ws.Column(3).Width, 18);
                ws.Column(4).Width = Math.Max(ws.Column(4).Width, 15);
                ws.Column(5).Width = Math.Max(ws.Column(5).Width, 15);
                ws.Column(6).Width = Math.Max(ws.Column(6).Width, 15);
                ws.Column(7).Width = Math.Max(ws.Column(7).Width, 35);

                foreach (var usedRow in ws.RowsUsed())
                {
                    usedRow.Height = Math.Max(usedRow.Height, 22);
                }

                //ws.Range(headerRow, 1, row - 1, 7).SetAutoFilter();

                //ws.SheetView.FreezeRows(headerRow);

                int footerRow = row + 2;

                ws.Cell(footerRow, 1).Value =
                    $"Report generated on: {DateTime.Now:dd-MM-yyyy HH:mm:ss}";

                ws.Range(footerRow, 1, footerRow, 7).Merge();

                ws.Cell(footerRow, 1).Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;

                ws.Cell(footerRow, 1).Style.Font.Italic = true;

                using var stream = new MemoryStream();

                workbook.SaveAs(stream);

                return stream.ToArray();
            }
        }

        private async Task<byte[]> GenerateExcelReport(AttendanceReport attendanceReport)
        {
            using (var context = new AppDbContext())
            {
                var employeeData = await context.Employees.FirstOrDefaultAsync(e => e.Code == attendanceReport.GeneratedBy);
                var employeeName = employeeData.Name;

                using var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Attendance Report");

                // Title
                ws.Cell("A1").Value = "ATTENDANCE REPORT";
                ws.Cell("A1").Style.Font.Bold = true;
                ws.Cell("A1").Style.Font.FontSize = 22;
                ws.Range("A1:G1").Merge();
                ws.Range("A1:G1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("A1:G1").Style.Fill.BackgroundColor = XLColor.LightGray;

                // Dates
                ws.Cell("A3").Value = $"Today: {DateTime.Today:dd-MM-yyyy}";
                ws.Cell("B3").Value = $"From: {attendanceReport.StartDate:dd-MM-yyyy HH:mm}";
                ws.Cell("C3").Value = $"To: {attendanceReport.EndDate:dd-MM-yyyy HH:mm}";
                var reportType = string.IsNullOrEmpty(attendanceReport.ReportType)
                    ? attendanceReport.ReportType
                    : char.ToUpper(attendanceReport.ReportType[0]) + attendanceReport.ReportType.Substring(1);
                ws.Cell("A5").Value = "Report Type: ";
                ws.Cell("B5").Value = reportType;

                ws.Cell("C5").Value = "Generated By:";
                ws.Cell("D5").Value = employeeName;

                ws.Cell("F5").Value = "Generated At:";
                ws.Cell("G5").Value = $"{attendanceReport.GeneratedAt:dd-MM-yyyy HH:mm}";

                ws.Cell("A3").Style.Font.Bold = true;
                ws.Cell("B3").Style.Font.Bold = true;
                ws.Cell("C3").Style.Font.Bold = true;

                ws.Cell("A5").Style.Font.Bold = true;
                ws.Cell("C5").Style.Font.Bold = true;
                ws.Cell("F5").Style.Font.Bold = true;

                ws.Cell("A8").Value = "EMPLOYEE STATISTICS";
                ws.Cell("A8").Style.Font.Bold = true;
                ws.Cell("A8").Style.Font.FontSize = 16;
                ws.Range("A8:B8").Merge();
                ws.Range("A8:B8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("A8:B8").Style.Fill.BackgroundColor = XLColor.LightGray;

                ws.Cell("A10").Value = "Total Employee";
                ws.Cell("B10").Value = $"{attendanceReport.Statistics.TotalEmployees}";

                ws.Cell("A11").Value = "Total on Time";
                ws.Cell("B11").Value = $"{attendanceReport.Statistics.TotalOnTime}";

                ws.Cell("A12").Value = "Overall Attendance Rate";
                ws.Cell("B12").Value = $"{attendanceReport.Statistics.OverallAttendanceRate}%";

                ws.Cell("A13").Value = "Total Late Arrivals";
                ws.Cell("B13").Value = $"{attendanceReport.Statistics.TotalLateArrivals}";

                ws.Cell("A14").Value = "Average Late Rate";
                ws.Cell("B14").Value = $"{attendanceReport.Statistics.AverageLateRate}%";

                ws.Cell("A15").Value = "Total Absences";
                ws.Cell("B15").Value = $"{attendanceReport.Statistics.TotalAbsences}";

                ws.Range("A10:A15").Style.Font.Bold = true;
                ws.Range("B10:B15").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                int headerRow = 17;
                int row = headerRow + 2; // Account for sub-header row

                // Define main headers
                ws.Cell(headerRow, 1).Value = "Code";
                ws.Cell(headerRow, 2).Value = "Name";
                ws.Cell(headerRow, 3).Value = "Department";
                ws.Cell(headerRow, 4).Value = "Designation";
                ws.Cell(headerRow, 5).Value = "Total Days";
                ws.Cell(headerRow, 6).Value = "Present Days";
                ws.Cell(headerRow, 7).Value = "Leave Days";
                ws.Cell(headerRow, 8).Value = "Total Hours Worked";
                ws.Cell(headerRow, 9).Value = "Average Hours Per Day";
                ws.Cell(headerRow, 10).Value = "On Time Days";
                ws.Cell(headerRow, 11).Value = "Absent Days";

                int currentColumn = 12;

                // LateRecords and AbsentRecords subcolumns (only for monthly or yearly reports)
                bool showLateRecords = attendanceReport.ReportType == "monthly" || attendanceReport.ReportType == "yearly";
                if (showLateRecords)
                {
                    // LateRecords subcolumns
                    ws.Cell(headerRow, currentColumn).Value = "LateRecords";
                    ws.Range(headerRow, currentColumn, headerRow, currentColumn + 2).Merge();
                    ws.Cell(headerRow + 1, currentColumn).Value = "Date";
                    ws.Cell(headerRow + 1, currentColumn + 1).Value = "Check In";
                    ws.Cell(headerRow + 1, currentColumn + 2).Value = "Late By";
                    currentColumn += 3;

                    // AbsentRecords subcolumn
                    ws.Cell(headerRow, currentColumn).Value = "Absent Records";
                    ws.Range(headerRow, currentColumn, headerRow, currentColumn).Merge();
                    ws.Cell(headerRow + 1, currentColumn).Value = "Date";
                    currentColumn++; // Move to next column if needed
                }

                // Bold headers and set alignment
                var headerRange = ws.Range(headerRow, 1, headerRow + 1, currentColumn - 1);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Set column widths to prevent auto-expansion
                ws.Column(1).Width = 10;  // Code
                ws.Column(2).Width = 20;  // Name
                ws.Column(3).Width = 15;  // Department
                ws.Column(4).Width = 15;  // Designation
                ws.Column(5).Width = 10;  // Total Days
                ws.Column(6).Width = 12;  // Present Days
                ws.Column(7).Width = 10;  // Leave Days
                ws.Column(8).Width = 15;  // Total Hours Worked
                ws.Column(9).Width = 18;  // Average Hours Per Day
                ws.Column(10).Width = 12; // On Time Days
                ws.Column(11).Width = 12; // Absent Days

                if (showLateRecords)
                {
                    ws.Column(12).Width = 12; // LateRecords - Date
                    ws.Column(13).Width = 10; // LateRecords - CheckIn
                    ws.Column(14).Width = 10; // LateRecords - LateBy
                    ws.Column(15).Width = 12; // AbsentRecords - Date
                }

                // Enable text wrapping for long content
                ws.Column(2).Style.Alignment.WrapText = true;
                ws.Column(3).Style.Alignment.WrapText = true;
                ws.Column(4).Style.Alignment.WrapText = true;

                foreach (var emp in attendanceReport.EmployeeSummaries)
                {
                    int lateCount = 0;
                    int absentCount = 0;

                    if (showLateRecords)
                    {
                        lateCount = emp.LateRecords?.Count ?? 0;
                        absentCount = emp.AbsentRecords?.Count ?? 0;
                    }

                    int maxChildRows = Math.Max(lateCount, absentCount);
                    if (maxChildRows == 0) maxChildRows = 1;

                    // Fill main employee info (merge vertically)
                    ws.Cell(row, 1).Value = emp.EmployeeId;
                    ws.Cell(row, 2).Value = emp.EmployeeName;
                    ws.Cell(row, 3).Value = emp.Department;
                    var designation = context.Designations.FirstOrDefault(d => d.Code == int.Parse(emp.Designation));
                    ws.Cell(row, 4).Value = designation?.Name ?? "-";
                    ws.Cell(row, 5).Value = emp.TotalDays;
                    ws.Cell(row, 6).Value = emp.PresentDays;
                    ws.Cell(row, 7).Value = emp.LeaveDays;
                    ws.Cell(row, 8).Value = emp.TotalHoursWorked;
                    ws.Cell(row, 9).Value = emp.AverageHoursPerDay;
                    ws.Cell(row, 10).Value = emp.OnTimeDays;
                    ws.Cell(row, 11).Value = emp.AbsentDays;

                    // Apply alignment and wrapping to main employee info
                    for (int c = 1; c <= 11; c++)
                    {
                        ws.Cell(row, c).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        ws.Cell(row, c).Style.Alignment.WrapText = true;
                    }

                    if (maxChildRows > 1)
                    {
                        for (int c = 1; c <= 11; c++)
                        {
                            var range = ws.Range(row, c, row + maxChildRows - 1, c);
                            range.Merge();
                            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                            range.Style.Alignment.WrapText = true;
                        }
                    }

                    // Fill LateRecords and AbsentRecords vertically (only if showLateRecords is true)
                    if (showLateRecords)
                    {
                        // Fill LateRecords
                        for (int i = 0; i < maxChildRows; i++)
                        {
                            if (emp.LateRecords != null && i < emp.LateRecords.Count)
                            {
                                var l = emp.LateRecords[i];
                                ws.Cell(row + i, 12).Value = l.Date.ToString("dd-MM-yyyy");
                                ws.Cell(row + i, 13).Value = l.CheckInTime.ToString(@"hh\:mm\:ss");
                                ws.Cell(row + i, 14).Value = l.LateBy.ToString(@"hh\:mm\:ss");
                            }
                            else if (maxChildRows > 1)
                            {
                                // Only fill with "-" if there are multiple rows
                                ws.Cell(row + i, 12).Value = "-";
                                ws.Cell(row + i, 13).Value = "-";
                                ws.Cell(row + i, 14).Value = "-";
                            }

                            // Apply alignment
                            ws.Cell(row + i, 12).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                            ws.Cell(row + i, 13).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                            ws.Cell(row + i, 14).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        }

                        // Fill AbsentRecords
                        for (int i = 0; i < maxChildRows; i++)
                        {
                            if (emp.AbsentRecords != null && i < emp.AbsentRecords.Count)
                            {
                                var a = emp.AbsentRecords[i];
                                ws.Cell(row + i, 15).Value = a.Date.ToString("dd-MM-yyyy");
                            }
                            else if (maxChildRows > 1)
                            {
                                // Only fill with "-" if there are multiple rows
                                ws.Cell(row + i, 15).Value = "-";
                            }
                            ws.Cell(row + i, 15).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        }
                    }

                    row += maxChildRows;
                }

                // Add borders to the data table
                int lastColumn = showLateRecords ? 15 : 11;
                ws.Range(headerRow, 1, row - 1, lastColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Range(headerRow, 1, row - 1, lastColumn).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                ws.Columns().AdjustToContents();

                // Save to byte array
                using var stream = new MemoryStream();
                wb.SaveAs(stream);
                return stream.ToArray();
            }
        }

        public async Task<byte[]> GetExportWorkByEmployee(DateTime startDate, DateTime endDate, string reportType, string empId)
        {
            try
            {
                //DateTime startDate = new DateTime();
                //DateTime endDate = new DateTime();
                var report = new AttendanceReport();
                if (reportType == "Today")
                {
                    //startDate = date.Date;
                    endDate = startDate.AddDays(1).AddSeconds(-1);
                    report = await GenerateReportEmployeeAsync(reportType, startDate, endDate, empId);
                }
                else
                {
                    report = await GenerateReportEmployeeAsync(reportType, startDate, endDate, empId);
                }

                    //else if (reportType == "Monthly")
                    //{
                    //    startDate = new DateTime(date.Year, date.Month, 1);
                    //    int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
                    //    DateTime lastDay = new DateTime(date.Year, date.Month, daysInMonth);
                    //    endDate = lastDay;
                    //    report = await GenerateReportEmployeeAsync(reportType, startDate, endDate, emp);

                    //}
                    //else if (reportType == "Yearly")
                    //{
                    //    startDate = new DateTime(date.Year, 1, 1);
                    //    //endDate = startDate.AddYears(1).AddSeconds(-1);
                    //    endDate = DateTime.Today;
                    //    report = await GenerateReportEmployeeAsync(reportType, startDate, endDate, emp);
                    //}
                return await GenerateExcelReport(report);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<AttendanceReport> GenerateDailyReportEmployeeAsync(DateTime date, string? empId)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1).AddSeconds(-1);

            using (var context = new AppDbContext())
            {
                //var ManagerId = await context.AppUsers.FirstOrDefaultAsync(m => m.Code == managerId);
                //var emp = ManagerId.EmployeeCode;

                return await GenerateReportEmployeeAsync("daily", startDate, endDate, empId);
            }

        }
    }
}