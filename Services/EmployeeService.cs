using AttendanceAPI.Contracts.Request;
using AttendanceAPI.Contracts.Response;
using AttendanceAPI.Data;
using AttendanceAPI.Entities;
using AttendanceAPI.Interface;
using DocumentFormat.OpenXml.InkML;
using Mapster;
using Microsoft.EntityFrameworkCore;
using OpenCvSharp;

namespace AttendanceAPI.Services
{
    public class EmployeeService : IEmployeeService
    {
        public async Task<List<TeamLead>> GetTeamLeadsAsync()
        {
            using (var context = new AppDbContext())
            {
                var teamLeads = await context.Employees
                    .OrderBy(d => d.Name)
                    .ProjectToType<TeamLead>()  // ✅ Mapster projection
                    .ToListAsync();

                return teamLeads;
            }
        }
        public async Task<List<Employee>> GetEmployeesAsync()
        {
            //var filePath = Path.Combine(_dataDirectory, "employees.json");
            //if (!File.Exists(filePath))
            //{
            //    return new List<Employee>();
            //}

            //var json = await File.ReadAllTextAsync(filePath);
            //return JsonSerializer.Deserialize<List<Employee>>(json, _jsonOptions) ?? new List<Employee>();

            using (var context = new AppDbContext())
            {
                return await context.Employees.ToListAsync();
            }
        }
        public async Task<Employee?> GetEmployeeByZkUserIdAsync(string zkUserId)
        {
            using (var context = new AppDbContext())
            {
                var s = await context.Employees.FirstOrDefaultAsync(a => a.ZkUserId == zkUserId);
                return s == null ? null : s;
            }
        }
        public async Task<Employee?> GetEmployeeByIdAsync(string employeeId)
        {
            using (var context = new AppDbContext())
            {
                var s = await context.Employees.FirstOrDefaultAsync(a => a.Code == employeeId);
                return s == null ? null : s;
            }

        }

