namespace AttendanceAPI.Controllers
{
    using AttendanceAPI.Contracts.Request;
    using AttendanceAPI.Entities;
    using AttendanceAPI.Interface;
    using AttendanceAPI.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    [ApiController]
    [Route("api/[controller]")]
    public class LeaveController : ControllerBase
    {
        private readonly NotificationService _notificationService;
        private ILeaveService service;


        public LeaveController(
            NotificationService notificationService, ILeaveService leaveService)
        {
            _notificationService = notificationService;
            service = leaveService;
        }
        [HttpGet("{employeeId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLeaveRequests(string employeeId)
        {
            try
            {
                var leaves = await service.GetLeaveRequestByIdAsync(employeeId);
                return Ok(leaves);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Failed to fetch leave requests",
                    error = ex.Message
                });
            }
        }
        [AllowAnonymous]
        [HttpGet("isLeaveToday")]
        public async Task<IActionResult> EmployeeLeaveCheck(string employeeId, string companyCode)
        {
            try
            {
                var leaveCheck = await service.GetCheckTodayLeave(employeeId, companyCode);
                return Ok(leaveCheck);
            }
            catch (Exception e)
            {
                return StatusCode(500, new
                {
                    message = "Failed to fetch leave type..",
                    error = e.Message
                });
            }
        }

        [HttpGet("statistics/{empCode}")]
        public async Task<IActionResult> GetLeaveStats(string empCode)
        {
            try
            {
                var leaveStats = await service.GetLeaveStatisticsAsync(empCode);
                return Ok(leaveStats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Failed to fetch leave stats..",
                    error = ex.Message
                });
            }
        }

