using AttendanceAPI.Contracts.Request;
using AttendanceAPI.Entities;
using AttendanceAPI.Interface;
using AttendanceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationImageController : ControllerBase
    {
        private readonly IRegistrationImageService _service;
        public RegistrationImageController(IRegistrationImageService service)
        {
            _service = service;
        }
        [AllowAnonymous]
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyRequst([FromBody] ApplyImageRequest request)
        {
            try
            {
                var addNew = await _service.ApplyRequest(request);
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
        [AllowAnonymous]
        [HttpPost("Approverequest")]
        public async Task<IActionResult> ApproveRequest([FromBody] RegistrationImageRequest request)
        {
            try
            {
                var editReq = await _service.ApproveRequest(request);
                return Ok(editReq);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [AllowAnonymous]
        [HttpGet("getlist")]
        public async Task<IActionResult> GetRequest()
        {
            try
            {
                var ReqList = await _service.GetRequest();
                return Ok(new
                {
                    //success = true,
                    //message = "successfully",
                    data = ReqList
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }
}
