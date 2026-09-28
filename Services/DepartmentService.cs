using AttendanceAPI.Data;
using AttendanceAPI.Interface;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Services
{
    public  class DepartmentService : IDepartmentService
    {
        public async Task<List<DepartmentResponse>> GetDepartmentsAsync()
        {
            using (var context = new AppDbContext())
            {
                var departments = await context.Departments
                    .OrderBy(d => d.Name)
                    .Select(d => new DepartmentResponse
                    {
                        Code = d.Code,
                        Name = d.Name
                    })
                    .ToListAsync();

                return departments;
            }
        }
        public class DepartmentResponse
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

    }
}
