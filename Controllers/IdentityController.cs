namespace AttendanceAPI.Controllers
{
    using AttendanceAPI.Contracts.Request;
    using AttendanceAPI.Contracts.Response;
    using AttendanceAPI.Data;
    using AttendanceAPI.Entities;
    using AttendanceAPI.Interface;
    using AttendanceAPI.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService service;

        public IdentityController(IIdentityService service)
        {
            this.service = service;
        }
        
        [AllowAnonymous]
        [HttpPost("verifyopt")] 
        public async Task<ActionResult> verifyopt([FromBody] ForgetPasswordRequest request)
        {
            var response = await service.VerifyOTP(request);
            return Ok(response);
        }
        [AllowAnonymous]
        [HttpPost("resetpassword")]
        public async Task<ActionResult> ResetPassword([FromBody] ForgetPasswordRequest request)
        {
            var response = await service.ResetPassword(request);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await service.LoginAsync(request);

                if (response.Success)
                {
                    if (response.RefreshToken != null)
                    Response.Cookies.Append("refresh_token", response.RefreshToken!, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddDays(7)
                    });

                    //response.RefreshToken = "";

                    return Ok(response);
                }
                //else if(employee.FcmToken == request.Password)
                //{
                //    var token = GenerateSimpleToken(employee.Id.ToString());

                //    return Ok(new LoginResponse
                //    {
                //        Success = true,
                //        Message = "Login successful",
                //        //Employee = employee,
                //        Token = token
                //    });
                //}
                else
                {
                    return BadRequest(response);
                }
            } catch (Exception e)
            {
                return BadRequest();
            }
                
        }
        [AllowAnonymous]
        [HttpGet("getUserRoles")]
        public async Task<IActionResult> GetUserRoles()
        {
            try
            {
                var result = await service.GetUserRole();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [AllowAnonymous]
        [HttpGet("getStandardGroup")]
        public async Task<IActionResult> GetStandardGroup()
        {
            try
            {
                var result = await service.GetStandardHour();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }



        public class RegisterFcmTokenRequest
        {
            public string? FcmToken { get; set; }
        }

        [HttpPost("registerfcmtoken")]
        public async Task<ActionResult> RegisterFcmToken(RegisterFcmTokenRequest request)
        {
            try
            {
                if (request.FcmToken == null) return BadRequest("Token not found");
                var empId = HttpContext.User?.FindFirst("employeeId").Value;
                var response = await service.RegisterFcmTokenAsync(empId, request.FcmToken);
                if (response)
                {
                    return Ok("FCM Token has been registered");
                } else { return BadRequest("Employee not found"); }
            } catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<ActionResult<LoginResponse>> Refresh()
        {
            var refreshToken = Request.Headers["refreshtoken"].FirstOrDefault();
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            var employeeId = string.Empty;
            var deviceHash = string.Empty;
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                employeeId = jwtToken.Claims
                    .FirstOrDefault(c => c.Type == "employeeId")?.Value;
                deviceHash = jwtToken.Claims
                    .FirstOrDefault(c => c.Type == "deviceHash")?.Value;
            }
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new { message = "Refresh token missing" });
            }

            var response = await service.RefreshAsync(refreshToken, employeeId, deviceHash);

            if (!response.Success)
            {
                return Unauthorized(response);
            }

            Response.Cookies.Append("refresh_token", response.RefreshToken!, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            //response.RefreshToken = null;

            return Ok(response);
        }



        [AllowAnonymous]
        [HttpPost("createuser")]
        public async Task<ActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var response = await service.CreateUserAsync(request);
            return Ok(response);
        }

        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var user = HttpContext.User.FindFirst("employeeId").Value;
                request.RequesterCode = user;
                var response = await service.ChangePasswordAsync(request);
                if (response.Success)
                {
                    return Ok(response);
                } else { return BadRequest(response); }
            }catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        //[HttpPost("fcm-token")]
        //public async Task<IActionResult> UpdateFcmToken([FromBody] UpdateFcmTokenRequest request)
        //{
        //    var employees = await _storage.GetEmployeesAsync();
        //    var employee = employees.FirstOrDefault(e => e.Id.ToString() == request.EmployeeId);

        //    if (employee != null)
        //    {
        //        employee.FcmToken = request.FcmToken;
        //        //await _storage.SaveEmployeesAsync(employees);
        //        return Ok(new { success = true });
        //    }

        //    return NotFound(new { error = "Employee not found" });
        //}
    }

    public class UpdateFcmTokenRequest
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string FcmToken { get; set; } = string.Empty;
    }
}