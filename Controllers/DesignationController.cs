using AttendanceAPI.Interface;
using AttendanceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DesignationController : ControllerBase
    {
        private readonly IDesignationService _service;

        public DesignationController(IDesignationService service)
        {
            _service = service;
        }
        [AllowAnonymous]
        [HttpGet("list")]
        public async Task<IActionResult> GetDesignations()
        {
            try
            {
                var data = await _service.GetDesignationsAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

}
