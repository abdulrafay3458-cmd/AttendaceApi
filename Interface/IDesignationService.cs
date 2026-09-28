using static AttendanceAPI.Services.DesignationService;

namespace AttendanceAPI.Interface
{
    public interface IDesignationService
    {
        Task<List<DesignationResponse>> GetDesignationsAsync();
    }
}
