using AttendanceAPI.Contracts.Request;
using AttendanceAPI.Interface;
using AttendanceAPI.Migrations;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AttendanceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskOverTimeController : ControllerBase
    {
        private readonly ITaskOvertimeService _overtimeService;

        public TaskOverTimeController(
            ITaskOvertimeService overtimeService)
        {
            _overtimeService = overtimeService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddOvertime(
            [FromBody] AddOvertimeRequest request)
        {
            try
            {
                var employeeId = User.FindFirst(
                    ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(employeeId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "User not authenticated"
                    });
                }               

                var overtime =
                    await _overtimeService.AddOvertimeAsync(
                        request,
                        employeeId);

                return Ok(new
                {
                    success = true,
                    message = "Overtime added successfully",
                    overtime = overtime
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateOvertime([FromBody] AddOvertimeRequest request)
        {
            try
            {
                var employeeId = User.FindFirst(
                    ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(employeeId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "User not authenticated"
                    });
                }

                var message = await _overtimeService
                    .UpdateOvertimeAsync(request);

                if (message == false)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Overtime record not found."
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Overtime updated successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }
        [HttpDelete("delete/{overtimeId}")]
        public async Task<IActionResult> DeleteOvertime(Guid overtimeId)
        {
            try
            {
                var message = await _overtimeService
                    .DeleteOvertimeAsync(overtimeId);

                return Ok(new
                {
                    success = true,
                    message = message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        [HttpPost("approve")]
        public async Task<IActionResult> ApproveOvertime(Guid taskId,DateTime overTimeDate)
        {
            var leadCode = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(leadCode))
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }

            var (success, error) = await _overtimeService.ApprovedOvertimeRequestAsync(leadCode, taskId,overTimeDate);

            if (success)
            {
                return Ok(new { success = true, message = "Overtime approved successfully" });
            }

            return BadRequest(new { success = false, error });
        }

        [HttpGet("pending-overtime-approvals/{leadCode}")]
        public async Task<IActionResult> GetPendingRequest(string leadCode)
        {
            try
            {
                var employeeId = User.FindFirst(
                    ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(employeeId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "User not authenticated"
                    });
                }
                var overtime =
                    await _overtimeService.GetPendingRequest(leadCode);
                return Ok(new
                {
                    success = true,
                    pendingOverTime = overtime
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }
        public class RejectOvertimeRequest
        {
            public Guid TaskId { get; set; }
            public string? Reason { get; set; }
            public DateTime overTimeDate { get; set; }
        }

        [HttpPost("reject")]
        public async Task<IActionResult> RejectOvertime([FromBody] RejectOvertimeRequest request)
        {
            var leadCode = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(leadCode))
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }

            var (success, error) = await _overtimeService.RejectOvertimeRequestAsync(leadCode, request.TaskId, request.Reason ,request.overTimeDate);

            if (success)
            {
                return Ok(new { success = true, message = "Overtime rejected successfully" });
            }

            return BadRequest(new { success = false, error });
        }
    }
}
