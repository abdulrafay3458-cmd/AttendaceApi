using AttendanceAPI.Services;
using AttendanceAPI.Contracts;
using AttendanceAPI.Contracts.Request;
using AttendanceAPI.Utilities;
using AttendanceAPI.Contracts.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AttendanceAPI.Interface;

namespace AttendanceAPI.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // Assign task (team lead/manager)
        [HttpPost("assign")]
        public async Task<IActionResult> AssignTask([FromBody] AssignTaskRequest request)
        {
            try
            {
                var task = await _taskService.AssignTaskAsync(request);
                return Ok(new
                {
                    success = true,
                    message = "Task assigned successfully",
                    task = task
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("assign-multiple")]
        public async Task<IActionResult> AssignTaskToMultiple([FromBody] AssignTaskMultipleRequest request)
        {
            try
            {
                var tasks = await _taskService.AssignTaskToMultipleAsync(request);
                return Ok(new
                {
                    success = true,
                    message = $"Task assigned to {tasks} users successfully",
                    tasks = tasks
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet("GetPriorities")]
        [AllowAnonymous]
        public IActionResult GetPriorities()
        {
            var priorities = new List<PriorityResponse>
    {
        new PriorityResponse { Title = "low", MinHours = 24, MaxHours = 48 },
        new PriorityResponse { Title = "medium", MinHours = 7, MaxHours = 8 },
        new PriorityResponse { Title = "high", MinHours = 2, MaxHours = 4 }
    };

            return Ok(new
            {
                success = true,
                priorities = priorities
            });
        }
        
        //manager
        //[HttpGet("employee/dailyRecord")]
        //public async Task<IActionResult> GetTaskDailyReport([FromQuery] DateTime? date, [FromQuery] string? managerId, [FromQuery] string? employeeId)
        //{
        //    try
        //    {
        //        var taskReportDate = date ?? DateTime.Today;
        //        var report = await _taskService.GenerateTaskDailyReportAsync(taskReportDate, managerId);
        //        return Ok(report);
        //    }
        //    catch (Exception ex)
        //    {
        //        //    _logger.LogError($"Error generating daily report: {ex.Message}");
        //        return StatusCode(500, new { error = "Failed to generate report" });
        //    }
        //}

        [HttpGet("task-report")]
        public async Task<IActionResult> ExportTaskReport([FromQuery] TaskExportRequest request)
        {
            try
            {
                if (!request.StartDate.HasValue || !request.EndDate.HasValue)
                {
                    return BadRequest(new { error = "StartDate and EndDate are required" });
                }

                if (string.IsNullOrEmpty(request.ManagerId))
                {
                    return BadRequest(new { error = "ManagerId is required" });
                }

                DateTime startDate = request.StartDate.Value.Date;
                DateTime endDate = request.EndDate.Value.Date.AddDays(1).AddSeconds(-1);

                var data = await _taskService.GetTaskReportDataForExport(
                    startDate,
                    endDate,
                    request.ManagerId,
                    request.EmployeeId
                );

                if (data == null || !data.Any())
                {
                    return NotFound(new { error = "No data found for the selected period" });
                }

                string employeeName = null;
                if (!string.IsNullOrEmpty(request.EmployeeId) && data.Any())
                {
                    employeeName = data.First().EmployeeName;
                }

                // Determine view type based on date range (for backward compatibility with GenerateExcelReport)
                //string viewType = _taskService.DetermineViewType(request.StartDate.Value, request.EndDate.Value);

                var fileBytes = _taskService.GenerateExcelReport(
                    data,
                    request.ViewType,
                    startDate,
                    endDate,
                    employeeName
                );

                string fileName = _taskService.GenerateFileName(request, employeeName);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Export failed: {ex.Message}" });
            }
        }

        //[HttpGet("employee/monthlyRecord")]
        //public async Task<IActionResult> GenerateTaskMonthlyReportAsync([FromQuery] int? year, [FromQuery] int? month, [FromQuery] string? managerId)
        //{
        //    try
        //    {
        //        var reportYear = year ?? DateTime.Now.Year;
        //        var reportMonth = month ?? DateTime.Now.Month;
        //        var report = await _taskService.GenerateTaskMonthlyReportAsync(reportYear, reportMonth, managerId);
        //        return Ok(report);
        //    }
        //    catch (Exception ex)
        //    {
        //        /*_logger.LogError($"Error generating monthly report: {ex.Message}");*/
        //        return StatusCode(500, new { error = "Failed to generate report" });
        //    }
        //}

        //[HttpGet("employee/yearlyRecord")]
        //public async Task<IActionResult> GenerateTaskYearlyReportAsync([FromQuery] int? year, [FromQuery] string? managerId)
        //{
        //    try
        //    {
        //        var reportYear = year ?? DateTime.Now.Year;
        //        var report = await _taskService.GenerateTaskYearlyReportAsync(reportYear, managerId);
        //        return Ok(report);
        //    }
        //    catch (Exception ex)
        //    {
        //        /*                _logger.LogError($"Error generating yearly report: {ex.Message}");*/
        //        return StatusCode(500, new { error = "Failed to generate report" });
        //    }
        //}

        [HttpGet("employee/dateRangeRecord")]
        public async Task<IActionResult> GetDateRangeReport([FromQuery] DateTime startDate,[FromQuery] DateTime endDate,[FromQuery] string managerId)
        {
            try
            {
                DateTime reportStart = startDate.Date;
                DateTime reportEnd = endDate.Date.AddDays(1).AddSeconds(-1);

                var report = await _taskService.GenerateTaskReport(
                    reportStart,
                    reportEnd,
                    managerId
                );

                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to generate report" });
            }
        }

        [HttpPatch("update")]
        public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskRequest request)
        {
            try
            {
                var task = await _taskService.UpdateTaskAsync(request);
                return Ok(new
                {
                    success = true,
                    message = "Task updated successfully",
                    task = task
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPatch("delete")]
        public async Task<IActionResult> DeleteTask([FromBody] DeleteTaskRequest request)
        {
            try
            {
                await _taskService.DeleteTaskAsync(request);
                return Ok(new
                {
                    success = true,
                    message = "Task deleted successfully",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // Get employee's tasks
        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetEmployeeTasks(string employeeId)
        {
            try
            {
                var tasks = await _taskService.GetEmployeeTasksAsync(employeeId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // Get active tasks (for dashboard)
        [HttpGet("employee/{employeeId}/active")]
        public async Task<IActionResult> GetActiveEmployeeTasks(string employeeId)
        {
            try
            {
                var tasks = await _taskService.GetActiveEmployeeTasksAsync(employeeId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet("GetPendingTasks")]
        public async Task<ActionResult<List<PendingTask>>> GetPendingTasks([FromQuery] string userId)
        {
            var tasks = await _taskService.GetPendingTasks(userId);

            return Ok(tasks);
        }
        // Start task
        [HttpPost("start")]
        public async Task<IActionResult> StartTask([FromBody] StartTaskRequest request)
        {
            try
            {
                var task = await _taskService.StartTaskAsync(request.EmployeeId, request.TaskId);
                return Ok(new
                {
                    success = true,
                    message = "Task started",
                    task = task
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpPost("validate-location")]
        public async Task<IActionResult> ValidateLocation([FromBody] ValidateLocationDto dto)
        {
            var task = await _taskService.GetTaskByIdAsync(dto.TaskId);

            if (task == null)
                return NotFound();

            // Not client-side → allow
            if (!string.Equals(task.TaskPreference, "client", StringComparison.OrdinalIgnoreCase))
                return Ok(true);

            if (task.ClientLocation == null)
                return BadRequest("Client location not configured");

            double distance = GeoHelper.CalculateDistance(
                dto.Latitude,
                dto.Longitude,
                task.ClientLocation.Latitude,
                task.ClientLocation.Longitude
            );

            bool allowed = distance <= task.ClientLocation.AllowedRadiusMeters;

            return Ok(allowed);
        }
        // Stop task
        [HttpPost("stop")]
        public async Task<IActionResult> StopTask([FromBody] StopTaskRequest request)
        {
            try
            {
                var task = await _taskService.StopTaskAsync(
                    request.EmployeeId,
                    request.TaskId,
                    request.Notes);

                return Ok(new
                {
                    success = true,
                    message = "Task stopped",
                    task = task,
                    todayHours = task.TimeLogs.LastOrDefault()?.HoursWorked ?? 0,
                    totalHours = task.TotalHoursWorked
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // Complete task
        [HttpPost("{taskId}/complete")]
        public async Task<IActionResult> CompleteTask(string taskId, [FromBody] CompleteTaskRequest request)
        {
            try
            {
                var task = await _taskService.CompleteTaskAsync(request.EmployeeId, taskId);
                return Ok(new
                {
                    success = true,
                    message = "Task completed",
                    task = task
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Get team tasks (for team lead)
        [HttpGet("team/{teamLeadId}")]
        public async Task<IActionResult> GetTeamTasks(string teamLeadId)
        {
            try
            {
                //var allTasks = await _storage.GetTasksAsync();
                //var teamTasks = allTasks
                //    .Where(t => t.AssignedBy == teamLeadId)
                //    .OrderByDescending(t => t.CreatedAt)
                //    .ToList();

                var allTasks = await _taskService.GetTeamLeadTasksAsync(teamLeadId);

                return Ok(allTasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetTaskSummary(string empCode)
        {
            try
            {
                var allTasks = await _taskService.GetEmpTaskSummaryAsync(empCode);

                return Ok(allTasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    public class CompleteTaskRequest
    {
        public string EmployeeId { get; set; } = string.Empty;
    }
}