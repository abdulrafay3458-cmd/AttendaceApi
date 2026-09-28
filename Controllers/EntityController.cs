using AttendanceAPI.Services;
using AttendanceAPI.Interface;
using Microsoft.AspNetCore.Mvc;
using AttendanceAPI.Data;
using AttendanceAPI.Contracts.Request;

namespace AttendanceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntityController : ControllerBase
    {
        private readonly IEntityService _entityService;
        private readonly ILogger<ReportController> _logger;

        public EntityController(IEntityService entityService, ILogger<ReportController> logger)
        {
            _entityService = entityService;
            _logger = logger;
        }

        [HttpPost("addnew")]
        public async Task<IActionResult> AddEntity([FromBody] EntityRequest request)
        {
            try
            {
                var addNew = await _entityService.AddEntityAsync(request);
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

        [HttpGet("all")]
        public async Task<IActionResult> GetEntity()
        {
            try
            {
                var response = await _entityService.GetEntityAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {

                _logger.LogError($"Error getting all clients: {ex.Message}");
                return StatusCode(500, new { error = "Failed to get  all clients" });
            }
        }

        [HttpPost("editEntity")]
        public async Task<IActionResult> EditEntity([FromBody] EntityRequest request)
        {
            try
            {
                var editEntity = await _entityService.EditEntityAsync(request);
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

    }
}