        [HttpGet("types")]
        public async Task<IActionResult> GetLeaveTypes()
        {
            try
            {
                var leaveTypes = await service.GetLeaveTypes();

                return Ok(leaveTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Failed to fetch leave type..",
                    error = ex.Message
                });
            }
        }
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyLeave([FromBody] ApplyLeaveRequest request)
        {
            try
            {

                var applyLeaves = await service.Applyleaves(request);

                // Notify employee
                await _notificationService.SendLeaveNotificationAsync(
                    request.EmployeeId,
                    "Leave Request Submitted",
                    $"Your {request.LeaveType} leave request for {applyLeaves.TotalDays} day(s) has been sent to {applyLeaves.ApproverManagerName} for approval"
                );

                ////// Notify manager
                await _notificationService.SendLeaveNotificationAsync(
                    applyLeaves.ApproverManagerId,
                    "New Leave Request",
                    $"{applyLeaves.EmployeeName} requested {request.LeaveType} leave for {applyLeaves.TotalDays} day(s)"
                );

                return Ok(new
                {
                    success = true,
                    message = "Leave request submitted successfully",
                    leaveRequest = applyLeaves,
                    approver = new { id = applyLeaves.ApproverManagerId, name = applyLeaves.ApproverManagerName }
                });
            }
            catch (Exception ex)
            {
                //  return StatusCode(500, new { error = ex.Message });
                throw new Exception(ex.Message);
            }
        }

        // Get pending requests for approval (manager view)
        [HttpGet("pending-approvals/{managerId}")]
        public async Task<IActionResult> GetPendingApprovals(string managerId)
        {
            try
            {
                var requests = await service.GetPendingLeaveRequestsForApproverAsync(managerId);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveLeave(int id, [FromBody] ApproveLeaveRequest request)
        {
            try
            {
                //int leaveId = Convert.ToInt32(id);
                var leaveRequest = await service.GetApproveLeaveByIdAsync(id, request);
                //if (leaveRequest == null)
                //{
                //    return NotFound(new { error = "Leave request not found" });
                //}

                //// Verify approver is the assigned manager
                //if (leaveRequest.ApproverManagerId != request.ApproverId)
                //{
                //    return Unauthorized(new { error = "You are not authorized to approve this request" });
                //}

                var approver = await service.GetEmployeeByIdAsync(request.ApproverId);

                // Notify employee
                await _notificationService.SendLeaveNotificationAsync(
                    leaveRequest.EmployeeId,
                    "Leave Request Approved ✓",
                    $"Your {leaveRequest.LeaveType} leave from {leaveRequest.StartDate:MMM dd} to {leaveRequest.EndDate:MMM dd} has been approved by {approver?.Name}"
                );

                return Ok(new
                {
                    success = true,
                    message = "Leave request approved",
                    leaveRequest = leaveRequest
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectLeave(int id, [FromBody] RejectLeaveRequest request)
        {
            try
            {
                var leaveRequest = await service.GetLeaveRequestForRejectionByIdAsync(id, request);
                //if (leaveRequest == null)
                //{
                //    return NotFound(new { error = "Leave request not found" });
                //}

                //// Verify approver is the assigned manager
                //if (leaveRequest.ApproverEmployeeCode != request.ApproverId)
                //{
                //    return Unauthorized(new { error = "You are not authorized to reject this request" });
                //}

                var approver = await service.GetEmployeeByIdAsync(request.ApproverId);

                // Notify employee
                //await _notificationService.SendLeaveNotificationAsync(
                //    leaveRequest.EmployeeCode,
                //    "Leave Request Rejected",
                //    $"Your {leaveRequest.leaveType} leave request has been rejected by {approver?.Name}. Reason: {request.Reason}"
                //);

                return Ok(new
                {
                    success = true,
                    message = "Leave request rejected",
                    leaveRequest = leaveRequest
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpPost("applycancelLeave")]
        public async Task<IActionResult> CancelLeave(CancelLeaveRequest cancelLeaveRequest)
        {
            try
            {
                var cancelLeave = await service.CancelLeave(cancelLeaveRequest);
                return Ok(new
                {
                    success = true,
                    message = "Leave Cancellation has been Applied",
                    leave = cancelLeave
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpPost("{id}/approvecancelRequest")]
        public async Task<IActionResult> ApproveCancelLeave(string id,string approverId)
        {
            try
            {
                var leaveCancel = await service.ApproveCancelLeaves(id, approverId);
                return Ok(new
                {
                    success = true,
                    message = "Applied Leave Deleted Successfuly",
                    leave = leaveCancel
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet("pending-cancel-approvals/{managerId}")]
        public async Task<IActionResult> GetPendingCancelApprovals(string managerId)
        {
            try
            {
                var requests = await service.GetPendingCancelLeaves(managerId);
                return Ok(new
                {
                    success = true,
                    leave = requests
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("getCancelLeaveDates")]
        public async Task<IActionResult> GetCancelLeaveDates(int no)
        {
            try
            {
                var lists = await service.GetCancelLeaveDatesAsync(no);
                return Ok(lists);
            }
            catch (Exception)
            {

                throw;
            }
            ;
            
        }

        [HttpPost("{id}/rejectCancelLeave")]
        public async Task<IActionResult> RejectCancelLeave(string id, [FromBody] RejectLeaveRequest request)
        {
            try
            {
                var leaveRequest = await service.GetCancelLeaveRequestForRejectionByIdAsync(id, request);
                if (leaveRequest == null)
                {
                    return NotFound(new { error = "Cancel Leave request not found" });
                }

                //// Verify approver is the assigned manager
                if (leaveRequest.ApproverEmployeeCode != request.ApproverId)
                {
                    return Unauthorized(new { error = "You are not authorized to reject this request" });
                }

                var employee = await service.GetEmployeeByIdAsync(leaveRequest.EmployeeCode);
                var approver = await service.GetEmployeeByIdAsync(request.ApproverId);

                // Notify employee
                await _notificationService.SendFcmNotificationAsync(employee.Code, employee.FcmToken,
                            "Leave Cancellation", $"Your Cancellation leave request has been rejected by {approver?.Name}. Reason: {request.Reason}");

                return Ok(new
                {
                    success = true,
                    message = "Leave Cancellation has been reject",
                    leaveRequest = leaveRequest
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}