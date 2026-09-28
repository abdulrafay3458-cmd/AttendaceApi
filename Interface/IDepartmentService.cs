using static AttendanceAPI.Services.DepartmentService;
using static AttendanceAPI.Services.DesignationService;

namespace AttendanceAPI.Interface
{
    public interface IDepartmentService
    {
        Task<List<DepartmentResponse>> GetDepartmentsAsync();
    }
}
