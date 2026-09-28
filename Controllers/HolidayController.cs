using AttendanceAPI.Contracts;
using AttendanceAPI.Contracts.Request;
using AttendanceAPI.Entities;
using AttendanceAPI.Interface;
using AttendanceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.EnvironmentVariables;

namespace AttendanceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HolidayController : ControllerBase
    {
        private readonly NotificationService _notificationService;
        private readonly IHolidayService _holidayService;

        public HolidayController(NotificationService notificationService, IHolidayService holidayService) 
        {
            _notificationService = notificationService;
            _holidayService = holidayService;

        }

        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteHoliday([FromBody] DateTime dateTime)
        {
            try
            {
                var deleteHoliday = await _holidayService.DeleteHoliday(dateTime);
                return Ok(new
                {
                    success = true,
                    message = "All Holidays",
                    Holidays = deleteHoliday
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetOffDay")]
        public async Task<IActionResult> GetEmployeeOffDay(string empCode, DateTime date)
        {
            try
            {
                var offDay = await _holidayService.GetOffDay(empCode, date);
                return Ok(offDay);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetHolidays")]
        public async Task<IActionResult> GetAllHolidays()
        {
            try
            {
                var holidays = await _holidayService.GetHolidays();
                return Ok(new
                {
                    success = true,
                    message = "All Holidays",
                    Holidays = holidays
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("Save")]
        [AllowAnonymous]
        public async Task<IActionResult> SaveHoliday([FromBody] HolidayRequest holidayRequest)
        {
            try
            {
                var holiday = await _holidayService.Save(holidayRequest);
                return Ok(new
                {
                    success = true,
                    Holiday = holidayRequest,
                    message = "Holiday Save Successfully",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("Update")]
        public async Task<IActionResult> UpdateHoliday([FromBody] HolidayRequest holidayRequest)
        {
            try
            {
                var holiday = await _holidayService.UpdateHoliday(holidayRequest);
                return Ok(new
                {
                    success = true,
                    Holiday = holidayRequest,
                    message = "Holiday Update Successfully",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
