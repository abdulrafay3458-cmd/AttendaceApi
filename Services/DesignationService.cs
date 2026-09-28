using AttendanceAPI.Data;
using AttendanceAPI.Interface;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Services
{
    public  class DesignationService : IDesignationService
    {
        public async Task<List<DesignationResponse>> GetDesignationsAsync()
        {
            using (var context = new AppDbContext())
            {
                var designations = await context.Designations
                    .OrderBy(d => d.Name)
                    .Select(d => new DesignationResponse
                    {
                        DesignationCode = d.Code,
                        DesignationName = d.Name
                    })
                    .ToListAsync();

                return designations;
            }
        }
        public class DesignationResponse
        {
            public int DesignationCode { get; set; }
            public string DesignationName { get; set; }
        }

    }
}
