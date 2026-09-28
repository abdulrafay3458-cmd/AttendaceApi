using AttendanceAPI.Contracts;
using AttendanceAPI.Contracts.Request;
using AttendanceAPI.Contracts.Response;
using AttendanceAPI.Data;
using AttendanceAPI.Entities;
using AttendanceAPI.Interface;
using AttendanceAPI.Services;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenCvSharp;
using Serilog;
using System.Threading.Tasks;

namespace AttendanceAPI.Controllers
{
   

    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly NotificationService notificationService;

        public EmployeeController(IEmployeeService employeeService, NotificationService notificationService)
        {
            _employeeService = employeeService;
            this.notificationService = notificationService;
        }

        [HttpGet("dailyRecord")]
        public async Task<ActionResult> GetEmployeeTodayRecord(string employeeId, DateTime date)
        {
            try
            {
                var tasksRecords = await _employeeService.GetEmployeeTodayRecord(employeeId, date);
                return Ok(tasksRecords);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _employeeService.GetEmployeesAsync();
            return Ok(employees);
        }

        [HttpGet("{employeeId}/image-status")]
        public async Task<IActionResult> GetImageStatus(string employeeId)
        {
            bool isApproved = await _employeeService.GetImageStatusAsync(employeeId);

            return Ok(new
            {
                imageApproved = isApproved
            });
        }


        [HttpPost("editEmployeeAndUser")]
        public async Task<IActionResult> UpdateEmployee([FromBody] EmployeeRequest request)
        {
            try
            {
                var addNew = await _employeeService.UpdateEmployee(request);
                return Ok(new
                {
                    success = true,
                    message = "successfully",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("getEmp")]
        public async Task<IActionResult> GetEmployee()
        {
            var context = HttpContext.User.FindFirst("EmployeeId").Value;
            var employee = await _employeeService.GetEmployeeByIdAsync(context);

            if (employee == null)
            {
                return NotFound(new { error = "Employee not found" });
            }

            return Ok(employee);
        }

        [HttpGet("getEmpCurrentInfo")]
        public async Task<IActionResult> GetEmployeeCurrentInfo()
        {
            var context = HttpContext.User.FindFirst("EmployeeId").Value;
            var employee = await _employeeService.GetEmployeeCurrentInfo(context);

            if (employee == null)
            {
                return NotFound(new { error = "Employee not found" });
            }

            return Ok(employee);
        }




        [HttpGet("teamLeadsList")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTeamLeadList()
        {
            try
            {
                var data = await _employeeService.GetTeamLeadsAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{managerId}/team")]
        public async Task<IActionResult> GetTeamMembers(string managerId)
        {
            try
            {
                var teamMembers = await _employeeService.GetTeamMembersAsync(managerId);             

                return Ok(teamMembers);
                //return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet("employeeWorkingDays/{employeeCode}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEmployeeWorkingDays(string employeeCode)
        {
            try
            {
                var workingDays = await _employeeService.GetWorkingDays(employeeCode);
                return Ok(workingDays);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            } 
        }

        [HttpPost("requesttoregisteruserdevice")]
        public async Task<IActionResult> RegisterUserDevice([FromBody] RegisterUserDeviceRequest request)
        {
            try
            {
                var EmpId = HttpContext.User.FindFirst("employeeId").Value;
                request.EmployeeId = EmpId;
                var response = await _employeeService.RegisterUserDeviceAsync(request);

                await notificationService.SendFcmNotificationAsync(response["AdminCode"], response["FcmToken"],
                                            "New request to register device", $"{response["EmpName"]} requested to register new device");
                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled Exception");
                return StatusCode(500);
            }
        }

        [HttpPost("approveusernewdevice")]
        public async Task<IActionResult> ApproveUserDevice(ApproveUserDeviceRequest request)
        {
            try
            {
                var context = HttpContext.User.FindFirst("employeeId").Value;
                var response = await _employeeService.ApproveUserDeviceAsync(request, context);

                if (response) { return Ok(); }
                else { return BadRequest(); }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled Exception");
                return StatusCode(500);
            }
        }

        [HttpGet("requestsfornewuserdevices")]
        public async Task<IActionResult> GetRequestsForNewUserDevices()
        {
            try
            {
                var response = await _employeeService.GetRequestsForNewUserDevicesAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled Exception");
                return StatusCode(500);
            }
        }

        [HttpGet("all-registered-devices")]
        public async Task<IActionResult> GetAllUsersWithTheirDevices()
        {
            try
            {
                var response = await _employeeService.GetAllUsersWithTheirDevicesAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled Exception");
                return StatusCode(500);
            }
        }

        [HttpDelete("remove-device")]
        public async Task<IActionResult> RemoveCurrentUserDevice([FromQuery] string empId)
        {
            try
            {
                var response = await _employeeService.RemoveCurrentUserDeviceAsync(empId);

                if (response) { return Ok(); }
                else { return BadRequest(); }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled Exception");
                return StatusCode(500);
            }
        }

        //[HttpPost("{employeeId}/fcm-token")]
        //public async Task<IActionResult> UpdateFcmToken(string employeeId, [FromBody] FcmTokenRequest request)
        //{
        //    try
        //    {
        //        var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
        //        if (employee == null)
        //            return NotFound(new { error = "Employee not found" });

        //        employee.FcmToken = request.FcmToken;
        //        await _storage.UpdateEmployeeAsync(employee);

        //        return Ok(new { success = true, message = "FCM token updated" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { error = ex.Message });
        //    }
        //}

        //public class FcmTokenRequest
        //{
        //    public string FcmToken { get; set; } = "";
        //}
    }
}