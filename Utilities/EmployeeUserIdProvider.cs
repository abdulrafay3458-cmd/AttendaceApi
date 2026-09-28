using Microsoft.AspNetCore.SignalR;

namespace AttendanceAPI.Utilities
{
    public class EmployeeUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            var a = connection.User?.FindFirst("employeeId")?.Value;
            return a;
        }
    }
}
