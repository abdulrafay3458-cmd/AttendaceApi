using AttendanceAPI.Contracts.Request;
using AttendanceAPI.Entities;

namespace AttendanceAPI.Contracts.Response
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public EmployeeDTO? Employee { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public bool? isAuthDevice { get; set; }
    }
}