        public async Task<TaskReportDto> GetEmployeeTodayRecord(string employeeCode, DateTime date)
        {
            using (var context = new AppDbContext())
            {
                //var leaveReq = await (from ld in context.LeaveRequisitionDetails
                //                      join lm in context.LeaveRequisitionMaster on ld.MasterId equals lm.Id
                //                      where lm.EmployeeCode == employeeCode && ld.FromDate.Date == date.Date
                //                      select lm).FirstOrDefaultAsync();

                bool leaveReq = await context.AttendanceLeaves.AnyAsync(e => e.EmployeeCode == employeeCode && e.Date.Date == date.Date);

                var attendance = await context.AttendanceRecords.FirstOrDefaultAsync(a => a.EmployeeCode == employeeCode && a.Day == date.Day && a.Month==date.Month && a.Year == date.Year);

                //var task = await context.Tasks.Where(t => t.AssignedTo == employeeCode)

                var result = await (
                    // LEFT JOIN Tasks
                    from a in context.Tasks

                        // LEFT JOIN TaskTimeLogs (filter by date inside join)
                    join b in context.TaskTimeLogs
                        on a.Id equals b.TaskId into timeLogGroup
                    from b in timeLogGroup
                        .Where(x => x.LogDate.Date == date.Date)
                        .DefaultIfEmpty()

                        //where (b.LogDate.Date == date.Date && a.AssignedTo == employeeCode)
                    where (a.AssignedTo == employeeCode && b.LogDate.Date == date.Date)

                    group b by new
                    {
                        a.Id,
                        a.Title,
                        a.Status
                    } into g

                    select new TaskRecord
                    {
                        TaskTitle = g.Key.Title,
                        TaskStatus = g.Key.Status,
                        TotalHoursWorked = g.Sum(x => x != null ? x.HoursWorked : 0)
                    }).ToListAsync();

                //group b by new
                //{
                //    Title = a != null ? a.Title : null,
                //    Status = a != null ? a.Status : null,
                //}
                //into g

                //select new TaskReportDto
                //{
                //    TaskTitle = g.Key.Title,
                //    TaskStatus = g.Key.Status,
                //    CheckIn = attendance.CheckInTime ?? null,
                //    CheckOut = attendance.CheckOutTime ?? null,
                //    IsLeave = leaveReq,
                //    TotalHoursWorked = g.Sum(x => x != null ? x.HoursWorked : 0)
                //}
                var data = new TaskReportDto
                {
                    CheckIn = attendance?.CheckInTime,
                    CheckOut = attendance?.CheckOutTime,
                    IsLeave = leaveReq,
                    TaskList = result
                };

                return data;
            }
        }
        public async Task<List<TeamMemberDto>> GetTeamMembersAsync(string managerId)
        {
            using (var context = new AppDbContext())
            {
                var role = await (from u in context.AppUsers
                                  join ur in context.UserRoles on u.Code equals ur.UserCode
                                  join rt in context.RoleTypes on ur.RoleId equals rt.Id
                                  where u.EmployeeCode == managerId
                                  select rt.Title.ToLower())
                               .ToListAsync();

                var baseQuery = from e in context.Employees
                                join u in context.AppUsers on e.Code equals u.EmployeeCode into eu
                                from u in eu.DefaultIfEmpty()
                                select new { e, u };

                var employees = await baseQuery.ToListAsync();

                var userCodes = employees.Where(x => x.u != null).Select(x => x.u.Code).Distinct().ToList();
                var rolesByUser = await context.UserRoles
                    .Where(r => userCodes.Contains(r.UserCode))
                    .ToListAsync();
                var rolesLookup = rolesByUser.GroupBy(r => r.UserCode)
                                             .ToDictionary(g => g.Key, g => g.Select(r => r.RoleId).ToList());

                var query = (from e in context.Employees
                             join u in context.AppUsers on e.Code equals u.EmployeeCode
                              into eu
                             from u in eu.DefaultIfEmpty()
                             join r in context.UserRoles on u.Code equals r.UserCode
                              into ru
                             from r in ru.DefaultIfEmpty()
                             select new TeamMemberDto
                             {
                                 EmployeeId = e.Code,
                                 MachineId = e.ZkUserId,
                                 Name = e.Name,
                                 Email = e.Email,
                                 Phone = e.Phone,
                                 Department = e.DepartmentCode,
                                 DesignationCode = e.DesignationCode,
                                 CompanyCode = e.CompanyCode,
                                 StandardHourCode = e.StandardHourCode,
                                 LeadCode = e.LeadCode,
                                 JoinDate = e.JoinDate,
                                 isActive = e.IsActive,
                                 RoleId = u != null && rolesLookup.ContainsKey(u.Code)
                                            ? string.Join(",", rolesLookup[u.Code])
                                            : null,
                                 IsAppUser = e.IsAppUser,
                                 DesignationName = e.Designation.Name
                             });

                // FIX: Only filter if user is a manager (and not admin)
                if (role.Contains("manager") && !role.Contains("admin"))
                {
                    query = query.Where(e => e.LeadCode == managerId);
                }

                // FIX: If user is neither admin nor manager, return empty list
                if (!role.Contains("admin") && !role.Contains("manager"))
                {
                    return new List<TeamMemberDto>();
                }

                return await query.ToListAsync();
            }
        }

        public async Task<bool> GetImageStatusAsync(string employeeId)
        {
            using (var context = new AppDbContext())
            {
                var isImagePending = await context.RegistrationImages.AnyAsync(r =>
                    r.EmployeeCode == employeeId &&
                    (r.Status == false && r.HistoryStatus == false));

                return !isImagePending;
            }
        }

