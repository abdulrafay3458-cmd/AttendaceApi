using AttendanceAPI.Contracts.Request;
using AttendanceAPI.Contracts.Response;
using AttendanceAPI.Entities;

namespace AttendanceAPI.Interface
{
    public interface IIdentityService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<bool> RegisterFcmTokenAsync(string empId, string token);
        Task<LoginResponse> RefreshAsync(string RefreshToken, string UserCode, string deviceHash);
        Task<string> CreateUserAsync(CreateUserRequest request);
        Task<bool> ResetPassword(ForgetPasswordRequest request);
        Task<bool> VerifyOTP(ForgetPasswordRequest request);
        Task<List<Contracts.Request.UserRole>> GetUserRole();
        Task<List<StandardHourGroup>> GetStandardHour();
        Task<ChangePasswordResponse> ChangePasswordAsync(ChangePasswordRequest request);
    }
}
