namespace AttendanceAPI.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using AttendanceAPI.Services;
    using AttendanceAPI.Interface;
    using Microsoft.AspNetCore.Authorization;

    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        //private readonly ReportService _reportService;
        private readonly ILogger<ReportController> _logger;
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService, ILogger<ReportController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        // GET: api/report/daily?date=2024-01-15&managerId=MGR001
        [HttpGet("daily")]
        public async Task<IActionResult> GetDailyReport([FromQuery] DateTime? date, [FromQuery] DateTime? endDate, [FromQuery] string? managerId)
        {
            try
            {
                var startDate = date ?? DateTime.Today;
                var reportEndDate = endDate ?? startDate;

                var report = await _reportService.GenerateDailyReportAsync(startDate, reportEndDate, managerId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating daily report: {ex.Message}");
                return StatusCode(500, new { error = "Failed to generate report" });
            }
        }


        [HttpGet("checkIn")]
        public async Task<IActionResult> GetDailyCheckInReport([FromQuery] DateTime? date, [FromQuery] DateTime? eDate, [FromQuery] string? managerId)
        {
            try
            {
                var startDate = date ?? DateTime.Today;
                var endDate = eDate ?? DateTime.Today;

                var report = await _reportService.GenerateCheckInReportAsync(startDate, endDate, managerId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating daily report: {ex.Message}");
                return StatusCode(500, new { error = "Failed to generate report" });
            }
        }

        // GET: api/report/weekly?date=2024-01-15&managerId=MGR001
        //[HttpGet("weekly")]
        //public async Task<IActionResult> GetWeeklyReport([FromQuery] DateTime? date, [FromQuery] string? managerId)
        //{
        //    try
        //    {
        //        var reportDate = date ?? DateTime.Today;
        //        var report = await _reportService.GenerateWeeklyReportAsync(reportDate, managerId);
        //        return Ok(report);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error generating weekly report: {ex.Message}");
        //        return StatusCode(500, new { error = "Failed to generate report" });
        //    }
        //}

        // GET: api/report/monthly?year=2024&month=1&managerId=MGR001
        //[HttpGet("monthly")]
        //public async Task<IActionResult> GetMonthlyReport([FromQuery] int? year, [FromQuery] int? month, [FromQuery] string? managerId)
        //{
        //    try
        //    {
        //        var reportYear = year ?? DateTime.Now.Year;
        //        var reportMonth = month ?? DateTime.Now.Month;
        //        var report = await _reportService.GenerateMonthlyReportAsync(reportYear, reportMonth, managerId);
        //        return Ok(report);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error generating monthly report: {ex.Message}");
        //        return StatusCode(500, new { error = "Failed to generate report" });
        //    }
        //}

        // GET: api/report/yearly?year=2024&managerId=MGR001
        //[HttpGet("yearly")]
        //public async Task<IActionResult> GetYearlyReport([FromQuery] int? year, [FromQuery] string? managerId)
        //{
        //    try
        //    {
        //        var reportYear = year ?? DateTime.Now.Year;
        //        var report = await _reportService.GenerateYearlyReportAsync(reportYear, managerId);
        //        return Ok(report);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error generating yearly report: {ex.Message}");
        //        return StatusCode(500, new { error = "Failed to generate report" });
        //    }
        //}

        // GET: api/report/monthly?year=2024&month=1&employeeId=MGR001
        //[HttpGet("empmonthly")]
        //public async Task<IActionResult> GenerateMonthlyReportEmployeeAsync([FromQuery] int? year, [FromQuery] int? month, [FromQuery] string? employeeId)
        //{
        //    try
        //    {
        //        var reportYear = year ?? DateTime.Now.Year;
        //        var reportMonth = month ?? DateTime.Now.Month;
        //        var report = await _reportService.GenerateMonthlyReportEmployeeAsync(reportYear, reportMonth, employeeId);
        //        return Ok(report);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error generating monthly report: {ex.Message}");
        //        return StatusCode(500, new { error = "Failed to generate report" });
        //    }
        //}
        [HttpGet("empmonthly")]
        public async Task<IActionResult> GenerateMonthlyReportEmployeeAsync([FromQuery] DateTime? date,[FromQuery] DateTime? endDate,[FromQuery] string? employeeId)
        {
            try
            {
                var startDate = date ?? DateTime.Today;
                var reportEndDate = endDate ?? startDate;
                var report = await _reportService.GenerateMonthlyReportEmployeeAsync(startDate, reportEndDate, employeeId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating monthly report: {ex.Message}");
                return StatusCode(500, new { error = "Failed to generate report" });
            }
        }


        // GET: api/report/yearly?year=2024&employeeId=MGR001
        //[HttpGet("empyearly")]
        //public async Task<IActionResult> GetEmployeeYearlyReport([FromQuery] int? year, [FromQuery] string? employeeId)
        //{
        //    try
        //    {
        //        var reportYear = year ?? DateTime.Now.Year;
        //        var report = await _reportService.GenerateYearlyReportEmployeeAsync(reportYear, employeeId);
        //        return Ok(report);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error generating yearly report: {ex.Message}");
        //        return StatusCode(500, new { error = "Failed to generate report" });
        //    }
        //}

        //GET: api/report/snapshot/today? managerId = MGR001
        [HttpGet("snapshot/today")]
        public async Task<IActionResult> GetTodaySnapshot([FromQuery] string? managerId)
        {
            try
            {
                var snapshot = await _reportService.GetTodaySnapshotAsync(managerId);
                return Ok(snapshot);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting today snapshot: {ex.Message}");
                return StatusCode(500, new { error = "Failed to get snapshot" });
            }
        }


        // PDF export call
        [HttpGet("getPDF")]
        public async Task<IActionResult> GetPdfReport([FromQuery] string? date)
        {
            return null;
        }

        [HttpGet("late/range")]
        public async Task<IActionResult> GetLateEmployeesRange([FromQuery] string? managerId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var lateSummaries = await _reportService.GetLateEmployeesRangeGroupedAsync(managerId, startDate, endDate);

                // Transform to ensure proper JSON serialization
                var result = lateSummaries.Select(summary => new
                {
                    employeeId = summary.EmployeeId,
                    employeeName = summary.EmployeeName,
                    department = summary.Department,
                    managerId = summary.ManagerId,
                    lateRecords = summary.LateRecords.Select(record => new
                    {
                        date = record.Date.ToString("yyyy-MM-dd"),
                        lateBy = record.LateBy.ToString(@"hh\:mm\:ss"),
                        lateMinutes = record.LateMinutes,
                        checkInTime = record.CheckInTime?.ToString("yyyy-MM-ddTHH:mm:ss")
                    }),
                    totalLateMinutes = summary.TotalLateMinutes
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting late employees: {ex.Message}");
                return StatusCode(500, new { error = "Failed to get late employees" });
            }
        }

        // GET: api/report/late/today?managerId=MGR001
        [HttpGet("late/today")]
        public async Task<IActionResult> GetLateEmployeesToday([FromQuery] string? managerId)
        {
            try
            {
                var lateEmployees = await _reportService.GetLateEmployeesTodayAsync(managerId);
                return Ok(lateEmployees);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting late employees: {ex.Message}");
                return StatusCode(500, new { error = "Failed to get late employees" });
            }
        }

        // GET: api/report/absent/today?managerId=MGR001
        [HttpGet("absent/today")]
        public async Task<IActionResult> GetAbsentEmployeesToday([FromQuery] string? managerId)
        {
            try
            {
                var today = DateTime.Today;
                var absentEmployees = await _reportService.GetAbsentEmployeesAsync(managerId, today, today);
                return Ok(absentEmployees);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting absent employees: {ex.Message}");
                return StatusCode(500, new { error = "Failed to get absent employees" });
            }
        }

        [HttpGet("absent/range")]
        public async Task<IActionResult> GetAbsentEmployeesRange([FromQuery] string? managerId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var absentEmployees = await _reportService.GetAbsentEmployeesAsync(managerId, startDate, endDate);
                return Ok(absentEmployees);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting absent employees: {ex.Message}");
                return StatusCode(500, new { error = "Failed to get absent employees" });
            }
        }
        // Export call
        [HttpGet("getExportByManager")]
        public async Task<IActionResult> GetExportReportByManager([FromQuery] DateTime date, string reportType, string managerId)
        {
            try
            {
                var getReport = await _reportService.GetExportWorkByManager(date, reportType, managerId);
                //return Ok(getReport);
                return File(getReport, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MyReport.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // Export call
        //[HttpGet("getExportByEmployee")]
        //public async Task<IActionResult> GetExportReportByEmployee([FromQuery] DateTime date, string reportType, string empId)
        //{
        //    try
        //    {
        //        var getReport = await _reportService.GetExportWorkByEmployee(date, reportType, empId);
        //        //return Ok(getReport);
        //        return File(getReport, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MyReport.xlsx");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { error = ex.Message });
        //    }
        //}

        [HttpGet("getExportByEmployee")]
        public async Task<IActionResult> GetExportReportByEmployee([FromQuery] DateTime startDate,[FromQuery] DateTime endDate,[FromQuery] string reportType,[FromQuery] string empId)
        {
            try
            {
                var getReport = await _reportService.GetExportWorkByEmployee(startDate, endDate, reportType, empId);
                return File(getReport, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MyReport.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("getExcelReport")]
        public async Task<IActionResult> GetExcelCheckInReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, string reportType, string managerId)
        {
            try
            {
                var getReport = await _reportService.GetExcelCheckInReport(startDate, endDate, reportType, managerId);
                return File(getReport, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CheckInOutReport.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}