using AttendanceAPI.Data;
using AttendanceAPI.Interface;
using Microsoft.EntityFrameworkCore;
using static AttendanceAPI.Services.DesignationService;

namespace AttendanceAPI.Services
{
    public  class CompanyService : ICompanyService
    {
        public async Task<List<CompaniesResponse>> GetCompaniesAsync()
        {
            using (var context = new AppDbContext())
            {
                var companies = await context.Company
                    .OrderBy(c => c.Code)
                    .Select(c => new CompaniesResponse
                    {
                        Code = c.Code,
                        Name = c.Name
                    })
                    .ToListAsync();

                return companies;
            }
        }

        public class CompaniesResponse
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

    }
}
