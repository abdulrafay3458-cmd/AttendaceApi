using AttendanceAPI.Contracts.Response;
using AttendanceAPI.Entities;

namespace AttendanceAPI.Interface
{
    public interface IZKTecoService
    {
        Task ConnectToDevices();
        Task<bool> ConnectToDevice(ZKTecoDevice device);
        Task<List<AttendanceRecord>> GetAttendanceFromDevice(ZKTecoDevice device, int noOfDays);
        Task<List<Employee>> SyncUsersFromDevice(Guid deviceId);
        Task DisconnectAll();

    }
}