        public async Task<Employee> UpdateEmployee(EmployeeRequest employee)
        {
            using (var context = new AppDbContext())
            {
                try
                {
                    var userId = await context.AppUsers.FirstOrDefaultAsync(a => a.EmployeeCode == employee.EmployeeId);
                    if (userId == null)
                    {
                        throw new Exception("Employee not found");
                    }

                    var existingRoles = await context.UserRoles
                        .Where(a => a.UserCode == userId.Code)
                        .ToListAsync();

                    var existingRoleCodes = existingRoles.Select(r => r.RoleId).ToHashSet();
                    // if (employee.RoleId.Count > 0) {                 
                    foreach (var roleId in employee.RoleId)
                    {
                        if (!existingRoleCodes.Contains(roleId))
                        {
                            context.UserRoles.Add(new Entities.UserRole
                            {
                                UserCode = userId.Code,
                                RoleId = roleId
                            });
                        }
                    }
                    //  }

                    var isEmpExists = await context.Employees.FirstOrDefaultAsync(c => c.Code == employee.EmployeeId);
                    var isUserExist = await context.AppUsers.FirstOrDefaultAsync(c => c.EmployeeCode == employee.EmployeeId);

                    if (isEmpExists != null)
                    {
                        // ✅ Mapster maps matching properties automatically
                        employee.Adapt(isEmpExists);

                        // Manual overrides for properties with different names
                        isEmpExists.Name = employee.FirstName + " " + employee.LastName;
                        isEmpExists.LeadCode = employee.ManagerId;
                        //isEmpExists.DepartmentCode = employee.Department;
                        isEmpExists.StandardHourCode = employee.StandardHour;

                        context.Employees.Update(isEmpExists);
                    }

                    if (isUserExist != null)
                    {
                        isUserExist.Name = employee.FirstName + " " + employee.LastName;
                        isUserExist.IsActive = employee.IsActive;

                        if (employee.IsAppUser == false)
                        {
                            isUserExist.IsActive = false;
                        }
                    }

                    await context.SaveChangesAsync();
                    return isEmpExists;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<List<WorkingDays>> GetWorkingDays(string employeeCode)
        {
            using (var context = new AppDbContext()){
                var employee =await context.Employees
                  .FirstOrDefaultAsync(e => e.Code == employeeCode);

                if (employee == null)
                    return null;

                var workingDays =await context.StandardHours
                    .Where(s => s.GroupCode == employee.StandardHourCode)
                    .Select(s => new WorkingDays
                    {
                        WeekDays = s.WeekDay,
                        minutes = s.Minutes
                    })
                    .ToListAsync();
                return workingDays;
            }
        }

        public async Task<Dictionary<string, string>> RegisterUserDeviceAsync(RegisterUserDeviceRequest request)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var existingRecord = await (from a in context.AppUsers
                                                join b in context.UserDevices on a.EmployeeCode equals b.EmployeeCode
                                                where a.EmployeeCode == request.EmployeeId
                                                select b).FirstOrDefaultAsync();
                    if (existingRecord == null)
                    {
                        await context.UserDevices.AddAsync(new UserDevice()
                        {
                            EmployeeCode = request.EmployeeId,
                            DeviceHash = request.DeviceHash,
                            DeviceManufacturer = request.DeviceManufacturer,
                            DeviceModel = request.DeviceModel,
                            RequestedAt = DateTime.Now
                        });
                    }
                    else
                    {
                        existingRecord.DeviceHash = request.DeviceHash;
                        existingRecord.DeviceManufacturer = request.DeviceManufacturer;
                        existingRecord.DeviceModel = request.DeviceModel;
                        existingRecord.isApproved = null;
                        existingRecord.RequestedAt = DateTime.Now;

                        context.UserDevices.Update(existingRecord);
                    }
                    await context.SaveChangesAsync();

                    var adminInfo = await (from a in context.AppUsers
                                           join b in context.Employees on a.EmployeeCode equals b.Code
                                           join c in context.UserRoles on a.Code equals c.UserCode
                                           join d in context.RoleTypes on c.RoleId equals d.Id
                                           where d.Title == "Admin"
                                           select new
                                           {
                                               Code = b.Code,
                                               FcmToken = b.FcmToken
                                           }).FirstOrDefaultAsync();

                    var EmpName = await context.Employees.Where(c => c.Code == request.EmployeeId).Select(c => c.Name).FirstOrDefaultAsync();

                    return new Dictionary<string, string>()
                    {
                         { "EmpName", EmpName },
                         { "AdminCode", adminInfo.Code },
                         { "FcmToken", adminInfo.FcmToken },
                    };
                }

            } catch (Exception e)
            {
                throw;
            }
        }

        public async Task<bool> ApproveUserDeviceAsync(ApproveUserDeviceRequest request, string approvedBy)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var userRegisteredDevice = await context.UserDevices.FirstOrDefaultAsync(c => c.EmployeeCode == request.requestId);

                    if (userRegisteredDevice == null) throw new Exception("Employee device not found.");
                    userRegisteredDevice.isApproved = request.isApprove;

                    if (request.isApprove)
                    {
                        userRegisteredDevice.ApprovedAt = DateTime.Now;
                        userRegisteredDevice.ApprovedBy = approvedBy;
                    } else
                    {
                        userRegisteredDevice.RejectedAt = DateTime.Now;
                        userRegisteredDevice.RejectedBy = approvedBy;
                        userRegisteredDevice.RejectedReason = request.reason;
                    }
                    
