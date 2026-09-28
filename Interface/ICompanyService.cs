using static AttendanceAPI.Services.CompanyService;
using static AttendanceAPI.Services.DesignationService;

namespace AttendanceAPI.Interface
{
    public interface ICompanyService
    {
        Task<List<CompaniesResponse>> GetCompaniesAsync();
    }
}
