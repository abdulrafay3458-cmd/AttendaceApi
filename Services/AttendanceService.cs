namespace AttendanceAPI.Services
{
    using AttendanceAPI.Contracts.Request;
    using AttendanceAPI.Contracts.Response;
    using AttendanceAPI.Data;
    using AttendanceAPI.Entities;
    using AttendanceAPI.Interface;
    using DocumentFormat.OpenXml.InkML;
    using Mapster;
    using Microsoft.AspNetCore.Mvc.Diagnostics;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Globalization;
 

    public class AttendanceService : IAttendenceServices
    {
       
        private readonly NotificationService _notificationService;
        private readonly ILogger<AttendanceService> _logger;
        private readonly IHubContext<AttendanceHub> _hubContext;
        private readonly ITaskService _taskService;
        private readonly TimeSpan _standardStartTime = new TimeSpan(9, 0, 0); // 9:00 AM
        private readonly int _graceMinutes = 30; // 30 minutes grace period
        private readonly AppDbContext _context;
        public AttendanceService(
            
            NotificationService notificationService,
            ILogger<AttendanceService> logger,
            IHubContext<AttendanceHub> hubContext,
            ITaskService taskService,
            AppDbContext context
           )
        {
            
            _notificationService = notificationService;
            _logger = logger;
            _hubContext = hubContext;
            _taskService = taskService;
            _context = context;

        }
        public async Task<Employee?> GetEmployeeByIdAsync(string employeeId)
        {
            using (var _context = new AppDbContext())
            {

                return _context.Employees.FirstOrDefault(e => e.Code == employeeId);
            }
        }
        public async Task<AttendanceRecord> MarkManualCheckIn(ManualAttendanceRequest request)
        {
            var checkInTime = DateTime.Parse(request.Time);
            using (var _context = new AppDbContext())
            {
            
                var record = await _context.AttendanceRecords
              .FirstOrDefaultAsync(a => a.EmployeeCode == request.EmployeeCode
                                && a.Day == request.Date.Day && a.Month == request.Date.Month && a.Year == request.Date.Year);

                if (record == null)
                {
                    var data = new AttendanceRecord
                    {
                        EmployeeCode = request.EmployeeCode,
                        CheckInSource = "manual",
                        //CheckInLatitude = record.CheckInLatitude,
                        //CheckInLongitude = record.CheckInLongitude,
                        Year = request.Date.Year,
                        Month = request.Date.Month,
                        Day = request.Date.Day,
                        CheckInTime = checkInTime,
                        AttendanceDate = request.Date,
                        StandardHourWeekday = ((int)DateTime.Now.DayOfWeek + 6) % 7 + 1,
                        Status = CheckTime(request.EmployeeCode, ((int)DateTime.Now.DayOfWeek + 6) % 7 + 1, _graceMinutes),
                        CheckOutSource = "",
                        Fortnight = request.Date.Day <= 15 ? 1 : 2,
                    };

                    _context.AttendanceRecords.Add(data);
                    record = data; 
                }
                else
                {
                    record.CheckInTime = checkInTime ;
                }

                _context.SaveChanges();
                return record;
            }
        }
        public async Task<AttendanceRecord?> MarkManualCheckOut(ManualAttendanceRequest request)
        {
            var checkOutTime = DateTime.Parse(request.Time);

            using (var _context = new AppDbContext())
            {
                var record = await _context.AttendanceRecords
                    .FirstOrDefaultAsync(a => a.EmployeeCode == request.EmployeeCode
                        && a.Day == request.Date.Day
                        && a.Month == request.Date.Month
                        && a.Year == request.Date.Year);

                if (record == null || record.CheckInTime == null)
                    return null;

                if (checkOutTime <= record.CheckInTime)
                    throw new Exception("Checkout must be after check-in");

                record.CheckOutTime = checkOutTime;
                record.CheckOutSource = "manual";

                await _context.SaveChangesAsync();

                return record;
            }
        }
        public async Task<bool> GetEmployeeActiveTask(string employeeId)
        {
            using (var _context = new AppDbContext())
            {
                var a = await _context.Tasks.FirstOrDefaultAsync(e => e.AssignedTo == employeeId && e.Status == "in_progress");

                return a == null ? true : false;
            }
        }
        public async Task<AttendanceRecord?> GetAttendanceByDateAsync(string employeeId, int day, int month, int year)
        {
            using (var _context = new AppDbContext())
            {
                var record = await _context.AttendanceRecords.FirstOrDefaultAsync(r =>
                r.EmployeeCode == employeeId &&
                r.Day == day && r.Month == month && r.Year == year);
                // var records = await GetAttendanceRecordsAsync();
                return record;
            }
        }
        public async Task<List<AttendanceRecord>> GetAttendence(string empCode)
        {
            using (var _context = new AppDbContext())
            {
                var attendenceList = await _context.AttendanceRecords.Where(a => a.EmployeeCode == empCode).ToListAsync();
                return attendenceList;
            }
        }
        public async Task<bool> AttendenceExisitAsync(string empCode, DateTime date)
        {
            using (var _context = new AppDbContext())
            {
                return await _context.AttendanceRecords
                .AnyAsync(a =>
                    a.EmployeeCode == empCode &&
                    a.AttendanceDate.Date == date.Date);
            }
        }
        public async Task<List<AttendanceHistory>> GetEmployeeAttendanceAsync(string employeeId, int days)
        {
            using (var _context = new AppDbContext())
            {
                var startDate = DateTime.Today.AddDays(-days);
                var recprd = _context.AttendanceRecords
                     .Where(r => r.EmployeeCode == employeeId && r.AttendanceDate >= startDate && (r.Status == "Present" || r.Status == "Late"))
                     .Select(a=> new AttendanceHistory()
                     {
                         AttendanceDate = a.AttendanceDate,
                         CheckInSource = a.CheckInSource,
                         TotalHours = a.TotalHours,
                         CheckInTime = a.CheckInTime,
                         CheckOutTime = a.CheckOutTime,
                         Status = a.Status == "Present" ? "On Time" : a.Status,
                         DeviceName = _context.MachineDevices.FirstOrDefault(d => d.Id == a.CheckInDeviceId).Name ?? "--"
                         
                     })
                     .OrderByDescending(r => r.AttendanceDate)
                     .ToList();
                return recprd;
            }
        }
        public Task<List<AttendanceRecord>> GetAttendanceByEmployeeAndDateRangeAsync(string employeeId, DateTime startDate, DateTime endDate)
        {
            using (var _context = new AppDbContext())
            {
                return _context.AttendanceRecords.Where(r =>
                r.EmployeeCode == employeeId &&
                r.AttendanceDate >= startDate && r.AttendanceDate <= endDate).ToListAsync();
            }
        }
        public async Task<AttendanceRecord> GetTodayAttendanceAsync(string employeeId)
        {
            using (var _context = new AppDbContext())
            {
                var today = DateTime.Today;

                // Filter only today's records for the given employee
                var todayEmployeeRecords = _context.AttendanceRecords
                    .Where(r => r.EmployeeCode == employeeId && r.AttendanceDate.Date == today)
                    .ToList();

                // If employee has no record today → return null
                if (!todayEmployeeRecords.Any())
                    return null;

                // Safely get earliest check-in
                var earliestCheckIn = todayEmployeeRecords
                    .Where(r => r.CheckInTime.HasValue)
                    .OrderBy(r => r.CheckInTime.Value)
                    .Select(r => r.CheckInTime)
                    .FirstOrDefault(); // returns null if no checkin

                // Safely get latest check-out
                var latestCheckOut = todayEmployeeRecords
                    .Where(r => r.CheckOutTime.HasValue)
                    .OrderByDescending(r => r.CheckOutTime.Value)
                    .Select(r => r.CheckOutTime)
                    .FirstOrDefault(); // returns null if no checkout

                // Get the main attendance record (first one)
                var attendance = todayEmployeeRecords.First();

                // Assign safely
                attendance.CheckInTime = earliestCheckIn;
                attendance.CheckOutTime = latestCheckOut;

                if (earliestCheckIn.HasValue && latestCheckOut.HasValue)
                {
                    TimeSpan totalDuration = latestCheckOut.Value - earliestCheckIn.Value;
                    string totalDurationText = totalDuration.ToString(@"hh\:mm");

                    attendance.TotalHours = totalDurationText;

                }


                return attendance;
            }
        }
        public async Task<AttendanceRecord> AddLeaveDay(AttendanceRecord record)
        {
            AttendanceRecord attendanceRecord = new AttendanceRecord();
            using (var _context = new AppDbContext())
            {
                try
                {
                    attendanceRecord.EmployeeCode = record.EmployeeCode;
                    attendanceRecord.AttendanceDate = record.AttendanceDate;
                    attendanceRecord.Day = record.Day;
                    attendanceRecord.Month = record.Month;
                    attendanceRecord.Year = record.Year;
                    attendanceRecord.Fortnight = record.Fortnight;
                    await _context.AttendanceRecords.AddAsync(attendanceRecord);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                }
            }
            return record;
        }

        //public async Task ProcessMachineRecord(List<AttendanceRecord> records)
        //{
        //    try
        //    {
        //        using (var context = new AppDbContext())
        //        {
        //            var recordList = new List<AttendanceRecord>();

        //            var empInfo = await context.Employees.Where(c => records.Select(c => c.EmployeeCode).Contains(c.Code)).Select(c => new { c.Code, c.FcmToken }).ToListAsync();

        //            foreach (var record in records)
        //            {
        //                var emp = empInfo.FirstOrDefault(c => c.Code == record.EmployeeCode);

        //                var existingRecord = await context.AttendanceRecords.FirstOrDefaultAsync(c => c.EmployeeCode == record.EmployeeCode &&
        //                                                                                c.Day == record.Day && c.Month == record.Month && c.Year == record.Year);
        //                var isRecordExists = existingRecord != null;
        //                var isRecordInList = recordList.Any(c => c.EmployeeCode == record.EmployeeCode && c.Day == record.Day && c.Month == record.Month && c.Year == record.Year);

        //                var isLeaveExists = await context.AttendanceLeaves.AnyAsync(c => c.EmployeeCode == record.EmployeeCode && c.Day == record.Day &&
        //                c.Month == record.Month && c.Year == record.Year && c.TypeCode != "HD");

        //                //var newOrUpdatedRecord = new AttendanceRecord();
        //                if (!isRecordExists) await _taskService.CheckPreviousDayTaskDuration(record.EmployeeCode);

        //                if (!isLeaveExists)
        //                {
        //                    if (record.CheckInTime.HasValue)
        //                    {
        //                        if (isRecordExists)
        //                        {
        //                            if (!existingRecord.CheckInTime.HasValue || existingRecord.CheckInTime > record.CheckInTime)
        //                            {
        //                                existingRecord.CheckInTime = record.CheckInTime.Value;
        //                                existingRecord.CheckInSource = record.CheckInSource;
        //                                existingRecord.CheckInDeviceId = record.CheckInDeviceId;
        //                                context.AttendanceRecords.Update(existingRecord);

        //                                await _notificationService.SendFcmNotificationAsync(emp.Code, emp.FcmToken,
        //                                    "Check-In Alert", $"You checkedIn at {record.CheckInTime.Value.TimeOfDay}");
        //                            }

        //                        }
        //                        else if (!isRecordInList)
        //                        {
        //                            var newRecord = record.Adapt<AttendanceRecord>();
        //                            var timeInString = await context.StandardHours.Where(c => c.GroupCode == record.StandardHourCode && c.WeekDay == record.StandardHourWeekday)
        //                                .Select(c => c.TimeIn).FirstOrDefaultAsync();
        //                            if (!TimeSpan.TryParseExact(timeInString, @"hh\:mm", CultureInfo.InvariantCulture, out TimeSpan inputTime))
        //                            {
        //                                throw new ArgumentException("Invalid time format. Use HH:mm format.");
        //                            }

        //                            TimeSpan targetTime = inputTime.Add(TimeSpan.FromMinutes(30));
        //                            TimeSpan? checkInTime = record.CheckInTime?.TimeOfDay;

        //                            newRecord.Status = checkInTime <= targetTime ? "Present" : "Late";

        //                            recordList.Add(newRecord);
        //                            context.AttendanceRecords.Add(newRecord);

        //                            await _notificationService.SendFcmNotificationAsync(emp.Code, emp.FcmToken,
        //                                    "Check-In Alert", $"You checkedIn at {record.CheckInTime.Value.TimeOfDay}");
        //                        }
        //                    }
        //                    else if (record.CheckOutTime.HasValue)
        //                    {
        //                        if (isRecordExists)
        //                        {
        //                            if (!existingRecord.CheckOutTime.HasValue || (record.CheckOutTime > existingRecord.CheckOutTime))
        //                            {
        //                                existingRecord.CheckOutTime = record.CheckOutTime.Value;
        //                                existingRecord.CheckOutSource = record.CheckOutSource;
        //                                existingRecord.CheckOutDeviceId = record.CheckOutDeviceId;
        //                                var timeDiff = record.CheckOutTime.Value - existingRecord.CheckInTime.Value;
        //                                existingRecord.TotalHours = Math.Round(timeDiff.TotalHours, 4).ToString();
        //                                context.AttendanceRecords.Update(existingRecord);

        //                                await _notificationService.SendFcmNotificationAsync(emp.Code, emp.FcmToken,
        //                                    "Check-Out Alert", $"You checkedOut at {record.CheckOutTime.Value.TimeOfDay}");
        //                            }
        //                        }
        //                        else if (!isRecordExists && !isRecordInList)
        //                        {
        //                            var newRecord = record.Adapt<AttendanceRecord>();

        //                            newRecord.Status = "Present";

        //                            recordList.Add(newRecord);
        //                            context.AttendanceRecords.Add(newRecord);

        //                            await _notificationService.SendFcmNotificationAsync(emp.Code, emp.FcmToken,
        //                                    "Check-Out Alert", $"You checkedOut at {record.CheckOutTime.Value.TimeOfDay}");
        //                        }
        //                        else if (!isRecordExists && isRecordInList)
        //                        {
        //                            var recToEdit = recordList.FirstOrDefault(c => c.EmployeeCode == record.EmployeeCode && c.Day == record.Day && c.Month == record.Month
        //                            && c.Year == record.Year);
        //                            if (recToEdit == null) continue;
        //                            recToEdit.CheckOutTime = record.CheckOutTime;
        //                            recToEdit.CheckOutSource = record.CheckOutSource;
        //                            recToEdit.CheckOutDeviceId = record.CheckOutDeviceId;
        //                            var timeDiff = recToEdit.CheckOutTime.Value - recToEdit.CheckInTime.Value;
        //                            recToEdit.TotalHours = Math.Round(timeDiff.TotalHours, 4).ToString();
        //                            //context.AttendanceRecords.Update(recToEdit);

        //                            await _notificationService.SendFcmNotificationAsync(emp.Code, emp.FcmToken,
        //                                    "Check-Out Alert", $"You checkedOut at {record.CheckOutTime.Value.TimeOfDay}");
        //                        }
        //                    }
        //                }
        //                //var hubContext = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IHubContext<AttendanceHub>>();
        //                // Broadcast via SignalR

        //                //var empInfo = await context.Employees.Where(c => c.Code == record.EmployeeCode).Select(c => new {c.Name, c.LeadCode}).FirstOrDefaultAsync();

        //                //var stoppingToken = new CancellationToken();

        //                //var hubResponse = new AttendanceUpdateHubResponse()
        //                //{
        //                //    EmployeeName = empInfo.Name,
        //                //    DeviceName = record.DeviceName,
        //                //    CheckInTime = record.CheckInTime,
        //                //    CheckOutTime = record.CheckOutTime
        //                //};

        //                //await _hubContext.Clients.User(empInfo.LeadCode).SendAsync(
        //                //    "ReceiveAttendanceUpdate",
        //                //    hubResponse,
        //                //    stoppingToken);
        //            }
        //            await context.SaveChangesAsync();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        public async Task ProcessMachineRecord(List<AttendanceRecord> records)
        {
            try
            {
                if (records == null || records.Count == 0)
                {
                    _logger.LogWarning("No records to process");
                    return;
                }

                _logger.LogInformation($"Processing {records.Count} attendance records");

             
                foreach (var punch in records)
                {
                    try
                    {
                        if (punch == null || string.IsNullOrEmpty(punch.EmployeeCode))
                            continue;

                        if (!punch.CheckInTime.HasValue && !punch.CheckOutTime.HasValue)
                            continue;

                        var punchTime = punch.CheckInTime ?? punch.CheckOutTime;

                        var deviceId = punch.CheckInTime.HasValue
                            ? punch.CheckInDeviceId
                            : punch.CheckOutDeviceId;

                        var latitude = punch.CheckInTime.HasValue
                            ? punch.CheckInLatitude
                            : punch.CheckOutLatitude;

                        var longitude = punch.CheckInTime.HasValue
                            ? punch.CheckInLongitude
                            : punch.CheckOutLongitude;

                        var address = punch.CheckInTime.HasValue
                            ? punch.CheckInAddress
                            : punch.CheckOutAddress;

                        var source = punch.CheckInTime.HasValue
                            ? punch.CheckInSource ?? "zkteco"
                            : punch.CheckOutSource ?? "zkteco";

                        var historyExists = await _context.AttendenceHistories
                            .AnyAsync(h =>
                                h.EmployeeCode == punch.EmployeeCode &&
                                h.AttendanceDate == punch.AttendanceDate &&
                                h.CheckInTime == punch.CheckInTime &&
                                h.CheckOutTime == punch.CheckOutTime &&
                                h.DeviceId == deviceId &&
                                h.Source == source);

                        if (historyExists)
                        {
                            _logger.LogDebug(
                                $"History already exists for employee {punch.EmployeeCode} " +
                                $"on {punch.AttendanceDate:yyyy-MM-dd}");

                            continue;
                        }

                        var history = new AttendenceHistory
                        {
                            AttendenceHistoryId = Guid.NewGuid(),

                            EmployeeCode = punch.EmployeeCode,

                            CheckInTime = punch.CheckInTime,
                            CheckOutTime = punch.CheckOutTime,

                            AttendanceDate = punch.AttendanceDate,

                            DeviceId = deviceId,

                            Latitude = latitude,
                            Longitude = longitude,

                       //     Address = address,

                            Source = source
                        };

                        await _context.AttendenceHistories.AddAsync(history);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            $"Error saving attendance history for employee {punch?.EmployeeCode}");
                    }
                }               
                await _context.SaveChangesAsync();



                // Group records by employee and date to combine check-in/check-out
                var groupedRecords = records
                    .Where(r => r != null && !string.IsNullOrEmpty(r.EmployeeCode))
                    .GroupBy(r => new { r.EmployeeCode, r.Year, r.Month, r.Day, r.Fortnight })
                    .Select(g => new
                    {
                        Key = g.Key,
                        CheckIn = g.Where(r => r.CheckInTime.HasValue)
                                   .OrderBy(r => r.CheckInTime)
                                   .FirstOrDefault(),
                        CheckOut = g.Where(r => r.CheckOutTime.HasValue)
                                    .OrderByDescending(r => r.CheckOutTime)
                                    .FirstOrDefault(),
                        AllRecords = g.ToList()
                    })
                    .ToList();

                var processedCount = 0;
                var errorCount = 0;
                var insertedCount = 0;
                var updatedCount = 0;

                foreach (var grouped in groupedRecords)
                {
                    try
                    {
                        // Check if record already exists in database
                        var existingRecord = await _context.AttendanceRecords
                            .FirstOrDefaultAsync(r =>
                                r.EmployeeCode == grouped.Key.EmployeeCode &&
                                r.Year == grouped.Key.Year &&
                                r.Month == grouped.Key.Month &&
                                r.Day == grouped.Key.Day &&
                                r.Fortnight == grouped.Key.Fortnight);

                        AttendanceRecord recordToSave;

                        if (existingRecord != null)
                        {
                            // Update existing record
                            recordToSave = existingRecord;

                            // Update check-in info if available and better (earlier)
                            if (grouped.CheckIn != null)
                            {
                                if (!recordToSave.CheckInTime.HasValue ||
                                    grouped.CheckIn.CheckInTime.Value < recordToSave.CheckInTime.Value)
                                {
                                    recordToSave.CheckInTime = grouped.CheckIn.CheckInTime;
                                    recordToSave.CheckInSource = grouped.CheckIn.CheckInSource ?? "zkteco";
                                    recordToSave.CheckInDeviceId = grouped.CheckIn.CheckInDeviceId;
                                    recordToSave.CheckInLatitude = grouped.CheckIn.CheckInLatitude;
                                    recordToSave.CheckInLongitude = grouped.CheckIn.CheckInLongitude;
                                    recordToSave.CheckInAddress = grouped.CheckIn.CheckInAddress;
                                    recordToSave.CheckInPhoto = grouped.CheckIn.CheckInPhoto;
                                }
                            }

                            // Update check-out info if available and better (later)
                            if (grouped.CheckOut != null)
                            {
                                if (!recordToSave.CheckOutTime.HasValue ||
                                    grouped.CheckOut.CheckOutTime.Value > recordToSave.CheckOutTime.Value)
                                {
                                    recordToSave.CheckOutTime = grouped.CheckOut.CheckOutTime;
                                    recordToSave.CheckOutSource = grouped.CheckOut.CheckOutSource ?? "zkteco";
                                    recordToSave.CheckOutDeviceId = grouped.CheckOut.CheckOutDeviceId;
                                    recordToSave.CheckOutLatitude = grouped.CheckOut.CheckOutLatitude;
                                    recordToSave.CheckOutLongitude = grouped.CheckOut.CheckOutLongitude;
                                    recordToSave.CheckOutAddress = grouped.CheckOut.CheckOutAddress;
                                    recordToSave.CheckOutPhoto = grouped.CheckOut.CheckOutPhoto;
                                }
                            }

                            // Calculate total hours
                            if (recordToSave.CheckInTime.HasValue && recordToSave.CheckOutTime.HasValue)
                            {
                                var timeDiff = recordToSave.CheckOutTime.Value - recordToSave.CheckInTime.Value;
                                if (timeDiff.TotalHours > 0)
                                {
                                    recordToSave.TotalHours = Math.Round(timeDiff.TotalHours, 2).ToString("F2");
                                }
                                else
                                {
                                    // If checkout is before checkin (overnight shift), add 24 hours
                                    var adjustedDiff = timeDiff.Add(TimeSpan.FromHours(24));
                                    recordToSave.TotalHours = Math.Round(adjustedDiff.TotalHours, 2).ToString("F2");
                                }
                            }

                            // Set attendance date
                            if (grouped.CheckIn != null)
                            {
                                recordToSave.AttendanceDate = grouped.CheckIn.AttendanceDate;
                            }
                            else if (grouped.CheckOut != null)
                            {
                                recordToSave.AttendanceDate = grouped.CheckOut.AttendanceDate;
                            }

                            // Determine status
                            if (recordToSave.CheckInTime.HasValue)
                            {
                                recordToSave.Status = DetermineAttendanceStatus(
                                    recordToSave.EmployeeCode,
                                    recordToSave.StandardHourWeekday ?? 0,
                                    recordToSave.CheckInTime.Value.TimeOfDay);
                            }

                            _context.AttendanceRecords.Update(recordToSave);
                            updatedCount++;
                            _logger.LogDebug($"Updated attendance record for {recordToSave.EmployeeCode} on {recordToSave.AttendanceDate:yyyy-MM-dd}");
                        }
                        else
                        {
                            // Create new record
                            recordToSave = new AttendanceRecord
                            {
                                EmployeeCode = grouped.Key.EmployeeCode,
                                Year = grouped.Key.Year,
                                Month = grouped.Key.Month,
                                Day = grouped.Key.Day,
                                Fortnight = grouped.Key.Fortnight,
                                StandardHourWeekday = ((int)new DateTime(grouped.Key.Year, grouped.Key.Month, grouped.Key.Day).DayOfWeek + 6) % 7 + 1,
                                AttendanceDate = grouped.CheckIn?.AttendanceDate ?? grouped.CheckOut?.AttendanceDate ?? new DateTime(grouped.Key.Year, grouped.Key.Month, grouped.Key.Day),
                            };

                            // Set check-in
                            if (grouped.CheckIn != null)
                            {
                                recordToSave.CheckInTime = grouped.CheckIn.CheckInTime;
                                recordToSave.CheckInSource = grouped.CheckIn.CheckInSource ?? "zkteco";
                                recordToSave.CheckInDeviceId = grouped.CheckIn.CheckInDeviceId;
                                recordToSave.CheckInLatitude = grouped.CheckIn.CheckInLatitude;
                                recordToSave.CheckInLongitude = grouped.CheckIn.CheckInLongitude;
                                recordToSave.CheckInAddress = grouped.CheckIn.CheckInAddress;
                                recordToSave.CheckInPhoto = grouped.CheckIn.CheckInPhoto;
                            }

                            // Set check-out
                            if (grouped.CheckOut != null)
                            {
                                recordToSave.CheckOutTime = grouped.CheckOut.CheckOutTime;
                                recordToSave.CheckOutSource = grouped.CheckOut.CheckOutSource ?? "zkteco";
                                recordToSave.CheckOutDeviceId = grouped.CheckOut.CheckOutDeviceId;
                                recordToSave.CheckOutLatitude = grouped.CheckOut.CheckOutLatitude;
                                recordToSave.CheckOutLongitude = grouped.CheckOut.CheckOutLongitude;
                                recordToSave.CheckOutAddress = grouped.CheckOut.CheckOutAddress;
                                recordToSave.CheckOutPhoto = grouped.CheckOut.CheckOutPhoto;
                            }

                            // Calculate total hours
                            if (recordToSave.CheckInTime.HasValue && recordToSave.CheckOutTime.HasValue)
                            {
                                var timeDiff = recordToSave.CheckOutTime.Value - recordToSave.CheckInTime.Value;
                                if (timeDiff.TotalHours > 0)
                                {
                                    recordToSave.TotalHours = Math.Round(timeDiff.TotalHours, 2).ToString("F2");
                                }
                                else
                                {
                                    var adjustedDiff = timeDiff.Add(TimeSpan.FromHours(24));
                                    recordToSave.TotalHours = Math.Round(adjustedDiff.TotalHours, 2).ToString("F2");
                                }
                            }

                            // Determine status
                            if (recordToSave.CheckInTime.HasValue)
                            {
                                recordToSave.Status = DetermineAttendanceStatus(
                                    recordToSave.EmployeeCode,
                                    recordToSave.StandardHourWeekday ?? 0,
                                    recordToSave.CheckInTime.Value.TimeOfDay);
                            }

                            await _context.AttendanceRecords.AddAsync(recordToSave);
                            insertedCount++;
                            _logger.LogDebug($"Added new attendance record for {recordToSave.EmployeeCode} on {recordToSave.AttendanceDate:yyyy-MM-dd}");
                        }

                        processedCount++;

                        // Save in batches every 50 records
                        if (processedCount % 50 == 0)
                        {
                            await _context.SaveChangesAsync();
                            _logger.LogInformation($"Saved {processedCount} records. Inserted: {insertedCount}, Updated: {updatedCount}, Errors: {errorCount}");
                        }
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        _logger.LogError(ex, $"Error processing record for employee: {grouped.Key.EmployeeCode} on {grouped.Key.Year}-{grouped.Key.Month}-{grouped.Key.Day}");
                    }
                }

                // Save remaining records
                if (processedCount % 50 != 0)
                {
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation($"Completed processing. Total groups: {groupedRecords.Count}, Processed: {processedCount}, Inserted: {insertedCount}, Updated: {updatedCount}, Errors: {errorCount}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ProcessMachineRecord");
                throw;
            }
        }

        // Helper method to determine attendance status
        private string DetermineAttendanceStatus(string empCode, int weekDay, TimeSpan checkInTime)
        {
            try
            {
                var employee = _context.Employees.FirstOrDefault(e => e.Code == empCode);
                if (employee == null) return "Present";

                var standardHour = _context.StandardHours
                    .FirstOrDefault(s => s.GroupCode == employee.StandardHourCode && s.WeekDay == weekDay);

                if (standardHour == null || string.IsNullOrEmpty(standardHour.TimeIn))
                    return "Present";

                if (!TimeSpan.TryParseExact(standardHour.TimeIn, @"hh\:mm", CultureInfo.InvariantCulture, out TimeSpan startTime))
                    return "Present";

                var allowedTime = startTime.Add(TimeSpan.FromMinutes(30));
                return checkInTime <= allowedTime ? "Present" : "Late";
            }
            catch
            {
                return "Present";
            }
        }

        public async Task<AttendanceRecord> ProcessAttendanceRecord(AttendanceRecord record)
        {
            var today = DateTime.Today;

            AttendanceRecord existingRecord = new AttendanceRecord();
            using (var _context = new AppDbContext())
            {

                existingRecord = await _context.AttendanceRecords
                                   .FirstOrDefaultAsync(r =>
                                       r.EmployeeCode == record.EmployeeCode && r.Day == record.Day &&
                                       r.Month == record.Month &&
                                       r.Year == record.Year
                                   && r.CheckInTime.HasValue
                                   );

                var standardHour = await _context.Employees.FirstOrDefaultAsync(a => a.Code == record.EmployeeCode);

                if (record.CheckInTime != null)
                {


                    //existingRecord = await _storage.GetCheckInByDateAsync(record.EmployeeCode, record.CheckInTime.Value);

                    if (existingRecord == null)
                    {
                        record.EmployeeCode = record.EmployeeCode;
                        //existingRecord = employee.Name,
                        record.AttendanceDate = DateTime.Today;
                        record.CheckInTime = DateTime.Now;
                        record.CheckInSource = "app";
                        record.CheckInLatitude = record.CheckInLatitude;
                        record.CheckInLongitude = record.CheckInLongitude;
                        record.CheckInPhoto = "";// photoFileName ?? string.Empty,
                        record.Status = "present";
                        record.Year = DateTime.Now.Year;
                        record.Month = DateTime.Now.Month;
                        record.Day = DateTime.Now.Day;
                        record.CheckInTime = DateTime.Now;
                        record.CheckInAddress = record.CheckInAddress;
                        record.AttendanceDate = DateTime.Now.Date;
                        record.StandardHourCode = standardHour.StandardHourCode;
                        record.StandardHourWeekday = ((int)DateTime.Now.DayOfWeek + 6) % 7 + 1;
                        record.Status = CheckTime(record.EmployeeCode, ((int)DateTime.Now.DayOfWeek + 6) % 7 + 1, _graceMinutes);
                        record.CheckOutSource = "";
                        record.Fortnight = record.Day <= 15 ? 1 : 2;

                        var attendanceHistories = new AttendenceHistory
                        {
                            AttendenceHistoryId = Guid.NewGuid(),
                            EmployeeCode = record.EmployeeCode,
                            AttendanceDate = record.AttendanceDate,
                            CheckInTime = DateTime.Now,
                            CheckOutTime = null,
                            Source = record.CheckInSource,
                            Latitude = record.CheckInLatitude,
                            Longitude = record.CheckInLongitude,
                            Location = record.CheckInAddress,
                            DeviceId = record.CheckInDeviceId,
                            Photo = record.CheckInPhoto ?? null
                        };

                        await _taskService.CheckPreviousDayTaskDuration(record.EmployeeCode);

                        _context.AttendanceRecords.Add(record);
                        _context.AttendenceHistories.Add(attendanceHistories);
                        await _context.SaveChangesAsync();

                        //await _storage.AddOrUpdateAttendanceAsync(record);

                        _logger.LogInformation($"? Check-in: {record.CheckInDeviceId} at {record.CheckInTime}");

                        // Send real-time notification
                        await _notificationService.SendAttendanceNotificationAsync(
                            record.EmployeeCode,
                            "? Checked In Successfully",
                            $"You checked in at {record.CheckInTime.Value:hh:mm tt}",
                            new Dictionary<string, object>
                            {
                        { "type", "check-in" },
                        { "time", record.CheckInTime.Value.ToString("o") },
                        //{ "location", record.Location ?? "Unknown" }
                            }
                        );

                    }
                }
                else
                {
                    //existingRecord = await _storage.GetCheckOutByDateAsync(record.EmployeeCode, record.CheckOutTime.Value);
                    //existingRecord = await _context.AttendanceRecords.FirstOrDefaultAsync(r =>
                    //r.EmployeeCode == record.EmployeeCode && r.CheckOutTime.HasValue &&
                    //r.CheckOutTime.Value == record.CheckOutTime.Value);
                    existingRecord = await _context.AttendanceRecords.FirstOrDefaultAsync(r =>
                                r.EmployeeCode == record.EmployeeCode && r.Day == record.Day &&
                                       r.Month == record.Month &&
                                       r.Year == record.Year && r.CheckOutTime.HasValue);


                    var records = await _context.AttendanceRecords.FirstOrDefaultAsync(a => a.EmployeeCode == record.EmployeeCode && a.AttendanceDate == record.AttendanceDate);


                    //    var standardHour = await _context.Employees.FirstOrDefaultAsync(a => a.Code == record.EmployeeCode);
                    if (existingRecord == null)
                    {
                        try
                        {
                            TimeSpan totalHours = record.CheckOutTime.Value - records.CheckInTime.Value;
                            string totalDurationText = totalHours.ToString(@"hh\:mm");
                            records.EmployeeCode = record.EmployeeCode;
                            records.Year = DateTime.Now.Year;
                            records.Month = DateTime.Now.Month;
                            records.Day = DateTime.Now.Day;
                            //// existingRecord.StandardHour = standardHour.Code;
                            //   existingRecord.week
                            records.CheckOutTime = record.CheckOutTime ?? DateTime.Now;
                            records.CheckOutSource = record.CheckOutSource ?? "app";
                            records.CheckOutLatitude = record.CheckOutLatitude;
                            records.CheckOutLongitude = record.CheckOutLongitude;
                            records.CheckOutAddress = record.CheckOutAddress;
                            records.CheckOutPhoto = record.CheckOutPhoto;
                            record.TotalHours = totalDurationText;
                            _context.AttendanceRecords.Update(records);
                            await _context.SaveChangesAsync();

                            _logger.LogInformation($"? Check-out: {record.CheckOutDeviceId} at {record.CheckOutTime}");

                            // Send real-time notification with total hours
                            await _notificationService.SendAttendanceNotificationAsync(
                                record.EmployeeCode,
                                "? Checked Out Successfully",
                                $"You checked out at {record.CheckOutTime.Value:hh:mm tt}. Total: {record.TotalHours:F1} hours",
                                new Dictionary<string, object>
                                {
                        { "type", "check-out" },
                        { "time", record.CheckOutTime.Value.ToString("o") },
                        { "totalHours", record.TotalHours ?? "" }
                                }
                            );
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                }


                // VALIDATION: Check if attendance already exists


            }
            // Broadcast via SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveAttendanceUpdate", record);

            return record;
        }


        // ============================================
        // NOTIFY MANAGER ABOUT LATE EMPLOYEE
        // ============================================
        private async Task NotifyManagerAboutLateEmployee(AttendanceRecord record, TimeSpan lateBy)
        {
            using (var _context = new AppDbContext())
            {
                try
                {
                    var employee = await _context.Employees.FirstOrDefaultAsync(a => a.Code == record.EmployeeCode); //_storage.GetEmployeeByIdAsync(record.EmployeeCode);
                                                                                                                     //_storage.GetEmployeeByIdAsync(record.EmployeeCode);
                    if (employee == null || string.IsNullOrEmpty(employee.LeadCode))
                    {
                        _logger.LogWarning($"⚠️ No manager found for employee {record.EmployeeCode}");
                        return;
                    }
                    var manager = await _context.Employees.FirstOrDefaultAsync(a => a.Code == employee.LeadCode);
                    //   var manager = await _storage.GetEmployeeByIdAsync(employee.LeadCode);
                    if (manager == null)
                    {
                        _logger.LogWarning($"⚠️ Manager {employee.LeadCode} not found");
                        return;
                    }

                    var lateMinutes = (int)lateBy.TotalMinutes;
                    var title = "⏰ Employee Late Alert";
                    var message = $"{employee.Name} checked in late by {lateMinutes} minutes at {record.CheckInTime:hh:mm tt}";

                    // Send to manager
                    await _notificationService.SendManagerNotificationAsync(
                        employee.LeadCode,
                        title,
                        message,
                        new Dictionary<string, object>
                        {
                        { "type", "late-alert" },
                        { "employeeId", record.EmployeeCode },
                        { "employeeName", employee.Name },
                        { "department", employee.Department },
                        { "checkInTime", record.CheckInTime?.ToString("o") ?? "" },
                        { "lateBy", lateMinutes },
                        { "lateByText", $"{lateMinutes} minutes" }
                        }
                    );

                    _logger.LogInformation($"📨 Late notification sent to manager {manager.Name} about {employee.Name}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Failed to notify manager about late employee: {ex.Message}");
                }
            }
        }

        // ============================================
        // CHECK AND NOTIFY ABSENT EMPLOYEES
        // ============================================
        public async Task CheckAndNotifyAbsentEmployeesAsync()
        {
            using (var _context = new AppDbContext())
            {
                try
                {
                    _logger.LogInformation("Checking for absent employees...");

                    var cutoffTime = DateTime.Now.TimeOfDay;

                    // Don't check before 10:00 AM - give employees time to check in
                    if (cutoffTime < new TimeSpan(10, 0, 0))
                    {
                        _logger.LogInformation("Too early to check absences (before 10:00 AM)");
                        return;
                    }
                    var employees = await _context.Employees.ToListAsync();//_storage.GetEmployeesAsync();
                    var today = DateTime.Today;
                    int absentCount = 0;
                    int notifiedCount = 0;

                    foreach (var employee in employees)
                    {
                        // Skip if employee doesn't have a manager
                        if (string.IsNullOrEmpty(employee.LeadCode))
                        {
                            continue;
                        }

                        // Check if employee has checked in today
                        var record = await GetTodayAttendanceAsync(employee.Code);

                        // Check if on approved leave
                        //var leaveRequests = await _storage.GetLeaveRequestsByEmployeeAsync(employee.Code);
                        //var onLeave = leaveRequests.Any(l =>
                        //    l.Status == "approved" &&
                        //    today >= l.StartDate.Date &&
                        //    today <= l.EndDate.Date);

                        // If not checked in and not on leave, notify manager
                        if (record?.CheckInTime == null)
                        {
                            absentCount++;
                            await NotifyManagerAboutAbsentEmployee(employee);
                            notifiedCount++;
                        }
                    }

                    _logger.LogInformation($"✅ Absent check completed: {absentCount} absent employees found, {notifiedCount} managers notified");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Error checking absent employees: {ex.Message}");
                    throw; // Re-throw to be caught by AbsentCheckService
                }
            }
        }

        // ============================================
        // NOTIFY MANAGER ABOUT ABSENT EMPLOYEE
        // ============================================
        private async Task NotifyManagerAboutAbsentEmployee(Employee employee)
        {
            using (var _context = new AppDbContext())
            {
                try
                {


                    var manager = await _context.Employees.FirstOrDefaultAsync(a => a.Code == employee.Code);//_storage.GetEmployeeByIdAsync(employee.LeadCode!);
                    if (manager == null)
                    {
                        _logger.LogWarning($"⚠️ Manager {employee.LeadCode} not found for absent employee {employee.Code}");
                        return;
                    }

                    var title = "⚠️ Employee Absent Alert";
                    var message = $"{employee.Name} ({employee.Code}) has not checked in today";

                    await _notificationService.SendManagerNotificationAsync(
                        employee.LeadCode!,
                        title,
                        message,
                        new Dictionary<string, object>
                        {
                        { "type", "absent-alert" },
                        { "employeeId", employee.Code },
                        { "employeeName", employee.Name },
                        { "department", employee.Department },
                        { "designation", employee.DesignationCode },
                        { "date", DateTime.Today.ToString("yyyy-MM-dd") },
                        { "time", DateTime.Now.ToString("o") }
                        }
                    );

                    _logger.LogInformation($"📨 Absent notification sent to manager {manager.Name} about {employee.Name}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Failed to notify manager about absent employee {employee.Code}: {ex.Message}");
                }
            }
        }

        // ============================================
        // DETERMINE ATTENDANCE STATUS
        // ============================================
        //private string DetermineStatus(DateTime checkInTime ,string empCode)
        //{
        //    using (var _context = new AppDbContext())
        //    {
        //        var checkInTimeOnly = checkInTime.TimeOfDay;
        //        var empStandardCode = _context.Employees.FirstOrDefault(a => a.Code == empCode).StandardHourCode;
        //        string[] formats = { "HH\\:mm", "H\\:mm", "HH\\:mm\\:ss" };
        //        var timeInString = _context.StandardHours
        //            .Where(a => a.GroupCode == empStandardCode)
        //            .Select(a => a.TimeIn)
        //            .FirstOrDefault(); // "09:00"

        //        var a = timeInString.Length;

        //        var rawTime = timeInString?.Trim();

        //        if (!TimeSpan.TryParseExact(
        //            rawTime,
        //            formats,
        //            CultureInfo.InvariantCulture,
        //            out TimeSpan time)) //00:00:00
        //            {
        //                throw new Exception($"Invalid TimeIn value: '{timeInString}'");
        //            }

        //        var lateThreshold = time.Add(TimeSpan.FromMinutes(_graceMinutes));

        //        return checkInTimeOnly <= lateThreshold ? "present" : "late";
        //    }
        //}

        public string CheckTime(string empCode, int weekDay, int minutesToAdd = 30)
        {
            using (var _context = new AppDbContext())
            {
                var empStandardCode = _context.Employees.FirstOrDefault(a => a.Code == empCode).StandardHourCode;
                //
                var timeInString = _context.StandardHours
                    .Where(a => a.GroupCode == empStandardCode && a.WeekDay == weekDay)
                    .Select(a => a.TimeIn)
                    .FirstOrDefault();

                if (!TimeSpan.TryParseExact(timeInString, @"hh\:mm", CultureInfo.InvariantCulture, out TimeSpan inputTime))
                {
                    throw new ArgumentException("Invalid time format. Use HH:mm format.");
                }

                TimeSpan targetTime = inputTime.Add(TimeSpan.FromMinutes(minutesToAdd));
                TimeSpan currentTime = new TimeSpan(
                    DateTime.Now.Hour,
                    DateTime.Now.Minute,
                    0
                );
                //     TimeSpan currentTime = DateTime.Now.TimeOfDay;

                // Compare
                return currentTime < targetTime ? "Present" : "Late";
            }
        }


        // ============================================
        // HELPER METHOD: GET EMPLOYEE'S MANAGER
        // ============================================
        public async Task<Employee?> GetEmployeeManagerAsync(string employeeId)
        {
            try
            {
                using (var _context = new AppDbContext())
                {
                    // var employee = await _storage.GetEmployeeByIdAsync(employeeId);
                    var employee = await _context.Employees.FirstOrDefaultAsync(a => a.Code == employeeId);
                    if (employee == null || string.IsNullOrEmpty(employee.LeadCode))
                    {
                        return null;
                    }

                    return await _context.Employees.FirstOrDefaultAsync(a => a.LeadCode == employee.LeadCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error getting manager for employee {employeeId}: {ex.Message}");
                return null;
            }
        }

        // ============================================
        // HELPER METHOD: MANUAL ABSENT CHECK TRIGGER
        // ============================================
        public async Task<Dictionary<string, object>> TriggerAbsentCheckAsync()
        {
            try
            {
                await CheckAndNotifyAbsentEmployeesAsync();

                return new Dictionary<string, object>
                {
                    { "success", true },
                    { "message", "Absent check triggered successfully" },
                    { "timestamp", DateTime.Now }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Manual absent check trigger failed: {ex.Message}");
                return new Dictionary<string, object>
                {
                    { "success", false },
                    { "error", ex.Message },
                    { "timestamp", DateTime.Now }
                };
            }
        }

        public async Task<AttendanceRecord?> UpdateCheckIn(AttendanceRecord attendanceRecord)
        {
            using (var context = new AppDbContext())
            {
                try
                {
                    var updateCheckIn = await context.AttendanceRecords.FirstOrDefaultAsync(c => c.EmployeeCode == attendanceRecord.EmployeeCode &&
                                        c.Year == attendanceRecord.Year && c.Month == attendanceRecord.Month && c.Day == attendanceRecord.Day);
                    if (updateCheckIn != null)
                    {
                        updateCheckIn.CheckInTime = attendanceRecord.CheckInTime;
                        updateCheckIn.CheckInLongitude = attendanceRecord.CheckInLongitude;
                        updateCheckIn.CheckInLatitude = attendanceRecord.CheckInLatitude;
                        updateCheckIn.CheckInAddress = attendanceRecord.CheckInAddress;

                        var attendanceHistories = new AttendenceHistory
                        {
                            AttendenceHistoryId = Guid.NewGuid(),
                            EmployeeCode = attendanceRecord.EmployeeCode,
                            AttendanceDate = attendanceRecord.AttendanceDate,
                            CheckInTime = DateTime.Now,
                            CheckOutTime = null,
                            Source = attendanceRecord.CheckInSource,
                            Latitude = attendanceRecord.CheckInLatitude,
                            Longitude = attendanceRecord.CheckInLongitude,
                            Location = attendanceRecord.CheckInAddress,
                            DeviceId = attendanceRecord.CheckInDeviceId,
                            Photo = attendanceRecord.CheckInPhoto ?? null
                        };

                        context.AttendenceHistories.Add(attendanceHistories);
                        context.AttendanceRecords.Update(updateCheckIn);
                        await context.SaveChangesAsync();
                    }
                    return updateCheckIn;
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }

        public async Task<AttendanceRecord?> UpdateCheckOut(AttendanceRecord attendanceRecord)
        {
            using (var context = new AppDbContext())
            {
                try
                {
                    var updateCheckOut = await context.AttendanceRecords.FirstOrDefaultAsync(c => c.EmployeeCode == attendanceRecord.EmployeeCode &&
                                        c.Year == attendanceRecord.Year && c.Month == attendanceRecord.Month && c.Day == attendanceRecord.Day);
                    if (updateCheckOut != null)
                    {
                        updateCheckOut.CheckOutTime = attendanceRecord.CheckOutTime;
                        updateCheckOut.CheckOutSource = attendanceRecord.CheckOutSource;
                        updateCheckOut.CheckOutLongitude = attendanceRecord.CheckOutLongitude;
                        updateCheckOut.CheckOutLatitude = attendanceRecord.CheckOutLatitude;
                        updateCheckOut.CheckOutAddress = attendanceRecord.CheckOutAddress;


                        var attendanceHistories = new AttendenceHistory
                        {
                            AttendenceHistoryId = Guid.NewGuid(),
                            EmployeeCode = attendanceRecord.EmployeeCode,
                            AttendanceDate = attendanceRecord.AttendanceDate,
                            CheckInTime = null,
                            CheckOutTime = DateTime.Now,
                            Source = attendanceRecord.CheckOutSource,
                            Latitude = attendanceRecord.CheckOutLatitude,
                            Longitude = attendanceRecord.CheckOutLongitude,
                            Location = attendanceRecord.CheckOutAddress,
                            DeviceId = attendanceRecord.CheckOutDeviceId,
                            Photo = attendanceRecord.CheckOutPhoto ?? null
                        };

                        context.AttendenceHistories.Add(attendanceHistories);
                        context.AttendanceRecords.Update(updateCheckOut);
                        await context.SaveChangesAsync();
                    }
                    return updateCheckOut;
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }

        public async Task<DayWise> DayWiseStandardHour(string empCode, DateTime dateTime)

        {
            using (var context = new AppDbContext())
            {
                try
                {
                    var result = new DayWise();

                    var employee = await context.Employees.FirstOrDefaultAsync(c => c.Code == empCode);

                    if (employee == null)

                        return result;

                    DayOfWeek dayOfWeek = dateTime.DayOfWeek;

                    int weekDayNumber = dayOfWeek == DayOfWeek.Sunday ? 7 : (int)dayOfWeek;

                    var getWeekDay = await context.StandardHours.FirstOrDefaultAsync(c => c.GroupCode == employee.StandardHourCode && c.WeekDay == weekDayNumber);

                    var attendance = await context.AttendanceRecords.FirstOrDefaultAsync(c => c.EmployeeCode == empCode && c.AttendanceDate == dateTime.Date);

                    var checkIn = attendance?.CheckInTime?.ToString("HH:mm") ?? "00:00";

                    var checkOut = attendance?.CheckOutTime?.ToString("HH:mm") ?? "00:00";

                    // ----- Standard hours display -----

                    result.StandardHours = FormatTimeToHoursMinutes(getWeekDay.Hours);

                    // ----- Working hours -----

                    result.WorkingHours = CalculateWorkingHour(checkIn, checkOut);

                    // ----- Overtime -----

                    result.OverTime = CheckOverTime(getWeekDay.Hours, checkIn, checkOut);

                    // ----- Late Logic -----

                    if (getWeekDay == null || string.IsNullOrEmpty(getWeekDay.TimeIn))

                    {

                        result.IsLate = false;

                    }

                    else

                    {

                        var standardTimeIn = TimeSpan.Parse(getWeekDay.TimeIn);

                        var allowedTime = standardTimeIn.Add(TimeSpan.FromMinutes(30)); // grace period

                        if (TimeSpan.TryParse(checkIn, out TimeSpan arrivalTime))

                        {

                            result.IsLate = arrivalTime > allowedTime;

                        }

                        else

                        {

                            result.IsLate = false;

                        }

                    }

                    return result;

                }

                catch (Exception ex)

                {

                    throw;

                }

            }

        }

        private string FormatTimeToHoursMinutes(string time)

        {

            TimeSpan ts = TimeSpan.Parse(time);

            string formatted = $"{ts.Hours}h {ts.Minutes}m";

            return formatted;

        }

        private string CalculateWorkingHour(string checkIn, string checkOut)

        {

            if (!TimeSpan.TryParse(checkIn, out TimeSpan inTime) ||

                !TimeSpan.TryParse(checkOut, out TimeSpan outTime))

            {

                return "0h 0m";

            }

            TimeSpan difference = outTime - inTime;

            if (difference.TotalMinutes < 0)

            {

                return "0h 0m"; // prevents negative duration

            }

            return $"{difference.Hours}h {difference.Minutes}m";

        }


        private string CheckOverTime(string standardHours, string checkIn, string checkOut)

        {

            if (!TimeSpan.TryParse(standardHours, out TimeSpan standardTime) ||

                !TimeSpan.TryParse(checkIn, out TimeSpan inTime) ||

                !TimeSpan.TryParse(checkOut, out TimeSpan outTime))

            {

                return "0h 0m";

            }

            TimeSpan workedTime = outTime - inTime;

            if (workedTime.TotalMinutes <= standardTime.TotalMinutes)

            {

                return "0h 0m"; // No overtime

            }

            TimeSpan overtime = workedTime - standardTime;

            return $"{overtime.Hours}h {overtime.Minutes}m";

        }

        public async Task<ChartDataResponse> ChartRecords(DateTime dateTime, string empCode, DateTime endDate)
        {
            try
            {
                //DateTime startDate = dateTime.Date;
                //DateTime endDate = endDate.Date;
                var data = new ChartDataResponse();
                //if (type == "weekly")
                //{
                //    DateTime inputDate = dateTime.Date;

                //    // Monday = 1, Sunday = 7
                //    int diff = (7 + (inputDate.DayOfWeek - DayOfWeek.Monday)) % 7;

                //    startDate = inputDate.AddDays(-diff);
                //    endDate = startDate.AddDays(7).AddSeconds(-1);
                //}
                //else if (type == "monthly")
                //{
                //    startDate = new DateTime(dateTime.Year, dateTime.Month, 1);
                //    int daysInMonth = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
                //    DateTime lastDay = new DateTime(dateTime.Year, dateTime.Month, daysInMonth);
                //    endDate = lastDay;
                //}
                //else if (type == "yearly")
                //{
                //    startDate = new DateTime(dateTime.Year, 1, 1);
                //    endDate = new DateTime(dateTime.Year, 12, 31);
                //}

                data = await GenerateChart(dateTime.Date, endDate, empCode);
                //data.Type = type;
                return data;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private async Task<ChartDataResponse> GenerateChart(DateTime startDate, DateTime endDate, string empCode)
        {
            using (var context = new AppDbContext())
            {
                try
                {
                    var data = new ChartDataResponse();
                    var employee = await context.Employees.FirstOrDefaultAsync(c => c.Code == empCode);

                    var attandanceRecord = await context.AttendanceRecords.Where(c => c.EmployeeCode == empCode &&
                                                    c.AttendanceDate >= startDate && c.AttendanceDate <= endDate).OrderBy(c => c.AttendanceDate).ToListAsync();
                    int onTime = 1;
                    int late = 1;
                    foreach (var item in attandanceRecord)
                    {
                        var checkIn = item?.CheckInTime?.ToString("HH:mm") ?? "00:00";
                        if (checkIn == "00:00")
                        {
                            continue;
                        }
                        DayOfWeek dayOfWeek = item.AttendanceDate.Date.DayOfWeek;
                        int weekDayNumber = dayOfWeek == DayOfWeek.Sunday ? 7 : (int)dayOfWeek;

                        var getWeekDay = await context.StandardHours.FirstOrDefaultAsync(c => c.GroupCode == employee.StandardHourCode &&
                                                c.WeekDay == weekDayNumber);
                        if (getWeekDay == null || string.IsNullOrEmpty(getWeekDay.TimeIn))
                        {
                            continue;
                        }
                        else
                        {
                            var standardTimeIn = TimeSpan.Parse(getWeekDay.TimeIn);
                            var allowedTime = standardTimeIn.Add(TimeSpan.FromMinutes(30)); // grace period
                            if (TimeSpan.TryParse(checkIn, out TimeSpan arrivalTime))
                            {
                                if (arrivalTime > allowedTime)
                                {
                                    data.Late = late++;
                                }
                                else
                                {
                                    data.OnTime = onTime++;
                                }
                            }

                        }
                    }

                    return data;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

    }
}