                    context.UserDevices.Update(userRegisteredDevice);
                    await context.SaveChangesAsync();

                    return true;
                }
            } catch (Exception e) { throw; }
        }

        public async Task<List<UserDevicesResponse>> GetRequestsForNewUserDevicesAsync()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    return await (from a in context.Employees
                                  join b in context.UserDevices on a.Code equals b.EmployeeCode
                                  where b.isApproved == null
                                  select new UserDevicesResponse()
                                  {
                                      EmployeeCode = a.Code,
                                      EmployeeName = a.Name,
                                      DeviceManufacturer = b.DeviceManufacturer,
                                      DeviceModel = b.DeviceModel,
                                  }).ToListAsync();
                }
            }
            catch (Exception e) { throw; }
        }

        public async Task<List<UserDevicesResponse>> GetAllUsersWithTheirDevicesAsync()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    return await (from a in context.Employees
                                  join b in context.UserDevices on a.Code equals b.EmployeeCode
                                  where b.isApproved == true
                                  select new UserDevicesResponse()
                                  {
                                      EmployeeCode = a.Code,
                                      EmployeeName = a.Name,
                                      DeviceManufacturer = b.DeviceManufacturer,
                                      DeviceModel = b.DeviceModel,
                                  }).ToListAsync();
                }
            }
            catch (Exception e) { throw; }
        }

        public async Task<bool> RemoveCurrentUserDeviceAsync(string EmpId)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var userRegisteredDevice = await context.UserDevices.FirstOrDefaultAsync(c => c.EmployeeCode == EmpId);

                    if (userRegisteredDevice == null) throw new Exception("User device not found.");
                    
                    context.UserDevices.Remove(userRegisteredDevice);
                    await context.SaveChangesAsync();

                    return true;
                }
            }
            catch (Exception e) { throw; }
        }

        public async Task<EmployeeDTO?> GetEmployeeCurrentInfo(string employeeId)
        {
            using (var context = new AppDbContext())
            {
                var userInfo = context.AppUsers.FirstOrDefault(s => s.EmployeeCode == employeeId);
                var userRole = await (from a in context.UserRoles
                                      join b in context.RoleTypes on a.RoleId equals b.Id
                                      where a.UserCode == userInfo.Code
                                      select b.Title).ToListAsync();

                var isImagePending = await context.RegistrationImages.AnyAsync(r =>
                    r.EmployeeCode == userInfo.EmployeeCode &&
                    r.Status == false &&
                r.HistoryStatus == false);

                var registeredDevice = await context.UserDevices.FirstOrDefaultAsync(c => c.EmployeeCode == userInfo.EmployeeCode);

                var empInfo = await (from a in context.Employees
                                     join b in context.Departments on a.DepartmentCode equals b.Code
                                     join c in context.Designations on a.DesignationCode equals c.Code
                                     join d in context.FaceImages on a.Code equals d.EmpCode into ad
                                     from d in ad.DefaultIfEmpty()
                                     where a.Code == userInfo.EmployeeCode
                                     select new EmployeeDTO
                                     {
                                         employeeId = a.Code,
                                         employeeCode = userInfo.Code,
                                         name = a.Name,
                                         faceImageBase64 = d.EmpFaceImage ?? "",
                                         email = a.Email,
                                         department = b.Name,
                                         designation = c.Name,
                                         role = userRole,
                                         CompanyCode = a.CompanyCode,
                                         imageApproved = !isImagePending,
                                         isDeviceRegistered = registeredDevice != null && registeredDevice.isApproved == true,
                                         AnyDeviceRegistered = registeredDevice != null && registeredDevice.isApproved == null
                                     }).FirstOrDefaultAsync();
                return empInfo;
            }
        }

        public async Task<bool> IsAuthorizeUserAndDevice(string employeeCode, string deviceHash)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var record = await context.UserDevices.Where(c => c.EmployeeCode == employeeCode && c.DeviceHash == deviceHash)
                        .Select(c => c.isApproved).FirstOrDefaultAsync();
                    if (record != null && record == true) return true;
                    return false;
                }
            }catch (Exception e)
            {
                throw;
            }
        }
    }
}