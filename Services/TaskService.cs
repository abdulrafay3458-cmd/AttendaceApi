namespace AttendanceAPI.Services
{
    using AttendanceAPI.Contracts.Request;
    using AttendanceAPI.Contracts.Response;
    using AttendanceAPI.Data;
    using AttendanceAPI.Entities;
    using AttendanceAPI.Interface;
    using Azure.Core;
    using ClosedXML.Excel;
    using DocumentFormat.OpenXml.InkML;
    using DocumentFormat.OpenXml.Vml.Office;
    using Mapster;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Management;
    using System.Security.Principal;
    using System.Threading.Tasks;
  

    public class TaskService : ITaskService
    {
        private readonly NotificationService _notificationService;
        private readonly ILogger<TaskService> _logger;
       private string _clientName { get; set; }

        public TaskService(
            NotificationService notificationService,
            ILogger<TaskService> logger
           )
        {
            _notificationService = notificationService;
            _logger = logger;
           
        }
        //public async Task<TaskModel?> GetTaskByIdAsync(Guid taskId)
        //{
        //    var tasks = await _storage.GetTasksAsync();
        //    return tasks.FirstOrDefault(t => t.Id == taskId.ToString());
        //}

        //public async Task<TaskResponse> AssignTaskAsync(AssignTaskRequest request)
        //{
        //    try
        //    {
        //        using (var context = new AppDbContext())
        //        {
        //            //var assignedBy = await _storage.GetEmployeeByIdAsync(request.AssignedBy);
        //            //var assignedTo = await _storage.GetEmployeeByIdAsync(request.AssignedTo);
        //            var assignedBy = await context.AppUsers.Where(c => c.EmployeeCode == request.AssignedBy).FirstOrDefaultAsync();
        //            var assignedTo = await context.Employees.Where(c => c.Code == request.AssignedTo).FirstOrDefaultAsync();

        //            if (assignedBy == null || assignedTo == null)
        //            {
        //                throw new Exception("Invalid employee IDs");
        //            }


        //            var task2 = new UserTask()
        //            {
        //                Id = Guid.NewGuid(),
        //                Title = request.TaskTitle,
        //                Description = request.TaskDescription,
        //                AssignedBy = request.AssignedBy,
        //                AssignedByName = assignedBy.Name,
        //                AssignedTo = request.AssignedTo,
        //                AssignedToName = assignedTo.Name,
        //                AssignedDate = DateTime.Now,
        //                DueDate = request.DueDate,
        //                TaskPreferance = request.TaskPreference,
        //                IsActive = true,
        //                IsDelete = false,
        //                Priority = request.Priority,
        //                Status = "assigned",
        //                EntityCode = request.Client
        //            };

        //            context.Tasks.Add(task2);

        //            //await _storage.AddTaskAsync(task);

        //            await context.SaveChangesAsync();

        //            // Send notification to employee
        //            await _notificationService.SendTaskNotificationAsync(
        //                assignedBy.Code,
        //                "New Task Assigned",
        //                $"{assignedBy.Name} assigned you a task: {request.TaskTitle}",
        //                context,
        //                new Dictionary<string, object>
        //                {
        //                { "taskId", task2.Id },
        //                { "priority", task2.Priority }
        //                }
        //            );

        //            await _notificationService.SendFcmNotificationAsync(assignedTo.Code, assignedTo.FcmToken,
        //                "New Task Assigned", $"{assignedBy.Name} assigned you a task: {request.TaskTitle}");
        //            var task = new TaskResponse
        //            {
        //                TaskTitle = request.TaskTitle,
        //                TaskDescription = request.TaskDescription,
        //                AssignedBy = request.AssignedBy,
        //                AssignedByName = assignedBy.Name,
        //                AssignedTo = request.AssignedTo,
        //                AssignedToName = assignedTo.Name,
        //                DueDate = request.DueDate,
        //                Priority = request.Priority,
        //                Status = "assigned"
        //            };

        //            _logger.LogInformation($"Task assigned: {task2.Title} → {assignedTo.Name}");

        //            return task;
        //        }
        //    } catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        public async Task<List<TaskResponse>> AssignTaskToMultipleAsync(AssignTaskMultipleRequest request)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var assignedBy = await context.AppUsers
                        .Where(c => c.EmployeeCode == request.AssignedBy)
                        .FirstOrDefaultAsync();

                    if (assignedBy == null)
                        throw new Exception("Invalid assigner ID");
                    var teamTaskId = Guid.NewGuid();
                    var tasks = new List<UserTask>();
                    var responses = new List<TaskResponse>();

                    foreach (var assigneeCode in request.AssignedTo)
                    {
                        var assignedTo = await context.Employees
                            .Where(c => c.Code == assigneeCode)
                            .FirstOrDefaultAsync();

                        if (assignedTo == null)
                        {
                            _logger.LogWarning($"Employee {assigneeCode} not found, skipping");
                            continue;
                        }

                        var task = new UserTask()
                        {
                            Id = Guid.NewGuid(),
                            Title = request.TaskTitle,
                            Description = request.TaskDescription,
                            AssignedBy = request.AssignedBy,
                            AssignedByName = assignedBy.Name,
                            AssignedTo = assigneeCode,
                            AssignedToName = assignedTo.Name,
                            AssignedDate = DateTime.Now,
                            DueDate = Convert.ToDateTime(request.DueDate),
                            TaskPreferance = request.TaskPreference,
                            IsActive = true,
                            IsDelete = false,
                            Priority = request.Priority,
                            Status = "assigned",
                            EntityCode = request.Client
                        };

                        context.Tasks.Add(task);
                        var teamTask = new TeamTask
                        {
                            TeamTaskId = teamTaskId,
                            TaskId = task.Id
                        };

                        context.TeamTasks.Add(teamTask);
                        tasks.Add(task);

                        // Send notifications
                        //await _notificationService.SendTaskNotificationAsync(
                        //    assignedBy.Code,
                        //    "New Task Assigned",
                        //    $"{assignedBy.Name} assigned you a task: {request.TaskTitle}",
                        //    context,
                        //    new Dictionary<string, object>
                        //    {
                        //{ "taskId", task.Id },
                        //{ "priority", task.Priority }
                        //    }
                        //);

                        //await _notificationService.SendFcmNotificationAsync(
                        //    assignedTo.Code,
                        //    assignedTo.FcmToken,
                        //    "New Task Assigned",
                        //    $"{assignedBy.Name} assigned you a task: {request.TaskTitle}"
                        //);

                        responses.Add(new TaskResponse
                        {
                            TaskTitle = request.TaskTitle,
                            TaskDescription = request.TaskDescription,
                            AssignedBy = request.AssignedBy,
                            AssignedByName = assignedBy.Name,
                            AssignedTo = assigneeCode,
                            AssignedToName = assignedTo.Name,
                            DueDate = Convert.ToDateTime(request.DueDate),
                            Priority = request.Priority,
                            Status = "assigned"
                        });

                        _logger.LogInformation($"Task assigned: {task.Title} → {assignedTo.Name}");
                    }

                    await context.SaveChangesAsync();
                    return responses;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in bulk task assignment");
                throw;
            }
        }

        // Keep the original method for backward compatibility
        public async Task<TaskResponse> AssignTaskAsync(AssignTaskRequest request)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var assignedBy = await context.AppUsers
                        .Where(c => c.EmployeeCode == request.AssignedBy)
                        .FirstOrDefaultAsync();

                    var assignedTo = await context.Employees
                        .Where(c => c.Code == request.AssignedTo)
                        .FirstOrDefaultAsync();

                    if (assignedBy == null || assignedTo == null)
                        throw new Exception("Invalid employee IDs");

                    var task = new UserTask()
                    {
                        Id = Guid.NewGuid(),
                        Title = request.TaskTitle,
                        Description = request.TaskDescription,
                        AssignedBy = request.AssignedBy,
                        AssignedByName = assignedBy.Name,
                        AssignedTo = request.AssignedTo,
                        AssignedToName = assignedTo.Name,
                        AssignedDate = DateTime.Now,
                        DueDate = request.DueDate,
                        TaskPreferance = request.TaskPreference,
                        IsActive = true,
                        IsDelete = false,
                        Priority = request.Priority,
                        Status = "assigned",
                        EntityCode = request.Client,
                        //AssignedToMultiple = $"[{request.AssignedTo}]" // Single user in JSON array
                    };

                    context.Tasks.Add(task);
                    await context.SaveChangesAsync();

                    // Send notifications...
                    await _notificationService.SendTaskNotificationAsync(
                        assignedBy.Code,
                        "New Task Assigned",
                        $"{assignedBy.Name} assigned you a task: {request.TaskTitle}",
                        context,
                        new Dictionary<string, object>
                        {
                    { "taskId", task.Id },
                    { "priority", task.Priority }
                        }
                    );

                    await _notificationService.SendFcmNotificationAsync(
                        assignedTo.Code,
                        assignedTo.FcmToken,
                        "New Task Assigned",
                        $"{assignedBy.Name} assigned you a task: {request.TaskTitle}"
                    );

                    var response = new TaskResponse
                    {
                        TaskTitle = request.TaskTitle,
                        TaskDescription = request.TaskDescription,
                        AssignedBy = request.AssignedBy,
                        AssignedByName = assignedBy.Name,
                        AssignedTo = request.AssignedTo,
                        AssignedToName = assignedTo.Name,
                        DueDate = request.DueDate,
                        Priority = request.Priority,
                        Status = "assigned"
                    };

                    _logger.LogInformation($"Task assigned: {task.Title} → {assignedTo.Name}");
                    return response;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in task assignment");
                throw;
            }
        }
        //public (DateTime startDate, DateTime endDate) CalculateDateRange(TaskExportRequest request)
        //{
        //    switch (request.ViewType.ToLower())
        //    {
        //        case "daily":
        //            DateTime inputDate = request.ReferenceDate ?? DateTime.Today;

        //            int diff = (7 + (inputDate.DayOfWeek - DayOfWeek.Monday)) % 7;

        //            DateTime weekStart = inputDate.AddDays(-diff);
        //            DateTime weekEnd = weekStart.AddDays(7).AddSeconds(-1);

        //            return (weekStart, weekEnd);

        //        case "monthly":
        //            int year = request.Year ?? DateTime.Now.Year;
        //            int month = request.Month ?? DateTime.Now.Month;

        //            DateTime startDate = new DateTime(year, month, 1);
        //            int daysInMonth = DateTime.DaysInMonth(year, month);
        //            DateTime endDate = new DateTime(year, month, daysInMonth, 23, 59, 59);

        //            return (startDate, endDate);

        //        case "yearly":
        //            int reportYear = request.Year ?? DateTime.Now.Year;

        //            DateTime yearStart = new DateTime(reportYear, 1, 1);
        //            DateTime yearEnd = yearStart.AddYears(1).AddSeconds(-1);

        //            return (yearStart, yearEnd);

        //        default:
        //            throw new ArgumentException($"Invalid view type: {request.ViewType}");
        //    }
        //}
        public string GenerateFileName(TaskExportRequest request, string? employeeName)
        {
            if (!string.IsNullOrEmpty(employeeName))
            {
                string safeName = string.Join("_", employeeName.Split(Path.GetInvalidFileNameChars()));
                return $"Task_Report_{safeName}.xlsx";
            }

            return $"Task_Report_AllEmployees.xlsx";
        }
        public async Task<List<EmployeeTaskReport>> GenerateTaskReportAsync(string reportType, DateTime startDate, DateTime endDate, string? managerId)
        {
            using (var context = new AppDbContext())
            {
                try
                {
                    var allEmployees = await context.Employees.Where(e => e.LeadCode == managerId)
                        .Select(e => new EmployeeTaskReport
                        {
                            EmployeeCode = e.Code,
                            EmployeeName = e.Name,
                            Tasks = new List<TaskReport>()
                        })
                        .ToListAsync();

                    var employeeTasks = await (from a in context.Tasks
                                               join b in context.TaskTimeLogs
                                                   on a.Id equals b.TaskId
                                               join e in context.Employees
                                                   on a.AssignedTo equals e.Code
                                               where b.LogDate >= startDate
                                                   && b.LogDate <= endDate
                                                   && e.LeadCode == managerId && a.Status == "completed"
                                               group new { a, b } by new
                                               {
                                                   EmployeeCode = a.AssignedTo,
                                                   EmployeeName = e.Name
                                               } into empGroup
                                               select new
                                               {
                                                   EmployeeCode = empGroup.Key.EmployeeCode,
                                                   EmployeeName = empGroup.Key.EmployeeName,
                                                   Tasks = empGroup
                                                       .GroupBy(x => new
                                                       {
                                                           x.a.Id,
                                                           x.a.Title,
                                                           x.a.Status,
                                                           x.a.Priority,
                                                           x.a.DueDate,
                                                           x.a.CompletedAt
                                                       })
                                                       .Select(taskGroup => new TaskReport
                                                       {
                                                           Taskid = taskGroup.Key.Id.ToString(),
                                                           TaskTitle = taskGroup.Key.Title,
                                                           TaskStatus = taskGroup.Key.Status,
                                                           TaskPriority = taskGroup.Key.Priority,
                                                           TaskDueDate = taskGroup.Key.DueDate,
                                                           CompletedAt = taskGroup.Key.CompletedAt,
                                                           TimeLogs = taskGroup.Select(t => new TaskTimeLogResponse
                                                           {
                                                               Notes = t.b.Notes,
                                                               StartTime = t.b.StartTime,
                                                               StopTime = t.b.StopTime,
                                                               HoursWorked = t.b.HoursWorked
                                                           }).ToList()
                                                       }).ToList()
                                               }).ToListAsync();

                    foreach (var employee in allEmployees)
                    {
                        var employeeWithTasks = employeeTasks.FirstOrDefault(et => et.EmployeeCode == employee.EmployeeCode);

                        if (employeeWithTasks != null)
                        {
                            employee.Tasks = employeeWithTasks.Tasks;
                        }

                    }

                    return allEmployees;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public async Task<List<TaskExportDataModel>> GetTaskReportDataForExport(DateTime startDate, DateTime endDate, string? managerId, string? employeeId = null)
        {
            using (var context = new AppDbContext())
            {
                var query = from t in context.Tasks
                            join tl in context.TaskTimeLogs on t.Id equals tl.TaskId
                            join e in context.Employees on t.AssignedTo equals e.Code
                            where tl.LogDate >= startDate.Date && tl.LogDate <= endDate.Date && t.Status == "completed"
                            select new { t, tl, e };

                if (!string.IsNullOrEmpty(managerId))
                {
                    query = query.Where(x => x.e.LeadCode == managerId);
                }

                if (!string.IsNullOrEmpty(employeeId))
                {
                    query = query.Where(x => x.e.Code == employeeId);
                }

                var results = await query
                    .Select(x => new TaskExportDataModel
                    {
                        EmployeeCode = x.e.Code,
                        EmployeeName = x.e.Name,
                        TaskId = x.t.Id.ToString(),
                        TaskTitle = x.t.Title,
                        LogDate = x.tl.LogDate,
                        StartTime = x.tl.StartTime,
                        StopTime = x.tl.StopTime,
                        HoursWorked = x.tl.HoursWorked,
                        Notes = x.tl.Notes ?? string.Empty,
                        DayOfWeek = x.tl.LogDate.DayOfWeek.ToString(),
                        WeekNumber = System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                            x.tl.LogDate,
                            System.Globalization.CalendarWeekRule.FirstDay,
                            DayOfWeek.Monday),
                        Month = x.tl.LogDate.ToString("MMMM"),
                        Year = x.tl.LogDate.Year
                    })
                    .OrderBy(x => x.EmployeeName)
                    .ThenBy(x => x.LogDate)
                    .ToListAsync();

                return results;
            }
        }
        public async Task<List<TaskResponse>> GetEmployeeTasksAsync(string userCode)
        {
            //var tasks = await GetTasksAsync();
            //return tasks
            //    .Where(t => t.AssignedTo == employeeId)
            //    .OrderByDescending(t => t.CreatedAt)
            //    .ToList();
            using (var context = new AppDbContext())
            {
                var taskLogs = await context.TaskTimeLogs.ToListAsync();

                var tasks = await context.Tasks.Where(c => c.AssignedTo == userCode && c.IsDelete == false).OrderByDescending(c => c.AssignedDate).ToListAsync();
                //var response = new List<TaskResponse>();

                var response = tasks.Select(c => new TaskResponse
                {
                    Id = c.Id.ToString(),
                    TaskTitle = c.Title,
                    TaskDescription = c.Description,
                    AssignedBy = c.AssignedBy,
                    AssignedByName = c.AssignedByName,
                    AssignedTo = c.AssignedTo,
                    AssignedToName = c.AssignedToName,
                    AssignedDate = c.AssignedDate,
                    DueDate = c.DueDate,
                    Status = c.Status,
                    Priority = c.Priority,
                    TotalHoursWorked = c.TotalHoursWorked,
                    TimeLogs = context.TaskTimeLogs
    .Where(log => log.TaskId == c.Id)
    .ToList()
    .Adapt<List<TaskTimeLogResponse>>()

                }).ToList();

                return response;
                //foreach (var c in tasks)
                //{
                //response.Add(new TaskResponse()
                //{
                //    Id = c.Id.ToString(),
                //    TaskTitle = c.Title,
                //    TaskDescription = c.Description,
                //    AssignedBy = c.AssignedBy,
                //    AssignedByName = c.AssignedByName,
                //    AssignedTo = c.AssignedTo,
                //    AssignedToName = c.AssignedToName,
                //    AssignedDate = c.AssignedDate,
                //    DueDate = c.DueDate,
                //    Status = c.Status,
                //    Priority = c.Priority,
                //    TotalHoursWorked = c.TotalHoursWorked,
                //    TimeLogs = mapper.Map<List<TaskTimeLogResponse>>(taskLogs.Where(c => c.TaskId == c.Id))
                //});
                //}
            }
        }
        public async Task<List<PendingTask>> GetPendingTasks(string userId)
        {
            var today = DateTime.UtcNow.Date;
            using (var context = new AppDbContext())
            {

                var task = await context.Tasks
                .Where(t => t.AssignedTo == userId
                            && t.Status != "completed")
                .Select(t => new PendingTask
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    DueDate = t.DueDate,
                    Priority = t.Priority,
                    AssignedDate = t.AssignedDate
                })
                .ToListAsync();


                return task;
            }
        }
        public async Task<List<TaskResponse>> GetActiveEmployeeTasksAsync(string userCode)
        {
            using (var context = new AppDbContext())
            {
                //var taskLogs = await context.TaskTimeLogs.ToListAsync();

                //var emp = await context.AppUsers.FirstOrDefaultAsync(a => a.Code == userCode);
                //var s = emp.EmployeeCode;

                //var tasks = await context.Tasks.Where(t => t.AssignedTo == userCode && !t.IsDelete)
                //                .OrderBy(t => t.DueDate).ToListAsync();

                //var clientName = await context.Entities.Where(a => a.Code == tasks)

                var tasks = await (from t in context.Tasks
                                   join e in context.Entities on t.EntityCode equals e.Code
                                   where t.AssignedTo == userCode && !t.IsDelete
                                   orderby t.DueDate
                                   select new TaskResponse
                                   {
                                       Id = t.Id.ToString(),
                                       TaskTitle = t.Title,
                                       ClientName = e.Name,
                                       ClientCode = e.Code,
                                       TaskDescription = t.Description,
                                       AssignedBy = t.AssignedBy,
                                       AssignedByName = t.AssignedByName,
                                       AssignedTo = t.AssignedTo,
                                       AssignedToName = t.AssignedToName,
                                       AssignedDate = t.AssignedDate,
                                       TaskPreference = t.TaskPreferance,
                                       DueDate = t.DueDate,
                                       Status = t.Status,
                                       Priority = t.Priority,
                                       TotalHoursWorked = t.TotalHoursWorked,
                                       TimeLogs = context.TaskTimeLogs
                                        .Where(log => log.TaskId == t.Id)
                                        .OrderBy(log => log.StartTime)
                                        .ToList()
                                        .Adapt<List<TaskTimeLogResponse>>(),

                                       CurrentDayStartTime = context.TaskTimeLogs
                                                                .Where(log =>
                                                                    log.TaskId == t.Id &&
                                                                    log.StartTime.HasValue &&
                                                                    log.StartTime.Value.Date == DateTime.Today &&
                                                                    log.StopTime.HasValue &&
                                                                    log.StopTime.Value.Date == DateTime.Today)
                                                                .Sum(log =>
                                                                    EF.Functions.DateDiffMinute(
                                                                        log.StartTime.Value,
                                                                        log.StopTime.Value
                                                                    )
                                                                ),
                                   }).ToListAsync();
                return tasks;
            }
        }
        public async Task<List<TaskResponse>> GetTeamLeadTasksAsync(string teamLeadCode)
        {
            using (var context = new AppDbContext())
            {
                var taskLogs = await context.TaskTimeLogs.ToListAsync();

                var tasks = await context.Tasks.Where(t => t.AssignedBy == teamLeadCode && !t.IsDelete)
                    .OrderByDescending(t => t.AssignedDate).ToListAsync();

                var response = await (from t in context.Tasks
                                      join e in context.Entities on t.EntityCode equals e.Code
                                      where t.AssignedBy == teamLeadCode && !t.IsDelete
                                      orderby t.DueDate
                                      select new TaskResponse
                                      {
                                          Id = t.Id.ToString(),
                                          TaskTitle = t.Title,
                                          ClientCode = e.Code,
                                          ClientName = e.Name,
                                          TaskDescription = t.Description,
                                          AssignedBy = t.AssignedBy,
                                          AssignedByName = t.AssignedByName,
                                          AssignedTo = t.AssignedTo,
                                          AssignedToName = t.AssignedToName,
                                          AssignedDate = t.AssignedDate,
                                          TaskPreference = t.TaskPreferance,
                                          DueDate = t.DueDate,
                                          Status = t.Status,
                                          Priority = t.Priority,
                                          TotalHoursWorked = t.TotalHoursWorked,

                                          TimeLogs = context.TaskTimeLogs
    .Where(log => log.TaskId == t.Id)
    .ToList()
    .Adapt<List<TaskTimeLogResponse>>()
                                      }).ToListAsync();

                return response;
            }
        }
        //public async Task<List<TaskResponse>> GetTeamLeadTasksAsync(string teamLeadCode)
        //{
        //    using (var context = new AppDbContext())
        //    {
        //        // 1. Get ALL tasks assigned by Team Lead
        //        // This includes both single and multi-assigned tasks.
        //        var tasks = await context.Tasks
        //            .Where(t =>
        //                t.AssignedBy == teamLeadCode &&
        //                !t.IsDelete)
        //            .OrderByDescending(t => t.AssignedDate)
        //            .ToListAsync();

        //        if (!tasks.Any())
        //            return new List<TaskResponse>();


        //        // 2. Get entities
        //        var entityCodes = tasks
        //            .Where(t => !string.IsNullOrEmpty(t.EntityCode))
        //            .Select(t => t.EntityCode)
        //            .Distinct()
        //            .ToList();

        //        var entities = await context.Entities
        //            .Where(e => entityCodes.Contains(e.Code))
        //            .ToListAsync();


        //        // 3. Get TeamTask mappings
        //        // For single tasks there may be NO record here.
        //        var taskIds = tasks
        //            .Select(t => t.Id)
        //            .ToList();

        //        var teamTasks = await context.TeamTasks
        //            .Where(tt => taskIds.Contains(tt.TaskId))
        //            .ToListAsync();


        //        // 4. Get all time logs
        //        var taskLogs = await context.TaskTimeLogs
        //            .Where(log => taskIds.Contains(log.TaskId))
        //            .ToListAsync();


        //        // 5. Build groups
        //        //
        //        // Multi task:
        //        // TeamTaskId AAA
        //        //     -> Saad
        //        //     -> Kamran
        //        //
        //        // Single task:
        //        // No TeamTask record
        //        //     -> its own TaskId is used as group ID

        //        var groupedTasks = tasks
        //            .GroupJoin(
        //                teamTasks,
        //                task => task.Id,
        //                teamTask => teamTask.TaskId,
        //                (task, mappings) => new
        //                {
        //                    Task = task,
        //                    TeamTasks = mappings.ToList()
        //                })
        //            .SelectMany(x =>
        //            {
        //                // MULTI ASSIGNMENT
        //                if (x.TeamTasks.Any())
        //                {
        //                    return x.TeamTasks.Select(tt => new
        //                    {
        //                        Task = x.Task,
        //                        TeamTaskId = tt.TeamTaskId
        //                    });
        //                }

        //                // SINGLE ASSIGNMENT
        //                // No TeamTask row, so use Task.Id
        //                return new[]
        //                {
        //            new
        //            {
        //                Task = x.Task,
        //                TeamTaskId = x.Task.Id
        //            }
        //                };
        //            })
        //            .GroupBy(x => x.TeamTaskId)
        //            .ToList();


        //        // 6. Build response
        //        var response = new List<TaskResponse>();

        //        foreach (var group in groupedTasks)
        //        {
        //            var first = group.First();
        //            var firstTask = first.Task;

        //            var entity = entities
        //                .FirstOrDefault(e => e.Code == firstTask.EntityCode);


        //            var taskResponse = new TaskResponse
        //            {
        //                // For multi:
        //                // TeamTaskId
        //                //
        //                // For single:
        //                // Task.Id
        //                Id = group.Key.ToString(),

        //                TaskTitle = firstTask.Title,

        //                TaskDescription = firstTask.Description,

        //                ClientCode = entity?.Code,

        //                ClientName = entity?.Name,

        //                AssignedBy = firstTask.AssignedBy,

        //                AssignedByName = firstTask.AssignedByName,

        //                AssignedDate = firstTask.AssignedDate,

        //                DueDate = firstTask.DueDate,

        //                TaskPreference = firstTask.TaskPreferance,

        //                Priority = firstTask.Priority,

        //                // All employees
        //                Assignees = group
        //                    .Select(x => new TaskAssigneeResponse
        //                    {
        //                        TaskId = x.Task.Id,

        //                        AssignedTo = x.Task.AssignedTo,

        //                        AssignedToName = x.Task.AssignedToName,

        //                        Status = x.Task.Status,

        //                        TotalHoursWorked =
        //                            x.Task.TotalHoursWorked
        //                    })
        //                    .GroupBy(x => x.TaskId)
        //                    .Select(x => x.First())
        //                    .ToList(),

        //                // Time logs
        //                TimeLogs = taskLogs
        //                    .Where(log =>
        //                        group
        //                            .Select(x => x.Task.Id)
        //                            .Contains(log.TaskId))
        //                    .Select(log =>
        //                        log.Adapt<TaskTimeLogResponse>())
        //                    .ToList()
        //            };

        //            response.Add(taskResponse);
        //        }


        //        return response
        //            .OrderByDescending(x => x.AssignedDate)
        //            .ToList();
        //    }
        //}




        public async Task<List<EmployeeTaskReport>> GenerateTaskYearlyReportAsync(int year, string? managerId)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = startDate.AddYears(1).AddSeconds(-1);

            using (var context = new AppDbContext())
            {
                return await GenerateTaskReportAsync("yearly", startDate, endDate, managerId);
            }
        }

        public async Task<List<EmployeeTaskReport>> GenerateTaskReport(DateTime startDate, DateTime endDate, string? managerId)
        {
            //var startDate = new DateTime(year, 1, 1);
            //var endDate = startDate.AddYears(1).AddSeconds(-1);

            using (var context = new AppDbContext())
            {
                return await GenerateTaskReportAsync("yearly", startDate, endDate, managerId);
            }
        }
        public async Task<List<EmployeeTaskReport>> GenerateTaskMonthlyReportAsync(int year, int month, string? managerId)
        {
            var startDate = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            DateTime lastDay = new DateTime(year, month, daysInMonth);
            var endDate = lastDay;

            using (var context = new AppDbContext())
            {
                return await GenerateTaskReportAsync("monthly", startDate, endDate, managerId);
            }
        }
        private string[] GetHeaders(bool isSingleEmployee, string viewType)
        {
            if (isSingleEmployee)
            {
                if (viewType.ToLower() == "year")
                {
                    return new[] {
                        "Task Title",
                        "Date",
                        "Day",
                        "Start Time",
                        "End Time",
                        "Notes",
                        "Hours",
                        "Month"
                    };
                }
                else
                {
                    return new[] {
                        "Task Title",
                        "Date",
                        "Day",
                        "Start Time",
                        "End Time",
                        "Notes",
                        "Hours"
                    };
                }
            }
            else
            {
                if (viewType.ToLower() == "year")
                {
                    return new[] {
                        "Employee Code",
                        "Employee Name",
                        "Task Title",
                        "Date",
                        "Day",
                        "Start Time",
                        "End Time",
                        "Hours",
                        "Notes",
                        "Month"
                    };
                }
                else
                {
                    return new[] {
                        "Employee Code",
                        "Employee Name",
                        "Task Title",
                        "Date",
                        "Day",
                        "Start Time",
                        "End Time",
                        "Hours",
                        "Notes"
                    };
                }
            }
        }
        private string GetLastColumnName(int columnCount)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return letters[columnCount - 1].ToString();
        }
        public byte[] GenerateExcelReport(List<TaskExportDataModel> data, string viewType, DateTime startDate, DateTime endDate, string? employeeName = null)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Task Report");

                string title = employeeName != null
                    ? $"Task Report - {employeeName}"
                    : "Task Report - All Employees";

                // Get headers based on view type
                string[] headers = GetHeaders(employeeName != null, viewType);
                int totalColumns = headers.Length;
                string lastColumn = GetLastColumnName(totalColumns);

                // Title
                worksheet.Cell("A1").Value = title;
                worksheet.Range($"A1:{lastColumn}1").Merge();
                worksheet.Cell("A1").Style
                    .Font.SetBold(true)
                    .Font.SetFontSize(16)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                // View type
                //worksheet.Cell("A2").Value = $"View: {viewType}";
                //worksheet.Range($"A2:{lastColumn}2").Merge();
                //worksheet.Cell("A2").Style
                //    .Font.SetItalic(true)
                //    .Font.SetFontColor(XLColor.FromArgb(100, 100, 100));

                // Period
                worksheet.Cell("A3").Value = $"Period: {startDate:dd MMM yyyy} - {endDate:dd MMM yyyy}";
                worksheet.Range($"A3:{lastColumn}3").Merge();
                worksheet.Cell("A3").Style
                    .Font.SetItalic(true)
                    .Font.SetFontColor(XLColor.FromArgb(100, 100, 100));

                // Generation time
                worksheet.Cell("A4").Value = $"Generated: {DateTime.Now:dd MMM yyyy HH:mm}";
                worksheet.Range($"A4:{lastColumn}4").Merge();
                worksheet.Cell("A4").Style
                    .Font.SetItalic(true)
                    .Font.SetFontColor(XLColor.FromArgb(100, 100, 100));

                worksheet.Row(5).Height = 10;

                int headerRow = 6;

                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = worksheet.Cell(headerRow, i + 1);
                    cell.Value = headers[i];
                    cell.Style
                        .Font.SetBold(true)
                        .Fill.SetBackgroundColor(XLColor.FromArgb(79, 129, 189))
                        .Font.SetFontColor(XLColor.White)
                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                        .Alignment.SetWrapText(true);
                }

                int currentRow = headerRow + 1;

                if (employeeName != null)
                {
                    var tasksByEmployee = data
                        .GroupBy(x => x.TaskId)
                        .OrderBy(x => x.First().TaskTitle);

                    foreach (var taskGroup in tasksByEmployee)
                    {
                        var taskLogs = taskGroup.OrderBy(x => x.LogDate).ThenBy(x => x.StartTime).ToList();
                        int taskRowCount = taskLogs.Count;

                        worksheet.Range(currentRow, 1, currentRow + taskRowCount - 1, 1).Merge();
                        var taskCell = worksheet.Cell(currentRow, 1);
                        taskCell.Value = taskGroup.First().TaskTitle;
                        taskCell.Style
                            .Font.SetBold(true)
                            .Fill.SetBackgroundColor(XLColor.FromArgb(230, 242, 255))
                            .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                            .Alignment.SetWrapText(true);

                        // Add all logs for this task
                        foreach (var log in taskLogs)
                        {
                            int col = 2;

                            worksheet.Cell(currentRow, col++).Value = log.LogDate.ToString("dd-MMM-yyyy");

                            worksheet.Cell(currentRow, col++).Value = log.DayOfWeek;

                            worksheet.Cell(currentRow, col++).Value = log.StartTime?.ToString("HH:mm") ?? "";

                            worksheet.Cell(currentRow, col++).Value = log.StopTime?.ToString("HH:mm") ?? "";

                            var notesCell = worksheet.Cell(currentRow, col);
                            notesCell.Value = log.Notes ?? "";
                            notesCell.Style.Alignment.SetWrapText(true);
                            col++;

                            // Hours Worked
                            var hoursCell = worksheet.Cell(currentRow, col);
                            hoursCell.Value = log.HoursWorked;
                            hoursCell.Style.NumberFormat.Format = "0.00";

                            // Add Month column for Yearly view
                            if (viewType.ToLower() == "year")
                            {
                                col++;
                                worksheet.Cell(currentRow, col).Value = log.Month;
                            }

                            currentRow++;
                        }

                        // Add empty row between tasks for better readability
                        if (taskGroup != tasksByEmployee.Last())
                        {
                            worksheet.Row(currentRow).Height = 5;
                            currentRow++;
                        }
                    }

                    // Add summary section for selected employee
                    int summaryRow = currentRow + 2;

                    worksheet.Range(summaryRow, 1, summaryRow, 3).Merge();
                    worksheet.Cell(summaryRow, 1).Value = "SUMMARY";
                    worksheet.Cell(summaryRow, 1).Style.Font.SetBold(true);

                    worksheet.Cell(summaryRow + 1, 1).Value = "Total Hours:";
                    var totalHoursCell = worksheet.Cell(summaryRow + 1, 2);
                    totalHoursCell.Value = data.Sum(x => x.HoursWorked ?? 0);
                    totalHoursCell.Style.NumberFormat.Format = "0.00";
                    totalHoursCell.Style.Font.SetBold(true);

                    worksheet.Cell(summaryRow + 2, 1).Value = "Total Tasks:";
                    worksheet.Cell(summaryRow + 2, 2).Value = data.Select(x => x.TaskId).Distinct().Count();

                    worksheet.Cell(summaryRow + 3, 1).Value = "Active Days:";
                    worksheet.Cell(summaryRow + 3, 2).Value = data.Select(x => x.LogDate.Date).Distinct().Count();
                }
                else
                {
                    // ALL EMPLOYEES - Group by employee, then by task
                    var employees = data
                        .GroupBy(x => x.EmployeeCode)
                        .OrderBy(x => x.First().EmployeeName);

                    foreach (var employeeGroup in employees)
                    {
                        var employeeName_ = employeeGroup.First().EmployeeName;
                        var employeeCode = employeeGroup.Key;
                        var tasksByEmployee = employeeGroup
                            .GroupBy(x => x.TaskId)
                            .OrderBy(x => x.First().TaskTitle);

                        int totalEmployeeRows = tasksByEmployee.Sum(taskGroup => taskGroup.Count());

                        worksheet.Range(currentRow, 1, currentRow + totalEmployeeRows - 1, 1).Merge();
                        var empCodeCell = worksheet.Cell(currentRow, 1);
                        empCodeCell.Value = employeeCode;
                        empCodeCell.Style
                            .Font.SetBold(true)
                            .Fill.SetBackgroundColor(XLColor.FromArgb(198, 224, 255))
                            .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                            .Alignment.SetWrapText(true);

                        worksheet.Range(currentRow, 2, currentRow + totalEmployeeRows - 1, 2).Merge();
                        var empNameCell = worksheet.Cell(currentRow, 2);
                        empNameCell.Value = employeeName_;
                        empNameCell.Style
                            .Font.SetBold(true)
                            .Fill.SetBackgroundColor(XLColor.FromArgb(198, 224, 255))
                            .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                            .Alignment.SetWrapText(true);

                        foreach (var taskGroup in tasksByEmployee)
                        {
                            var taskLogs = taskGroup.OrderBy(x => x.LogDate).ToList();
                            int taskRowCount = taskLogs.Count;

                            // Task Title (merged across all logs of this task)
                            worksheet.Range(currentRow, 3, currentRow + taskRowCount - 1, 3).Merge();
                            var taskCell = worksheet.Cell(currentRow, 3);
                            taskCell.Value = taskGroup.First().TaskTitle;
                            taskCell.Style
                                .Font.SetBold(true)
                                .Fill.SetBackgroundColor(XLColor.FromArgb(230, 242, 255))
                                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                .Alignment.SetWrapText(true);

                            // Add all logs for this task
                            foreach (var log in taskLogs)
                            {
                                int col = 4;

                                worksheet.Cell(currentRow, col++).Value = log.LogDate.ToString("dd-MMM-yyyy");
                                worksheet.Cell(currentRow, col++).Value = log.DayOfWeek;
                                worksheet.Cell(currentRow, col++).Value = log.StartTime?.ToString("HH:mm") ?? "";
                                worksheet.Cell(currentRow, col++).Value = log.StopTime?.ToString("HH:mm") ?? "";

                                var hoursCell = worksheet.Cell(currentRow, col);
                                hoursCell.Value = log.HoursWorked;
                                hoursCell.Style.NumberFormat.Format = "0.00";
                                col++;

                                // Notes with text wrapping
                                var notesCell = worksheet.Cell(currentRow, col);
                                notesCell.Value = log.Notes ?? "";
                                notesCell.Style.Alignment.SetWrapText(true);
                                col++;

                                // Add view-specific columns
                                if (viewType.ToLower() == "month" || viewType.ToLower() == "day")
                                {
                                    // For monthly, weekly, daily views - no week, month, or year columns
                                    // Just skip adding them
                                }
                                else if (viewType.ToLower() == "year")
                                {
                                    // For yearly view - add month only (no week or year)
                                    worksheet.Cell(currentRow, col++).Value = log.Month;
                                }

                                currentRow++;
                            }
                        }

                        // Add empty row between employees for better readability
                        if (employeeGroup != employees.Last())
                        {
                            worksheet.Row(currentRow).Height = 10;
                            currentRow++;
                        }
                    }

                    // Add summary section for all employees
                    int summaryRow = currentRow + 2;

                    worksheet.Range(summaryRow, 1, summaryRow, 3).Merge();
                    worksheet.Cell(summaryRow, 1).Value = "SUMMARY - ALL EMPLOYEES";
                    worksheet.Cell(summaryRow, 1).Style.Font.SetBold(true);

                    worksheet.Cell(summaryRow + 1, 1).Value = "Total Hours (All):";
                    var totalHoursCell = worksheet.Cell(summaryRow + 1, 2);
                    totalHoursCell.Value = data.Sum(x => x.HoursWorked ?? 0);
                    totalHoursCell.Style.NumberFormat.Format = "0.00";
                    totalHoursCell.Style.Font.SetBold(true);

                    worksheet.Cell(summaryRow + 2, 1).Value = "Total Employees:";
                    worksheet.Cell(summaryRow + 2, 2).Value = data.Select(x => x.EmployeeCode).Distinct().Count();

                    // Employee breakdown
                    int breakdownRow = summaryRow + 4;
                    worksheet.Range(breakdownRow, 1, breakdownRow, 2).Merge();
                    worksheet.Cell(breakdownRow, 1).Value = "EMPLOYEE BREAKDOWN";
                    worksheet.Cell(breakdownRow, 1).Style.Font.SetBold(true);

                    breakdownRow++;
                    worksheet.Cell(breakdownRow, 1).Value = "Employee Code";
                    worksheet.Cell(breakdownRow, 2).Value = "Employee Name";
                    worksheet.Cell(breakdownRow, 3).Value = "Total Hours";
                    worksheet.Range(breakdownRow, 1, breakdownRow, 3).Style
                        .Font.SetBold(true)
                        .Fill.SetBackgroundColor(XLColor.FromArgb(220, 220, 220));

                    breakdownRow++;
                    var employeeTotals = data
                        .GroupBy(x => new { x.EmployeeCode, x.EmployeeName })
                        .Select(g => new {
                            EmployeeCode = g.Key.EmployeeCode,
                            EmployeeName = g.Key.EmployeeName,
                            TotalHours = g.Sum(x => x.HoursWorked ?? 0)
                        })
                        .OrderByDescending(x => x.TotalHours);

                    foreach (var emp in employeeTotals)
                    {
                        worksheet.Cell(breakdownRow, 1).Value = emp.EmployeeCode;
                        worksheet.Cell(breakdownRow, 2).Value = emp.EmployeeName;
                        var empHoursCell = worksheet.Cell(breakdownRow, 3);
                        empHoursCell.Value = emp.TotalHours;
                        empHoursCell.Style.NumberFormat.Format = "0.00";
                        breakdownRow++;
                    }
                }

                for (int i = 1; i <= headers.Length; i++)
                {
                    worksheet.Column(i).AdjustToContents(10, 50);

                    if (i == 1 && employeeName != null)
                    {
                        worksheet.Column(i).Width = 40;
                    }
                    else if (i == 3 && employeeName == null)
                    {
                        worksheet.Column(i).Width = 40;
                    }
                    else if ((i == 6 && employeeName != null) ||
                             (i == 9 && employeeName == null))
                    {
                        worksheet.Column(i).Width = 40;
                    }
                }

                worksheet.Columns().AdjustToContents();

                var dataRange = worksheet.Range(headerRow, 1, currentRow - 1, headers.Length);
                dataRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                dataRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);


                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
        public async Task<List<EmployeeTaskReport>> GenerateTaskDailyReportAsync(DateTime date, string? managerId)
        {
            DateTime inputDate = date.Date;

            // Monday = 1, Sunday = 7
            int diff = (7 + (inputDate.DayOfWeek - DayOfWeek.Monday)) % 7;

            DateTime weekStart = inputDate.AddDays(-diff);
            DateTime weekEnd = weekStart.AddDays(7).AddSeconds(-1);


            using (var context = new AppDbContext())
            {
                return await GenerateTaskReportAsync("daily", weekStart, weekEnd, managerId);
            }

        }
        public async Task<TaskResponse> UpdateTaskAsync(UpdateTaskRequest request)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var existingTask = await context.Tasks.FirstOrDefaultAsync(c => c.Id == request.TaskId);

                    if (existingTask == null || existingTask.IsDelete)
                    {
                        throw new Exception("Task Not Found");
                    }

                    var assignBy = await context.Employees.Where(c => c.Code == request.AssignedBy).Select(c => new { c.Code, c.Name }).FirstOrDefaultAsync();
                    var assignedTo = await context.Employees.Where(c => request.AssignedTo.Contains(c.Code)).Select(c => new { c.Code, c.Name, c.FcmToken }).FirstOrDefaultAsync();
                    var timelog = await context.TaskTimeLogs.FirstOrDefaultAsync(a => a.TaskId == existingTask.Id && a.StopTime == null);                 

                    if (assignedTo == null || assignBy == null)
                    {
                        throw new Exception("Invalid employee IDs");
                    }

                    if (existingTask.Status != "completed")
                    {
                        if (!request.AssignedTo.Contains(existingTask.AssignedTo))
                        {
                            existingTask.AssignedTo = assignedTo.Code;
                            existingTask.AssignedToName = assignedTo.Name;
                            if (timelog != null)
                            {
                                existingTask.Status = "stopped";
                                timelog.StopTime = DateTime.Now;
                                context.TaskTimeLogs.Update(timelog);
                            }                          
                        
                        }
                        existingTask.Title = request.TaskTitle;
                        existingTask.Description = request.TaskDescription;
                        existingTask.TaskPreferance = request.TaskPreference;
                        existingTask.DueDate = request.DueDate;
                        existingTask.Priority = request.Priority;
                        existingTask.UpdatedAt = DateTime.Now;
                        if (existingTask.EntityCode != request.ClientCode)
                        {
                            var isClientExists = await context.Entities.AnyAsync(c => c.Code == request.ClientCode);

                            if (!isClientExists)
                            {
                                throw new Exception("Client Not Found");
                            }
                            
                            existingTask.EntityCode = request.ClientCode;
                        }
                        
                        context.Tasks.Update(existingTask);
                        await context.SaveChangesAsync();

                        // Send notification to employee
                        await _notificationService.SendTaskNotificationAsync(
                            assignBy.Code,
                            "Task Updated",
                            $"You updated a task: {request.TaskTitle}",
                            context,
                            new Dictionary<string, object>
                            {
                            { "taskId", existingTask.Id },
                            { "priority", existingTask.Priority }
                            }
                        );

                        await _notificationService.SendFcmNotificationAsync(assignedTo.Code, assignedTo.FcmToken,
                            "Task Updated", $"{assignBy.Name} updated a task: {request.TaskTitle}");
                        if (request.ClientCode != null) 
                        {
                            var clientName = await context.Entities.FirstOrDefaultAsync(a => a.Code == request.ClientCode);
                            _clientName = clientName.Name;
                        }

                        var task = new TaskResponse
                        {
                            TaskTitle = request.TaskTitle,
                            TaskDescription = request.TaskDescription,
                            AssignedBy = request.AssignedBy,
                            AssignedByName = assignBy.Name,
                            AssignedTo = assignedTo.Code,
                            AssignedToName = assignedTo.Name,
                            DueDate = request.DueDate,
                            Priority = request.Priority,
                            Status = "assigned",
                            ClientCode = request.ClientCode,
                            ClientName = _clientName
                        };

                        _logger.LogInformation($"✅ Task assigned: {existingTask.Title} → {assignedTo.Name}");

                        return task;
                    } else
                    {
                        // Send notification to employee
                        await _notificationService.SendTaskNotificationAsync(
                            assignBy.Code,
                            "Can't update a task",
                            $"{request.TaskTitle} Task is already completed",
                            context,
                            new Dictionary<string, object>
                            {
                            { "taskId", request.TaskId },
                            { "priority", request.Priority }
                            }
                        );
                        throw new Exception("Can;t update task. Task is already completed");
                    }
                }
            } catch (Exception e)
            {
                throw e;
            }
        }

        public async Task DeleteTaskAsync(DeleteTaskRequest request)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var isLeadExists = await context.Employees.AnyAsync(c => c.Code == request.LeadCode);
                    if (!isLeadExists) throw new Exception("Employee Lead doesn't exists");
                    var task = await context.Tasks.FirstOrDefaultAsync(c => c.Id == Guid.Parse(request.TaskId));
                    if (task == null) throw new Exception("Task not found");
                    if (task.AssignedBy != request.LeadCode) throw new Exception("The task is not assigned by this lead");
                    task.IsDelete = true;
                    task.DeletedAt = DateTime.Now;
                    context.Tasks.Update(task);
                    await context.SaveChangesAsync();
                }
            } catch (Exception e) { throw; }
        }
        public async Task<TaskResponse?> GetTaskByIdAsync(Guid taskId)
        {
            using (var context = new AppDbContext())
            {
                var tasks = await context.Tasks.ToListAsync();
                var query = (from t in tasks
                            join c in context.Entities
                            on t.EntityCode equals c.Code into tc
                            from c in tc.DefaultIfEmpty() // ensures default empty if no matching entity
                            where t.Id == taskId && !t.IsDelete
                            select new TaskResponse
                            {
                                Id = t.Id.ToString(),
                                TaskTitle = t.Title,
                                TaskDescription = t.Description,
                                AssignedBy = t.AssignedBy,
                                AssignedByName = t.AssignedByName ?? "",
                                AssignedTo = t.AssignedTo,
                                AssignedToName = t.AssignedToName ?? "",
                                TotalHoursWorked = t.TotalHoursWorked,
                                TaskPreference = t.TaskPreferance ?? "",
                                AssignedDate = t.AssignedDate,
                                DueDate = t.DueDate,
                                Priority = t.Priority,
                                ClientCode = t.EntityCode ?? "",
                                Status = t.Status,
                                ClientLocation = new ClientLocation
                                {
                                    Latitude = (double) c.latitude,
                                    Longitude = (double) c.longitude,
                                    AllowedRadiusMeters = (double) c.AllowedRadiusMeters
                                }
                            }).FirstOrDefault();
                //var s = new TaskModel
                //{
                //    Id = tasks.Id.ToString(),
                //    TaskTitle = tasks.Title,
                //    TaskDescription = tasks.Description,
                //    AssignedBy = tasks.AssignedBy,
                //    AssignedByName = tasks.AssignedByName,
                //    AssignedTo = tasks.AssignedTo,
                //    AssignedToName = tasks.AssignedToName,
                //    TotalHoursWorked = tasks.TotalHoursWorked,
                //    TaskPreference = tasks.TaskPreferance,
                //    AssignedDate = tasks.AssignedDate,
                //    DueDate = tasks.DueDate,
                //    Priority = tasks.Priority,
                //    Client = tasks.EntityCode,
                //    Status = tasks.Status
                //};
                return query;
            }
            //var tasks = await _storage.GetTasksAsync();
            //return tasks.FirstOrDefault(t => t.Id == taskId.ToString());
        }
        //public async Task<TaskResponse> StartTaskAsync(string employeeId, string taskId)
        //{
        //    try
        //    {
        //        //var task = await _storage.GetTaskByIdAsync(taskId);
        //        using (var context = new AppDbContext())
        //        {
        //            //var emp = await context.AppUsers.FirstOrDefaultAsync(a => a.Code == employeeId);
        //            var s = employeeId;

        //            var task = await context.Tasks.FirstOrDefaultAsync(c => c.Id == Guid.Parse(taskId));
        //            if (task == null)
        //            {
        //                throw new Exception("Task not found");
        //            }
        //            if (task.Status == "completed")
        //            {
        //                throw new Exception("Task already completed");
        //            }

        //            if (task.AssignedTo != employeeId)
        //            {
        //                throw new Exception("Task not assigned to this employee");
        //            }

        //            //var hasRunningTask = await context.Tasks.Where(t =>
        //            //     t.AssignedTo == employeeId &&
        //            //     (t.Status == "stopped" || t.Status == "assigned") &&
        //            //     t.Id != Guid.Parse(taskId)
        //            // ).ToListAsync();

        //            //var a = context.Tasks.Where(a => a.Status == "stopped" && a.Id != Guid.Parse(taskId));

        //            //var b = context.Tasks.Where(a => a.Status == "assigned" && a.Id != Guid.Parse(taskId));

        //            var anyInProgress = await context.Tasks.Where(a => a.AssignedTo == employeeId && a.Status == "in_progress" && a.Id != Guid.Parse(taskId)).ToListAsync();


        //            if (anyInProgress.Count != 0)
        //                throw new Exception("You need to stop/complete the previous task first");

        //            // Check if task already in progress today
        //            var today = DateTime.Today;
        //            var getCheckIn = await context.AttendanceRecords.FirstOrDefaultAsync(c => c.EmployeeCode == employeeId && c.Year == DateTime.Today.Year && c.Month == DateTime.Today.Month && c.Day == DateTime.Today.Day);

        //            if (getCheckIn.CheckInTime == null)
        //            {
        //                throw new Exception("You cannot start the task until you have checked in.");
        //            }

        //            // Check if task already in progress today
        //            //var today = DateTime.Today;
        //            //var todayLog = task.TimeLogs.FirstOrDefault(l => l.Date.Date == today);
        //            var todayLog = await context.TaskTimeLogs.FirstOrDefaultAsync(l => l.TaskId == Guid.Parse(taskId) && l.LogDate.Date == today);

        //            if (todayLog != null && todayLog.StartTime != null && todayLog.StopTime == null)
        //            {
        //                throw new Exception("Task already started today");
        //            }

        //            // Create new time log for today
        //            var timeLog = new TaskTimeLogs
        //            {
        //                Id = Guid.NewGuid(),
        //                TaskId = Guid.Parse(taskId),
        //                LogDate = today,
        //                StartTime = DateTime.Now
        //            };

        //            //task.TimeLogs.Add(timeLog);
        //            context.TaskTimeLogs.Add(timeLog);
        //            task.Status = "in_progress";
        //            context.Tasks.Update(task);
        //            //await _storage.UpdateTaskAsync(task);
        //            await context.SaveChangesAsync();

        //            _logger.LogInformation($"⏱️ Task started: {task.Title} by {task.AssignedToName}");

        //            var taskLogs = await context.TaskTimeLogs.Where(c => c.TaskId == task.Id).ToListAsync();
        //            var logs = _mapper.Map<List<TaskTimeLogResponse>>(taskLogs);

        //            var response = new TaskResponse()
        //            {
        //                Id = task.Id.ToString(),
        //                TaskTitle = task.Title,
        //                TaskDescription = task.Description,
        //                AssignedBy = task.AssignedBy,
        //                AssignedByName = task.AssignedByName,
        //                AssignedTo = task.AssignedTo,
        //                AssignedToName = task.AssignedToName,
        //                AssignedDate = task.AssignedDate,
        //                DueDate = task.DueDate,
        //                Status = task.Status,
        //                Priority = task.Priority,
        //                TimeLogs = logs
        //            };

        //            return response;
        //        }
        //    } catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        public async Task<TaskResponse> StartTaskAsync(string employeeId, string taskId)
        {
            try
            {
                bool isExtraHour = false;
                //var task = await _storage.GetTaskByIdAsync(taskId);
                using (var context = new AppDbContext())
                {
                    //var emp = await context.AppUsers.FirstOrDefaultAsync(a => a.Code == employeeId);
                    var s = employeeId;

                    var task = await context.Tasks.FirstOrDefaultAsync(c => c.Id == Guid.Parse(taskId));
                    if (task == null)
                    {
                        throw new Exception("Task not found");
                    }
                    if (task.Status == "completed")
                    {
                        throw new Exception("Task already completed");
                    }

                    if (task.AssignedTo != employeeId)
                    {
                        throw new Exception("Task not assigned to this employee");
                    }

                    //var hasRunningTask = await context.Tasks.Where(t =>
                    //     t.AssignedTo == employeeId &&
                    //     (t.Status == "stopped" || t.Status == "assigned") &&
                    //     t.Id != Guid.Parse(taskId)
                    // ).ToListAsync();

                    //var a = context.Tasks.Where(a => a.Status == "stopped" && a.Id != Guid.Parse(taskId));

                    //var b = context.Tasks.Where(a => a.Status == "assigned" && a.Id != Guid.Parse(taskId));

                    var anyInProgress = await context.Tasks.Where(a => a.AssignedTo == employeeId && a.Status == "in_progress" && a.Id != Guid.Parse(taskId)).ToListAsync();


                    if (anyInProgress.Count != 0)
                        throw new Exception("You need to stop/complete the previous task first");

                    // Check if task already in progress today
                    var today = DateTime.Today;
                    var getCheckIn = await context.AttendanceRecords.FirstOrDefaultAsync(c => c.EmployeeCode == employeeId && c.Year == DateTime.Today.Year && c.Month == DateTime.Today.Month && c.Day == DateTime.Today.Day);

                    // TO HANDLE EXTRA HOUR WORK (CASE 1)
                    if (getCheckIn?.CheckInTime == null)
                    {

                        isExtraHour = context.LeaveRequisitionMaster
                                    .Where(e => e.EmployeeCode == employeeId)
                                    .Any(e => context.LeaveRequisitionDetails.Any(d =>
                                        d.MasterId == e.Id &&
                                        d.FromDate <= today &&
                                        d.ToDate >= today
                                    ));
                        if (!isExtraHour)
                        {
                            throw new Exception("You cannot start the task until you have checked in.");
                        }

                    }
                    // TO HANDLE EXTRA HOUR WORK (CASE 2)
                    if (getCheckIn?.CheckOutTime != null)
                    {
                        var standardMinutes = context.StandardHours
                            .Where(x =>
                                x.GroupCode == getCheckIn.StandardHourCode &&
                                x.WeekDay == getCheckIn.StandardHourWeekday)
                            .Select(x => (int?)x.Minutes)
                            .FirstOrDefault();

                        if (
                            standardMinutes.HasValue &&
                            double.TryParse(getCheckIn.TotalHours, out double totalHours)
                        )
                        {
                            double todayMinutes = totalHours * 60;

                            isExtraHour = todayMinutes >= standardMinutes.Value;
                        }
                        else
                        {
                            isExtraHour = false;
                        }
                    }

                    // Check if task already in progress today
                    //var today = DateTime.Today;
                    //var todayLog = task.TimeLogs.FirstOrDefault(l => l.Date.Date == today);
                    var todayLog = await context.TaskTimeLogs.FirstOrDefaultAsync(l => l.TaskId == Guid.Parse(taskId) && l.LogDate.Date == today);

                    if (todayLog != null && todayLog.StartTime != null && todayLog.StopTime == null)
                    {
                        throw new Exception("Task already started today");
                    }

                    // Create new time log for today
                    var timeLog = new TaskTimeLogs
                    {
                        Id = Guid.NewGuid(),
                        TaskId = Guid.Parse(taskId),
                        LogDate = today,
                        StartTime = DateTime.Now,
                        IsExtraWork = isExtraHour,
                        EmployeeCode = employeeId
                    };

                    //task.TimeLogs.Add(timeLog);
                    context.TaskTimeLogs.Add(timeLog);
                    task.Status = "in_progress";
                    context.Tasks.Update(task);
                    //await _storage.UpdateTaskAsync(task);
                    await context.SaveChangesAsync();

                    _logger.LogInformation($"⏱️ Task started: {task.Title} by {task.AssignedToName}");

                    var taskLogs = await context.TaskTimeLogs.Where(c => c.TaskId == task.Id).ToListAsync();
                   
                   var logs = taskLogs.Adapt<List<TaskTimeLogResponse>>();


                    var response = new TaskResponse()
                    {
                        Id = task.Id.ToString(),
                        TaskTitle = task.Title,
                        TaskDescription = task.Description,
                        AssignedBy = task.AssignedBy,
                        AssignedByName = task.AssignedByName,
                        AssignedTo = task.AssignedTo,
                        AssignedToName = task.AssignedToName,
                        AssignedDate = task.AssignedDate,
                        DueDate = task.DueDate,
                        Status = task.Status,
                        Priority = task.Priority,
                        TimeLogs = logs,
                        IsExtraHours = isExtraHour
                    };

                    return response;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<TaskResponse> StopTaskAsync(string employeeId, string taskId, string notes)
        {
            using (var context = new AppDbContext())
            {

                var task = await context.Tasks.Where(c => c.Id == Guid.Parse(taskId)).FirstOrDefaultAsync();

                if (task.Status == "assigned") {

                    throw new Exception("Task is not started Yet!");
                }
                if (task == null)
                {
                    throw new Exception("Task not found");
                }
                //var emp = await context.AppUsers.FirstOrDefaultAsync(a => a.Code == employeeId);
                //var s = emp.EmployeeCode;
                if (task.AssignedTo != employeeId)
                {
                    throw new Exception("Task not assigned to this employee");
                }

                // Find today's time log
                var today = DateTime.Today;
                var todayLog = await context.TaskTimeLogs.FirstOrDefaultAsync(l =>
                    l.LogDate.Date == today &&
                    l.StartTime != null &&
                    l.StopTime == null);

                if (todayLog == null)
                {
                    throw new Exception("No active task session found for today");
                }

                // Stop the timer
                todayLog.StopTime = DateTime.Now;
                todayLog.Notes = notes;
                

                // Calculate hours worked
                var duration = todayLog.StopTime.Value - todayLog.StartTime.Value;
                todayLog.HoursWorked = duration.TotalHours;
                // Update total hours
                //task.TotalHoursWorked = task.TimeLogs.Sum(l => l.HoursWorked);
                //task.TotalHoursWorked = await context.TaskTimeLogs.Where(c => c.TaskId == task.Id).SumAsync(l => l.HoursWorked);
                var totalSeconds = await context.TaskTimeLogs.Where(c => c.TaskId == task.Id)
          .SumAsync(l => EF.Functions.DateDiffSecond(l.StartTime.Value, l.StopTime ?? DateTime.Now));
                double totalHours = totalSeconds / 3600.0;
                task.TotalHoursWorked = totalHours;
                task.Status = "stopped";
                //await _storage.UpdateTaskAsync(task);
                _logger.LogInformation($"⏱️ Task stopped: {task.Title} - {todayLog.HoursWorked:F2}h worked today");
                // Notify team lead
                await _notificationService.SendTaskNotificationAsync(
                    task.AssignedBy,
                    "Task Progress Update",
                    $"{task.AssignedToName} worked {todayLog.HoursWorked:F1}h on: {task.Title}",
                    context,
                    new Dictionary<string, object>
                    {
                        { "taskId", task.Id },
                        { "hoursWorked", todayLog.HoursWorked }
                    }
                );

                context.TaskTimeLogs.Update(todayLog);
                context.Tasks.Update(task);

                await context.SaveChangesAsync();
                var taskLogs = await context.TaskTimeLogs.Where(c => c.TaskId == task.Id).ToListAsync();
                var logs = taskLogs.Adapt<List<TaskTimeLogResponse>>();

                var response = new TaskResponse()
                {
                    Id = task.Id.ToString(),
                    TaskTitle = task.Title,
                    TaskDescription = task.Description,
                    AssignedBy = task.AssignedBy,
                    AssignedByName = task.AssignedByName,
                    AssignedTo = task.AssignedTo,
                    AssignedToName = task.AssignedToName,
                    AssignedDate = task.AssignedDate,
                    DueDate = task.DueDate,
                    Status = task.Status,
                    Priority = task.Priority,
                    TimeLogs = logs
                };

                return response;
            }
        }

        public async Task<TaskResponse> CompleteTaskAsync(string employeeId, string taskId)
        {
            using (var context = new AppDbContext())
            {
                var emp = await context.Employees.FirstOrDefaultAsync(c => c.Code == employeeId);

                var task = await context.Tasks.Where(c => c.Id == Guid.Parse(taskId)).FirstOrDefaultAsync();

                if (task == null)
                    throw new Exception("Task not found");

                if (task.AssignedTo != employeeId)
                    throw new Exception("Task not assigned to this employee");

                switch (task.Status)
                {
                    case "completed":
                        throw new Exception("Task is already completed");

                    case "assigned":
                        throw new Exception("Task must be started first");
                }
                var totalSeconds = await context.TaskTimeLogs.Where(c => c.TaskId == task.Id)
                                    .SumAsync(l => EF.Functions.DateDiffSecond(l.StartTime.Value, l.StopTime ?? DateTime.Now));
                double totalHours = totalSeconds / 3600.0;
                task.TotalHoursWorked = totalHours;
                task.Status = "completed";
                task.CompletedAt = DateTime.Now;
                context.Tasks.Update(task);
                //await _storage.UpdateTaskAsync(task);

                await context.SaveChangesAsync();

                var empLead = await (from a in context.Employees
                                     join b in context.Employees on a.LeadCode equals b.Code
                                     where a.Code == employeeId
                                     select new { b.Code, b.FcmToken }).FirstOrDefaultAsync();

                // Notify employee
                await _notificationService.SendTaskNotificationAsync(
                    task.AssignedBy,
                    "Task Completed",
                    $"{task.AssignedToName} completed: {task.Title} (Total: {task.TotalHoursWorked:F1}h)",
                    context,
                    new Dictionary<string, object>
                    {
                        { "taskId", task.Id },
                        { "totalHours", task.TotalHoursWorked }
                    }
                );

                // Notify team lead
                await _notificationService.SendFcmNotificationAsync(empLead.Code, empLead.FcmToken,
                        "Task Completed", $"{emp.Name} completed a task: {task.Title}");

                _logger.LogInformation($"✅ Task completed: {task.Title} - Total: {task.TotalHoursWorked:F2}h");

                var taskLogs = await context.TaskTimeLogs.Where(c => c.TaskId == task.Id).ToListAsync();
                var logs = taskLogs.Adapt<List<TaskTimeLogResponse>>();

                var response = new TaskResponse()
                {
                    Id = task.Id.ToString(),
                    TaskTitle = task.Title,
                    TaskDescription = task.Description,
                    AssignedBy = task.AssignedBy,
                    AssignedByName = task.AssignedByName,
                    AssignedTo = task.AssignedTo,
                    AssignedToName = task.AssignedToName,
                    AssignedDate = task.AssignedDate,
                    DueDate = task.DueDate,
                    Status = task.Status,
                    Priority = task.Priority,
                    TimeLogs = logs
                };

                return response;
            }
        }

        public async Task<TaskSummaryResponse> GetEmpTaskSummaryAsync(string empCode)
        {
            using (var context = new AppDbContext())
            {
                try
                {
                    var empTaskData = await context.Tasks.Where(t => t.AssignedTo == empCode && !t.IsDelete).ToListAsync();
                    var response = new TaskSummaryResponse()
                    {
                        ToDoCount = empTaskData.Where(t => t.Status != "completed").Count(),
                        InProgressCount = empTaskData.Where(t => t.Status == "in_progress").Count(),
                        CompleteCount = empTaskData.Where(t => t.Status == "completed").Count(),
                    };
                    return response;
                } catch (Exception e) { throw; }
            }
        }

        //public async Task CheckPreviousDayTaskDuration(string empId)
        //{
        //    using var context = new AppDbContext();

        //    var today = DateTime.Today;

        //    // 1️⃣ Get previous attendance record
        //    var prevAttendance = await context.AttendanceRecords
        //        .Where(x => x.EmployeeCode == empId &&
        //                    x.AttendanceDate < today &&
        //                    (x.CheckInTime.HasValue || x.CheckOutTime.HasValue))
        //        .OrderByDescending(x => x.AttendanceDate)
        //        .FirstOrDefaultAsync();

        //    if (prevAttendance == null ||
        //        !prevAttendance.CheckInTime.HasValue ||
        //        !prevAttendance.CheckOutTime.HasValue)
        //        return;

        //    // 2️⃣ Get in-progress task
        //    var task = await context.Tasks
        //        .FirstOrDefaultAsync(x => x.AssignedTo == empId && x.Status == "in_progress");

        //    if (task == null)
        //        return;

        //    var checkOut = prevAttendance.CheckOutTime.Value;
        //    var checkIn = prevAttendance.CheckInTime.Value;

        //    // 3️⃣ Get all logs of this task
        //   var logs = await context.TaskTimeLogs
        //        .Where(x => x.TaskId == task.Id && x.StartTime.HasValue)
        //        .ToListAsync();

        //    if (!logs.Any())
        //        return;

        //    // 4️⃣ Fix logs without StopTime
        //    foreach (var log in logs.Where(x => !x.StopTime.HasValue))
        //    {
        //        log.StopTime = checkOut;
        //        log.HoursWorked = (checkOut - log.StartTime.Value).TotalHours;
        //    }

        //    // 5️⃣ Ensure task time does not exceed attendance duration
        //    var attendanceMinutes = (checkOut - checkIn).TotalMinutes;

        //    var totalMinutes = logs
        //        .Where(x => x.StopTime.HasValue)
        //        .Sum(x => (x.StopTime.Value - x.StartTime.Value).TotalMinutes);

        //    if (totalMinutes > attendanceMinutes)
        //    {
        //        var lastLog = logs
        //            .Where(x => x.StopTime.HasValue)
        //            .OrderByDescending(x => x.StopTime)
        //            .FirstOrDefault();

        //        if (lastLog != null)
        //        {
        //            lastLog.StopTime = checkOut;
        //            lastLog.HoursWorked = (checkOut - lastLog.StartTime.Value).TotalHours;
        //        }
        //    }

        //    // 6️⃣ Recalculate total hours
        //    var totalSeconds = logs.Sum(x =>
        //        (int)((x.StopTime ?? checkOut) - x.StartTime!.Value).TotalSeconds);

        //    task.TotalHoursWorked = totalSeconds / 3600.0;
        //    task.Status = "stopped";

        //    await context.SaveChangesAsync();
        //}

        public async Task CheckPreviousDayTaskDuration(string empId)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var prevDayCheckInOut = await context.AttendanceRecords
                        .Where(c => c.EmployeeCode == empId && c.AttendanceDate != DateTime.Today
                                && (c.CheckInTime.HasValue || c.CheckOutTime.HasValue))
                        .OrderByDescending(c => c.AttendanceDate)
                        .Select(c => new { c.AttendanceDate, c.CheckInTime, c.CheckOutTime }).FirstOrDefaultAsync();

                    if (prevDayCheckInOut == null || !prevDayCheckInOut.CheckInTime.HasValue || !prevDayCheckInOut.CheckOutTime.HasValue)
                        return;

                    var prevDayTasks = await context.Tasks.Where(c => c.Status == "in_progress" && c.AssignedTo == empId).FirstOrDefaultAsync();
                    var prevDayTaskLogWithNoStopTime = await (from a in context.Tasks
                                                              join b in context.TaskTimeLogs on a.Id equals b.TaskId
                                                              where a.Status == "in_progress" && a.AssignedTo == empId
                                                              && b.StartTime.HasValue && !b.StopTime.HasValue
                                                              select b).ToListAsync();

                    if (prevDayTaskLogWithNoStopTime.Any())
                    {
                        foreach (var a in prevDayTaskLogWithNoStopTime)
                        {
                            a.StopTime = prevDayCheckInOut.CheckOutTime;
                            var hrsWorked = prevDayCheckInOut.CheckOutTime.Value - a.StartTime.Value;
                            a.HoursWorked = Convert.ToDouble(hrsWorked.TotalHours);
                            context.TaskTimeLogs.Update(a);
                        }
                        var totalSeconds = await context.TaskTimeLogs.Where(c => c.TaskId == prevDayTasks.Id)
                                        .SumAsync(l => EF.Functions.DateDiffSecond(l.StartTime.Value, l.StopTime ?? prevDayCheckInOut.CheckOutTime.Value));
                        double totalHours = totalSeconds / 3600.0;
                        prevDayTasks.TotalHoursWorked = totalHours;
                        prevDayTasks.Status = "stopped";
                        context.Tasks.Update(prevDayTasks);
                        await context.SaveChangesAsync();
                    }
                    else
                    {

                        var prevDayTaskHours = await (from a in context.Tasks
                                                      join b in context.TaskTimeLogs on a.Id equals b.TaskId
                                                      where a.Status == "in_progress" && a.AssignedTo == empId && b.StartTime.HasValue
                                                      && b.StopTime.HasValue && b.StartTime.Value.Date == prevDayCheckInOut.AttendanceDate
                                                      group b by b.TaskId into g
                                                      select new
                                                      {
                                                          TaskId = g.Key,
                                                          StartTime = g.Max(x => x.StartTime),
                                                          TotalDuration = g.Sum(x => EF.Functions.DateDiffMinute(x.StartTime.Value, x.StopTime.Value))
                                                      }).ToListAsync();

                        var prevDayWorkHours = prevDayCheckInOut.CheckOutTime - prevDayCheckInOut.CheckInTime;

                        if (prevDayTaskHours.Any())
                        {
                            foreach (var a in prevDayTaskHours)
                            {
                                if (a.TotalDuration > prevDayWorkHours.Value.TotalMinutes)
                                {
                                    var lastLog = await context.TaskTimeLogs.Where(c => c.TaskId == a.TaskId).OrderByDescending(c => c.StopTime).FirstOrDefaultAsync();
                                    lastLog.StopTime = prevDayCheckInOut.CheckOutTime;
                                    var hrsWorked = prevDayCheckInOut.CheckOutTime.Value - a.StartTime.Value;
                                    lastLog.HoursWorked = Convert.ToDouble(hrsWorked.TotalHours);
                                    context.TaskTimeLogs.Update(lastLog);
                                }
                            }
                            var totalSeconds = await context.TaskTimeLogs.Where(c => c.TaskId == prevDayTasks.Id)
                                        .SumAsync(l => EF.Functions.DateDiffSecond(l.StartTime.Value, l.StopTime ?? prevDayCheckInOut.CheckOutTime.Value));
                            double totalHours = totalSeconds / 3600.0;
                            prevDayTasks.TotalHoursWorked = totalHours;
                            prevDayTasks.Status = "stopped";
                            context.Tasks.Update(prevDayTasks);
                            await context.SaveChangesAsync();
                        }
                    }
                }
            } catch (Exception e)
            {
                throw;
            }
        }

        public string DetermineViewType(DateTime startDate, DateTime endDate)
        {
            if (startDate.Date == endDate.Date)
                return "daily";

            if (startDate.Day == 1 &&
                endDate == startDate.AddMonths(1).AddDays(-1))
                return "monthly";

            if (startDate.Month == 1 && startDate.Day == 1 &&
                endDate.Month == 12 && endDate.Day == 31 &&
                endDate.Year == startDate.Year)
                return "yearly";

            return "custom";
        }
    }
}
