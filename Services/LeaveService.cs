using AttendanceAPI.Contracts.Request;
using AttendanceAPI.Contracts.Response;
using AttendanceAPI.Data;
using AttendanceAPI.Entities;
using AttendanceAPI.Interface;
using Azure.Core;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2016.Excel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using OpenCvSharp;
using Serilog;
using System.Globalization;
using System.Threading.Tasks;
using static OpenCvSharp.Stitcher;

namespace AttendanceAPI.Services
{
    public class LeaveService : ILeaveService
    {
        //private readonly ILogger<LeaveService> _logger;
        private readonly NotificationService _notificationService;
        private readonly AppDbContext _context;
        private readonly IServiceProvider _serviceProvider;
        public LeaveService(AppDbContext appContext, IServiceProvider serviceProvider, NotificationService notificationService)
        {
            //_logger = logger;
            _context = appContext;
            _serviceProvider = serviceProvider;
            _notificationService = notificationService;
        }

        public async Task<LeaveRequestResponse> Applyleaves(ApplyLeaveRequest request)
        {
            using (var context = new AppDbContext())
            {
                try
                {
                    //var IsCancelledLeaved = await context.LeaveRequisitionDetails
                    //    .Where(a => a.FromDate >= request.StartDate &&
                    //                a.ToDate <= request.EndDate)
                    //    .Select(v => v.MasterId)
                    //    .ToListAsync();

                    //var masterLeave = await context.LeaveRequisitionMaster
                    //    .Where(a => IsCancelledLeaved.Contains(a.Id) && a.Status == "C" && a.EmployeeCode == request.EmployeeId)
                    //    .ToListAsync();


                    bool isleaveRequestExist = context.LeaveRequisitionMaster.Any(a => a.EmployeeCode == request.EmployeeId && a.Status == "S");
                    if(isleaveRequestExist)
                    {
                        throw new Exception("cannot request more than one leave");
                    }

                    var employee = await context.Employees.FirstOrDefaultAsync(c => c.Code == request.EmployeeId);

                    if (employee == null)
                    {
                        throw new Exception("Employee Not Found");
                    }

                    var manager = await context.Employees.FirstOrDefaultAsync(a => a.Code == employee.LeadCode);
                    if (manager == null)
                        throw new Exception("Manager not found");

                    if (request.StartDate > request.EndDate)
                    {
                        throw new Exception("End date must be after start date");
                    }
                    //bool isExist = await CheckIfHoliday(request.StartDate.Date, request.EndDate.Date, context, request.EmployeeId);
                    //if (isExist)
                    //{
                    //    throw new Exception("Leave cannot be applied on holidays or non-working days.");
                    //}
                    var masterMaxKey = context.LeaveRequisitionMaster.Max(a => (int?)a.Id) == null ? 1 : _context.LeaveRequisitionMaster.Max(c => c.Id) + 1;
                    var detailMaxKey = context.LeaveRequisitionDetails.Max(a => (int?)a.Id) == null ? 1 : _context.LeaveRequisitionDetails.Max(a => a.Id) + 1;
                    var masterDisplayMaxKey = getMaxLeaveMasterDisplayId(request.EmployeeId, _context);

                    var masterRecord = new LeaveRequisitionMaster
                    {
                        Id = masterMaxKey,
                        DisplayId = masterDisplayMaxKey,
                        EmployeeCode = employee.Code,
                        Status = "S",
                        SubmissionDate = DateTime.Today,
                        ApproverEmployeeCode = employee.LeadCode
                    };
                    await context.LeaveRequisitionMaster.AddAsync(masterRecord);
                    var leaves = context.AttendanceLeaves.Where(a => a.EmployeeCode == request.EmployeeId && a.Year == DateTime.Now.Year).ToList();

                    var listDates = new List<DateTime>();
                    for (var dt = Convert.ToDateTime(request.StartDate); dt <= Convert.ToDateTime(request.EndDate); dt = dt.AddDays(1))
                    {
                        listDates.Add(dt);
                    }
                    listDates = await FilterWorkingDays(listDates, context, request.EmployeeId);

                    ValidateIfCheckInOutExist(listDates, context, request.EmployeeId);

                    bool exist = LeaveExist(listDates, context, leaves, request.EmployeeId, request.LeaveType);
                    //ValidateIfCheckInOutExist(listDates, context, request.EmployeeId);
                    //bool exist = LeaveExist(listDates, context, leaves, request.EmployeeId, request.LeaveType);
                    if (exist) { throw new Exception("Leave Already Exist"); }

                    decimal totalDays = GetTotalDays(listDates, context, employee.Code ,request.LeaveType);

                    var detailRecord = new LeaveRequisitionDetail
                    {
                        Id = detailMaxKey,
                        DisplayId = "AL-1",
                        MasterId = masterRecord.Id,
                        FromDate = request.StartDate.Date,
                        ToDate = request.EndDate.Date,
                        LeaveTypeCode = request.LeaveType,
                        NoOfDays = totalDays,
                        Purpose = request.Reason,
                        AdjustmentType = null,
                        PurposeCode = 1,
                        RejectionReason = null
                    };

                    await context.LeaveRequisitionDetails.AddAsync(detailRecord);

                    await context.SaveChangesAsync();
                    Log.Information("Your {0} leave request for {1} day(s) has been sent to {2} for approval", request.LeaveType, totalDays, manager.Name);

                    // Notify employee
                    await _notificationService.SendTaskNotificationAsync(
                        employee.Code,
                        "Leave Application Sent",
                        $"Your leave application sent to {manager.Name}",
                        context,
                        new Dictionary<string, object>
                        {
                            { "leaveMasterId", masterRecord.Id }
                        }
                    );

                    // Notify team lead
                    await _notificationService.SendFcmNotificationAsync(manager.Code, manager.FcmToken,
                            "Request for leave", $"{employee.Name} requested for {request.LeaveType} leave");

                    var leaveRequest = new LeaveRequestResponse
                    {
                        Status = masterRecord.Status,
                        EmployeeId = masterRecord.EmployeeCode,
                        ApproverManagerId = masterRecord.ApproverEmployeeCode,
                        LeaveType = detailRecord.LeaveTypeCode,
                        StartDate = detailRecord.FromDate,
                        EndDate = detailRecord.ToDate,
                        TotalDays = totalDays,
                        Reason = request.Reason,
                        EmployeeName = employee.Name,
                        ApproverManagerName = manager.Name,
                    };

                    return leaveRequest;

                }
                catch (Exception e)
                {
                    Log.Error(e, "Unhandled Exception");
                    throw new Exception(e.Message);
                }
            }
        }
        private async Task<List<DateTime>> FilterWorkingDays(
    List<DateTime> listDates,
    AppDbContext context,
    string empCode)
        {
            var validDates = new List<DateTime>();

            var employee = await context.Employees.FirstOrDefaultAsync(e => e.Code == empCode);

            foreach (var date in listDates)
            {
                DayOfWeek dayOfWeek = date.DayOfWeek;
                int weekDayNumber = dayOfWeek == DayOfWeek.Sunday ? 7 : (int)dayOfWeek;

                var weekDay = context.StandardHours
                    .FirstOrDefault(c => c.GroupCode == employee.StandardHourCode &&
                                         c.WeekDay == weekDayNumber);

                bool isWeekend = weekDay != null && weekDay.Minutes == 0;

                bool holidays = context.Holiday
                    .Any(c => c.Day == date.Day &&
                                c.Month == date.Month &&
                                c.Year == date.Year);

                //bool isHoliday = getHolidayDetail(holidays, date);

                if (!isWeekend && !holidays)
                {
                    validDates.Add(date);
                }
            }

            return validDates;
        }
        private async Task<bool> CheckIfHoliday(DateTime startDate, DateTime endDate, AppDbContext context, string EmpCode)
        {
            bool isHolidayExist = false;
            var listDates = RangeDates(startDate, endDate);
            foreach (var a in listDates)
            {
                var getStandardHour = await context.Employees.FirstOrDefaultAsync(c => c.Code == EmpCode);
                DayOfWeek dayOfWeek = a.DayOfWeek;
                int weekDayNumber = dayOfWeek == DayOfWeek.Sunday ? 7 : (int)dayOfWeek;
                var getWeekDay = context.StandardHours.FirstOrDefault(c => c.GroupCode == getStandardHour.StandardHourCode && c.WeekDay == weekDayNumber);
                var getHoliday = context.Holiday.Where(c => c.Day == a.Day && c.Month == a.Month && c.Year == a.Year).ToList();
                var checkHolidayExist = getHolidayDetail(getHoliday, a);

                if (checkHolidayExist && getWeekDay != null && (getWeekDay.Minutes == 0))
                {
                    isHolidayExist = true;
                }
            }
            return isHolidayExist;

        }
        private void ValidateIfCheckInOutExist(List<DateTime> dateTimes, AppDbContext context, string EmployeeId)
        {
            foreach (var item in dateTimes)
            {
                var attandanceRecorc =  context.AttendanceRecords.FirstOrDefault(c => c.EmployeeCode == EmployeeId && c.Year == item.Year &&
                                        c.Month == item.Month && c.Day == item.Day);

                if (attandanceRecorc != null && (attandanceRecorc.CheckInTime != null || attandanceRecorc.CheckOutTime != null))
                {
                    throw new Exception($"You cannot Apply Leave because your check In/Out exist on {item.Date.ToString("dd-MM-yyyy")}");
                }
            }
        }
        public async Task<LeaveRequestResponse> GetApproveLeaveByIdAsync(int Id, ApproveLeaveRequest request)
        {
            using (var context = new AppDbContext())
            {
                try
                {
                    var getLeaveMaster = await context.LeaveRequisitionMaster.FirstOrDefaultAsync(c => c.Id == Id && c.ApproverEmployeeCode == request.ApproverId);
                    if (getLeaveMaster == null)
                    {
                        throw new Exception("Leave Request Not Found");
                    }
                    if (getLeaveMaster.ApproverEmployeeCode != request.ApproverId)
                    {
                        throw new Exception("You are not authorized to approve this request");
                    }
                    var getLeaveDetail = await context.LeaveRequisitionDetails.FirstOrDefaultAsync(c => c.MasterId == Id);

                    var conLeaveApprove = new LeaveApproveResponse();

                    var listDates = new List<DateTime>();
                    for (var dt = getLeaveDetail.FromDate; dt <= getLeaveDetail.ToDate; dt = dt.AddDays(1))
                    {
                        listDates.Add(dt);
                    }
                    conLeaveApprove.EmployeeCode = getLeaveMaster.EmployeeCode;
                    conLeaveApprove.Id = getLeaveMaster.Id;
                    conLeaveApprove.FromDate = getLeaveDetail.FromDate;
                    conLeaveApprove.ToDate = getLeaveDetail.ToDate;
                    conLeaveApprove.LeaveType = getLeaveDetail.LeaveTypeCode;
                    conLeaveApprove.AdjustmentType = getLeaveDetail.AdjustmentType;
                    await AddApproveLeaves(listDates, conLeaveApprove, context);
                    getLeaveDetail.NoOfDays = conLeaveApprove.NoOfDays;
                    if (getLeaveMaster != null)
                    {
                        getLeaveMaster.ApprovedDate = DateTime.Today;
                        getLeaveMaster.Status = "A";
                        getLeaveMaster.RejectionDate = null;
                    }

                    await context.SaveChangesAsync();
                    Log.Information("leaves has been approved by {0}", getLeaveMaster.EmployeeCode);

                    // Notify employee
                    await _notificationService.SendTaskNotificationAsync(
                        getLeaveMaster.ApproverEmployeeCode,
                        "Leave Approved",
                        $"You approved {getLeaveMaster.DisplayId} of employee: {getLeaveMaster.EmployeeCode}",
                        context,
                        new Dictionary<string, object>
                        {
                            { "leaveMasterId", getLeaveMaster.Id }
                        }
                    );

                    var empInfo = await context.Employees.Where(c => c.Code == getLeaveMaster.EmployeeCode).Select(c => new { c.Code, c.FcmToken }).FirstOrDefaultAsync();

                    // Notify team lead
                    await _notificationService.SendFcmNotificationAsync(empInfo.Code, empInfo.FcmToken,
                            "Leave Approved", $"Your {getLeaveDetail.Purpose} has been approved");

                    var leaveRequest = new LeaveRequestResponse
                    {
                        Status = getLeaveMaster.Status,
                        EmployeeId = getLeaveMaster.EmployeeCode,
                        ApproverManagerId = getLeaveMaster.ApproverEmployeeCode,
                        LeaveType = getLeaveDetail.LeaveTypeCode,
                        StartDate = getLeaveDetail.FromDate,
                        EndDate = getLeaveDetail.ToDate,
                        TotalDays = conLeaveApprove.NoOfDays,
                        Reason = getLeaveDetail.Purpose,
                        //EmployeeName = employee.Name,
                        //ApproverManagerName = manager.Name,
                    };

                    return leaveRequest;
                }
                catch (Exception ex)
                {
                    Log.Information(ex.Message);
                    throw ex;
                }

            }
        }

        public async Task<Employee> GetEmployeeByIdAsync(string Id)
        {
            using (var context = new AppDbContext())
            {
                var getEmployee = await context.Employees.FirstOrDefaultAsync(c => c.Code == Id);
                return getEmployee;

            }
        }

        public async Task<LeaveRejectResponse> GetLeaveRequestForRejectionByIdAsync(int Id, RejectLeaveRequest request)
        {
            using (var context = new AppDbContext())
            {
                try
                {
                    var getLeave = await context.LeaveRequisitionMaster.FirstOrDefaultAsync(c => c.Id == Id && c.ApproverEmployeeCode == request.ApproverId);
                    if (getLeave == null)
                    {
                        throw new Exception("Leave request not found");
                    }
                    if (getLeave.ApproverEmployeeCode != request.ApproverId)
                    {
                        throw new Exception("You are not authorized to reject this request");
                    }
                    getLeave.RejectionDate = DateTime.Today;
                    getLeave.Status = "R";

                    var getdeatilLeave = await context.LeaveRequisitionDetails.FirstOrDefaultAsync(c => c.MasterId == Id);
                    getdeatilLeave.RejectionReason = request.Reason;

                    await context.SaveChangesAsync();
                    Log.Information("Your {0} leave has been rejected by {1}. Reason {2}", getdeatilLeave.PurposeCode, getLeave.ApproverEmployeeCode, getdeatilLeave.RejectionReason);

                    // Notify employee
                    await _notificationService.SendTaskNotificationAsync(
                        getLeave.ApproverEmployeeCode,
                        "Leave Rejected",
                        $"You rejected {getLeave.DisplayId} of employee: {getLeave.EmployeeCode}",
                        context,
                        new Dictionary<string, object>
                        {
                            { "leaveMasterId", getLeave.Id }
                        }
                    );

                    var empInfo = await context.Employees.Where(c => c.Code == getLeave.EmployeeCode).Select(c => new { c.Code, c.FcmToken }).FirstOrDefaultAsync();

                    // Notify team lead
                    await _notificationService.SendFcmNotificationAsync(empInfo.Code, empInfo.FcmToken,
                            "Leave Rejected", $"Your {getdeatilLeave.Purpose} has been rejected");

                    return new LeaveRejectResponse
                    {
                        leaveType = getdeatilLeave.LeaveTypeCode,
                        EmployeeCode = getLeave.EmployeeCode,
                    };
                }
                catch (Exception ex)
                {
                    Log.Information(ex.Message);
                    throw ex;
                }
            }
        }
        public async Task<IList<LeaveRequestResponse>> GetPendingLeaveRequestsForApproverAsync(string managerId)
        {
            using (var context = new AppDbContext())
            {
                var leaveRequest = await (from a in context.LeaveRequisitionMaster
                                          join b in context.LeaveRequisitionDetails on a.Id equals b.MasterId
                                          join c in context.Employees on a.EmployeeCode equals c.Code
                                          join manager in context.Employees on a.ApproverEmployeeCode equals manager.Code
                                          where a.ApproverEmployeeCode == managerId && a.Status == "S"
                                          select new LeaveRequestResponse
                                          {
                                              Id = a.Id,
                                              Status = a.Status,
                                              EmployeeId = a.EmployeeCode,
                                              ApproverManagerId = a.ApproverEmployeeCode,
                                              LeaveType = b.LeaveTypeCode,
                                              StartDate = b.FromDate,
                                              EndDate = b.ToDate,
                                              TotalDays = b.NoOfDays,
                                              Reason = b.Purpose,
                                              RejectionReason = b.RejectionReason,
                                              EmployeeName = c.Name,
                                              ApproverManagerName = manager.Name,
                                              ApprovedAt = a.ApprovedDate,
                                              CreatedAt = a.SubmissionDate
                                          }).ToListAsync();

                return leaveRequest;
            }
        }

        private string getMaxLeaveMasterDisplayId(string employeeCode, AppDbContext context)
        {
            string maxMasterDisplayId = "";
            var employeeLeaveMasterCount = context.LeaveRequisitionMaster.Count(c => c.EmployeeCode == employeeCode);
            if (employeeLeaveMasterCount != null && employeeLeaveMasterCount > 0)
            {
                var nextCount = employeeLeaveMasterCount + 1;
                maxMasterDisplayId = string.Format("LRF-{0}", nextCount.ToString().PadLeft(3, '0'));
            }
            else
            {
                var nextCount = 1;
                maxMasterDisplayId = string.Format("LRF-{0}", nextCount.ToString().PadLeft(3, '0'));
            }

            return maxMasterDisplayId;
        }

        private async Task AddApproveLeaves(List<DateTime> listDates, LeaveApproveResponse conLeaveapprove, AppDbContext context)
        {
            string EmployeeCode = "";
            DateTime fromDate = new DateTime();
            DateTime toDate = new DateTime();
            string leaveType = "";
            int RequisitionNo = 0;

            EmployeeCode = conLeaveapprove.EmployeeCode;
            fromDate = conLeaveapprove.FromDate;
            toDate = conLeaveapprove.ToDate;
            leaveType = conLeaveapprove.LeaveType;
            RequisitionNo = conLeaveapprove.Id;

            int day = 0;
            foreach (var a in listDates)
            {
                var getStandardHour = await context.Employees.FirstOrDefaultAsync(c => c.Code == EmployeeCode);
                DayOfWeek dayOfWeek = a.DayOfWeek;
                int weekDayNumber = dayOfWeek == DayOfWeek.Sunday ? 7 : (int)dayOfWeek;
                var getWeekDay = context.StandardHours.FirstOrDefault(c => c.GroupCode == getStandardHour.StandardHourCode && c.WeekDay == weekDayNumber);
                var getHoliday = context.Holiday.Where(c => c.Day == a.Day && c.Month == a.Month && c.Year == a.Year).ToList();
                var checkHolidayExist = getHolidayDetail(getHoliday, a);

                if (checkHolidayExist && getWeekDay != null && (getWeekDay.Minutes > 0))
                {
                    var checkDay = await context.AttendanceRecords.FirstOrDefaultAsync(c => c.EmployeeCode == EmployeeCode && c.Year == a.Year && c.Month == a.Month && c.Day == a.Day);
                    if (checkDay == null)
                    {
                        await AddLeaveRecord(EmployeeCode, a);

                    }
                    context.AttendanceLeaves.Add(new AttendanceLeave
                    {
                        EmployeeCode = EmployeeCode,
                        Year = a.Year,
                        Month = a.Month,
                        Day = a.Day,
                        Fortnight = Fortnight(a),
                        Date = a.Date,
                        TypeCode = leaveType,
                        Hours = getWeekDay.Hours,
                        Minutes = getWeekDay.Minutes,
                        RequisitionNumber = RequisitionNo
                    });
                    day++;
                    conLeaveapprove.NoOfDays = day;
                }

            }

        }
        private async Task AddLeaveRecord(string EmployeeCode, DateTime date)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var leaveday = new AttendanceRecord()
                {
                    EmployeeCode = EmployeeCode,
                    Day = date.Day,
                    Month = date.Month,
                    Year = date.Year,
                    Fortnight = Fortnight(date.Date),
                    AttendanceDate = date.Date,
                };
                var attendancerecord = scope.ServiceProvider.GetRequiredService<AttendanceService>();
                await attendancerecord.AddLeaveDay(leaveday);
            }
        }
        private int Fortnight(DateTime date)
        {
            return date.Day >= 16 ? 2 : 1;
        }
        private bool LeaveExist(List<DateTime> listDates, AppDbContext context, List<AttendanceLeave> leaves, string employeeCode, string leaveType)
        {
            bool exist = false;
            foreach (var date in listDates)
            {
                exist = leaves.Exists(a => a.EmployeeCode == employeeCode && a.Date == date.Date);
                if (exist) return exist;
            }
            return exist;
        }
        private bool getHolidayDetail(List<Holiday> holidayList, DateTime a)
        {
            var result = true;
            var test = holidayList.Where(c => c.Day == a.Day && c.Year == a.Year && c.Month == a.Month).ToList();
            if (test.Any())
            {
                result = false;
            }
            return result;
        }

        public async Task<IList<LeaveRequestResponse>> GetLeaveRequestsByEmployeeAsync(string approverId)
        {
            using (var context = new AppDbContext())
            {
                try
                {
                    var leaveRequest = await (from a in context.LeaveRequisitionMaster
                                              join b in context.LeaveRequisitionDetails on a.Id equals b.MasterId
                                              join c in context.Employees on a.EmployeeCode equals c.Code
                                              join manager in context.Employees on a.ApproverEmployeeCode equals manager.Code
                                              where a.ApproverEmployeeCode == approverId
                                              select new LeaveRequestResponse
                                              {
                                                  Status = a.Status,
                                                  EmployeeId = a.EmployeeCode,
                                                  ApproverManagerId = a.ApproverEmployeeCode,
                                                  LeaveType = b.LeaveTypeCode,
                                                  StartDate = b.FromDate,
                                                  EndDate = b.ToDate,
                                                  TotalDays = b.NoOfDays,
                                                  Reason = b.Purpose,
                                                  RejectionReason = b.RejectionReason,
                                                  EmployeeName = c.Name,
                                                  ApproverManagerName = manager.Name,
                                                  ApprovedAt = a.ApprovedDate

                                              }).ToListAsync();

                    return leaveRequest;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        private List<DateTime> GetValidLeaveDates(List<DateTime> dateTimes, AppDbContext context, string employeeCode , string leaveType)
        {
            var validDates = new List<DateTime>();

            var employee = context.Employees.FirstOrDefault(c => c.Code == employeeCode);
            var standardHourCode = employee.StandardHourCode;

            foreach (var a in dateTimes)
            {
                DayOfWeek dayOfWeek = a.DayOfWeek;
                int weekDayNumber = dayOfWeek == DayOfWeek.Sunday ? 7 : (int)dayOfWeek;

                var getWeekDay = context.StandardHours
                    .FirstOrDefault(c => c.GroupCode == standardHourCode && c.WeekDay == weekDayNumber);

                var getHoliday = context.Holiday
                    .Where(c => c.Day == a.Day && c.Month == a.Month && c.Year == a.Year)
                    .ToList();

                var checkHolidayExist = getHolidayDetail(getHoliday, a);

                if (checkHolidayExist && getWeekDay != null && getWeekDay.Minutes > 0)
                {
                    validDates.Add(a);
                }
            }

            return validDates;
        }
        private decimal GetTotalDays(List<DateTime> dateTimes, AppDbContext context, string EmployeeCode , string leaveType)
        {
            decimal day = 0;

            foreach (var a in dateTimes)
            {
                var getStandardHour = context.Employees.FirstOrDefault(c => c.Code == EmployeeCode).StandardHourCode;
                DayOfWeek dayOfWeek = a.DayOfWeek;
                int weekDayNumber = dayOfWeek == DayOfWeek.Sunday ? 7 : (int)dayOfWeek;
                var getWeekDay = context.StandardHours.FirstOrDefault(c => c.GroupCode == getStandardHour && c.WeekDay == weekDayNumber);
                var getHoliday = context.Holiday.Where(c => c.Day == a.Day && c.Month == a.Month && c.Year == a.Year).ToList();
                var checkHolidayExist = getHolidayDetail(getHoliday, a);

                if (checkHolidayExist && getWeekDay != null && (getWeekDay.Minutes > 0))
                {
                    if (leaveType == "HD") // Half Day
                        day += 0.5m;
                    else
                        day += 1;
                }
            }
            return day;
        }
        public async Task<IList<LeaveRequestResponse>> GetLeaveRequestByIdAsync(string employeeId)
        {
            using (var context = new AppDbContext())
            {
                var leaveRequests = await (from a in context.LeaveRequisitionMaster
                                           join b in context.LeaveRequisitionDetails on a.Id equals b.MasterId
                                           join l in context.LeaveTypes on b.LeaveTypeCode equals l.Code
                                           join c in context.Employees on a.EmployeeCode equals c.Code
                                           join manager in context.Employees on a.ApproverEmployeeCode equals manager.Code
                                           where a.EmployeeCode == employeeId
                                           select new
                                           {
                                               EmployeeCode = a.EmployeeCode,
                                               EmployeeName = c.Name,
                                               ApprovedDate = a.ApprovedDate,
                                               Status =  a.Status,
                                               SubmissionDate = a.SubmissionDate,
                                               Id = a.Id,
                                               FromDate = b.FromDate,
                                               ToDate = b.ToDate,
                                               Purpose = b.Purpose,
                                               RejectionReason = b.RejectionReason,
                                               LeaveTitle = l.Title, 
                                               LeaveType = b.LeaveTypeCode,

                                               ManagerName = manager.Name
                                           }).ToListAsync();

                var responseList = new List<LeaveRequestResponse>();

                foreach (var item in leaveRequests)
                {
                    // 1️⃣ Generate full date range
                    var fullRange = Enumerable.Range(0, (item.ToDate.Date - item.FromDate.Date).Days + 1)
                        .Select(d => item.FromDate.Date.AddDays(d))
                        .ToList();
                     fullRange = GetValidLeaveDates(fullRange, context, employeeId , item.LeaveType);
    //                var fullRange = await context.AttendanceLeavesCK GET lEAVES
    //.Where(a => a.RequisitionNumber == item.Id)
    //.Select(a => a.Date.Date)
    //.ToListAsync();
                    // 2️⃣ Get approved cancelled dates for this leave
                    var cancelledDates = await context.CancelLeaves
                        .Where(c =>
                            c.LeaveId == item.Id &&
                            c.EmployeeCode == employeeId &&
                            c.State != "R"
                            )
                        .Select(c => c.LeaveDate.Date)
                        .ToListAsync();

                    // 3️⃣ Remove cancelled dates from full range
                    var remainingDates = fullRange
                        .Where(d => !cancelledDates.Contains(d))
                        .ToList();

                    //int adjustedTotalDays = remainingDates.Count;
                    decimal adjustedTotalDays = item.LeaveType == "HD"
                        ? remainingDates.Count * 0.5m
                        : remainingDates.Count;

                    // 4️⃣ If all days cancelled → optionally skip OR mark as fully cancelled
                    string finalStatus =
                        adjustedTotalDays == 0
                        ? "cancelled"
                        : item.Status == "S" ? "submitted"
                        : item.Status == "R" ? "rejected"
                        : item.Status == "A" ? "approved"
                        : "unknown";

                    responseList.Add(new LeaveRequestResponse
                    {
                        Id = item.Id,
                        Status = finalStatus,
                        EmployeeId = item.EmployeeCode,
                        LeaveType = item.LeaveTitle,
                        StartDate = remainingDates.Any() ? remainingDates.Min() : item.FromDate,
                        EndDate = remainingDates.Any() ? remainingDates.Max() : item.ToDate,
                        TotalDays = adjustedTotalDays,
                        Reason = item.Purpose,
                        RejectionReason = item.RejectionReason,
                        EmployeeName = item.EmployeeName,
                        ApproverManagerName = item.ManagerName,
                        ApprovedAt = item.ApprovedDate,
                        ApprovedBy = item.ManagerName,
                        CreatedAt = item.SubmissionDate,
                        AvailableDates = remainingDates

                    });
                }

                return responseList;
            }
        }

        //public async Task<IList<LeaveRequestResponse>> GetLeaveRequestByIdAsync(string employeeId)
        //{

        //    using (var context = new AppDbContext())
        //    {
        //        try
        //        {
        //            var leaveRequests = await (from a in context.LeaveRequisitionMaster
        //                                       join b in context.LeaveRequisitionDetails on a.Id equals b.MasterId
        //                                       //join d in context.AttendanceLeaves on a.Id equals d.RequisitionNumber
        //                                       join l in context.LeaveTypes on b.LeaveTypeCode equals l.Code
        //                                       join c in context.Employees on a.EmployeeCode equals c.Code
        //                                       join manager in context.Employees on a.ApproverEmployeeCode equals manager.Code
        //                                       where a.EmployeeCode == employeeId
        //                                       select new LeaveRequestResponse
        //                                       {
        //                                           Id = a.Id,
        //                                           Status =
        //                                                a.Status == "S" ? "submitted" :
        //                                                a.Status == "R" ? "rejected" :
        //                                                a.Status == "A" ? "approved" :
        //                                                "unknown",
        //                                           EmployeeId = a.EmployeeCode,
        //                                           //ApproverManagerId = a.ApproverEmployeeCode,
        //                                           LeaveType = l.Title,
        //                                           StartDate = b.FromDate,
        //                                           EndDate = b.ToDate,
        //                                           TotalDays = b.NoOfDays,
        //                                           Reason = b.Purpose,
        //                                           RejectionReason = b.RejectionReason,
        //                                           EmployeeName = c.Name,
        //                                           ApproverManagerName = manager.Name,
        //                                           ApprovedAt = a.ApprovedDate,
        //                                           ApprovedBy = manager.Name,
        //                                           CreatedAt = a.SubmissionDate


        //                                       }).ToListAsync();

        //            return leaveRequests;
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }
        //}
        private List<DateTime> RangeDates(DateTime startDate, DateTime endDate)
        {
            List<DateTime> listDates = new List<DateTime>();
            for (var dt = startDate; dt <= endDate; dt = dt.AddDays(1))
            {
                listDates.Add(dt);
            }
            return listDates;
        }
        public async Task<List<LeaveTypeResponse>> GetLeaveTypes()
        {
            using (var context = new AppDbContext())
            {
                return await context.LeaveTypes
                    .Select(x => new LeaveTypeResponse
                    {
                        LeaveCode = x.Code,
                        LeaveTitle = x.Title,
                        LeaveDays = x.NoOfDays
                    })
                    .ToListAsync();
            }
        }
        // Employee Request to Manager
        public async Task<CancelLeave>CancelLeave(CancelLeaveRequest cancelLeaveRequest)
        {
            using var context = new AppDbContext();

            var employee = await context.Employees
                .FirstOrDefaultAsync(c => c.Code == cancelLeaveRequest.EmployeeId);

            var manager = await context.Employees
                .FirstOrDefaultAsync(a => a.Code == employee.LeadCode);

            if (manager == null)
                throw new Exception("Manager not found");

            var cancelRecords = new List<CancelLeave>();

            foreach (var dateStr in cancelLeaveRequest.Dates)
            {
                if (!DateTime.TryParse(dateStr, out DateTime date))
                    throw new Exception($"Invalid date {dateStr}");

                var leaveDate = date.Date;

                var getLeave = await context.AttendanceLeaves
                    .FirstOrDefaultAsync(c =>
                        c.EmployeeCode == cancelLeaveRequest.EmployeeId &&
                        c.Year == leaveDate.Year &&
                        c.Month == leaveDate.Month &&
                        c.Day == leaveDate.Day);

                if (getLeave == null)
                    throw new Exception($"No leave exists on {leaveDate:dd-MM-yyyy}");


                bool alreadyRequested = await context.CancelLeaves.AnyAsync(c =>
                    c.EmployeeCode == cancelLeaveRequest.EmployeeId &&
                    c.LeaveDate == leaveDate &&
                    c.State == "S");

                if (alreadyRequested)
                    continue;

                var cancelLeave = new CancelLeave
                {
                    Id = Guid.NewGuid(),
                    EmployeeCode = cancelLeaveRequest.EmployeeId,
                    LeaveDate = leaveDate,
                    LeaveId = cancelLeaveRequest.LeaveId,
                    State = "S",
                    SubmissionDate = DateTime.Today,
                    Purpose = cancelLeaveRequest.Purpose,
                    ApproverEmployeeCode = employee.LeadCode
                };

                cancelRecords.Add(cancelLeave);
            }

            await context.CancelLeaves.AddRangeAsync(cancelRecords);
            await context.SaveChangesAsync();

            return cancelRecords[0];
        }


        //public async Task<CancelLeave> CancelLeave(CancelLeaveRequest cancelLeaveRequest)
        //{
        //    using (var context = new AppDbContext())
        //    {
        //        try
        //        {
        //            var employee = await context.Employees.FirstOrDefaultAsync(c => c.Code == cancelLeaveRequest.EmployeeId);
        //            var manager = await context.Employees.FirstOrDefaultAsync(a => a.Code == employee.LeadCode);
        //            if (manager == null)
        //                throw new Exception("Manager not found");

        //            List<DateTime> listDates = new List<DateTime>();
        //            listDates = RangeDates(cancelLeaveRequest.StartDate.Date, cancelLeaveRequest.EndDate.Date);

        //            foreach (var a in listDates)
        //            {
        //                var getStandardHour = context.Employees.FirstOrDefault(c => c.Code == employee.Code).StandardHourCode;
        //                DayOfWeek dayOfWeek = a.DayOfWeek;
        //                int weekDayNumber = dayOfWeek == DayOfWeek.Sunday ? 7 : (int)dayOfWeek;
        //                var getWeekDay = context.StandardHours.FirstOrDefault(c => c.GroupCode == getStandardHour && c.WeekDay == weekDayNumber);
        //                if (getWeekDay != null && (getWeekDay.Minutes == 0))
        //                {
        //                    throw new Exception($"The selected date {a.Date:dd-MM-yyyy} is a holiday.");
        //                }

        //                var getLeave = await context.AttendanceLeaves.FirstOrDefaultAsync(c => c.EmployeeCode == cancelLeaveRequest.EmployeeId &&
        //                                 c.Year == a.Year && c.Month == a.Month && c.Day == a.Day);

        //                if (getLeave != null)
        //                {
        //                    continue;
        //                }
        //                else
        //                {
        //                    throw new Exception($"There is no Applied Leave on {a.Date.ToString("dd-MM-yyyy")}");
        //                }

        //            }
        //            var cancelLeave = new CancelLeave
        //            {
        //                Id = Guid.NewGuid(),
        //                EmployeeCode = cancelLeaveRequest.EmployeeId,
        //                FromDate = cancelLeaveRequest.StartDate.Date,
        //                ToDate = cancelLeaveRequest.EndDate.Date,
        //                State = "S",
        //                SubmissionDate = DateTime.Today,
        //                Purpose = cancelLeaveRequest.Purpose,
        //                ApproverEmployeeCode = employee.LeadCode,
        //            };

        //            await context.CancelLeaves.AddAsync(cancelLeave);
        //            await context.SaveChangesAsync();

        //            Log.Information("Your leave cancellation request has been sent to {0} for approval.", manager.Name);

        //            // Notify employee
        //            await _notificationService.SendFcmNotificationAsync(employee.Code, employee.FcmToken,
        //                    "Request for leave cancellation", $"Your leave cancellation request has been sent to {manager.Name}");

        //            // Notify team lead
        //            await _notificationService.SendFcmNotificationAsync(manager.Code, manager.FcmToken,
        //                    "Request for leave cancellation", $"{employee.Name} requested for leave cancellation");

        //            return cancelLeave;

        //        }
        //        catch (Exception ex)
        //        {
        //            throw;
        //        }
        //    }
        //}

        // Manager Cancel the Leave
        public async Task<CancelLeave> ApproveCancelLeaves(string id, string approverId)
        {
            using (var context = new AppDbContext())
            {
                try
                {
                    var cancelLeave = await context.CancelLeaves
                        .FirstOrDefaultAsync(c =>
                            c.Id == Guid.Parse(id) &&
                            c.ApproverEmployeeCode == approverId);

                    if (cancelLeave == null)
                        throw new Exception("Cancel leave request not found.");

                    if (cancelLeave.State == "A")
                        throw new Exception("This cancellation is already approved.");

                    var employee = await context.Employees
                        .FirstOrDefaultAsync(c => c.Code == cancelLeave.EmployeeCode);

                    if (employee == null)
                        throw new Exception("Employee not found.");

                    cancelLeave.State = "A";
                    cancelLeave.ApproveDate = DateTime.Today;

                    var leaveDate = cancelLeave.LeaveDate.Date;

                    var removeLeave = await context.AttendanceLeaves
                        .FirstOrDefaultAsync(c =>
                            c.EmployeeCode == cancelLeave.EmployeeCode &&
                            c.Year == leaveDate.Year &&
                            c.Month == leaveDate.Month &&
                            c.Day == leaveDate.Day);
                    if (removeLeave != null)
                    {
                        context.AttendanceLeaves.Remove(removeLeave);
                    }
                    var removeAttendanceRecord = await context.AttendanceRecords
                        .FirstOrDefaultAsync(c =>
                            c.EmployeeCode == cancelLeave.EmployeeCode &&
                            c.Year == leaveDate.Year &&
                            c.Month == leaveDate.Month &&
                            c.Day == leaveDate.Day);
                    if (removeAttendanceRecord != null && removeAttendanceRecord.CheckInTime == null)
                    {
                        context.AttendanceRecords.Remove(removeAttendanceRecord);
                    }
                    
                    //var master = await context.LeaveRequisitionMaster.FirstOrDefaultAsync(a => a.Id == cancelLeave.LeaveId);
                    //master.Status = "C";
                    //context.LeaveRequisitionMaster.Update(master);
                    await context.SaveChangesAsync();

                    Log.Information("Leave cancellation approved by {0}", approverId);

                    //  Notify employee
                    await _notificationService.SendFcmNotificationAsync(
                        employee.Code,
                        employee.FcmToken,
                        "Leave Cancellation Approved",
                        $"Your leave cancellation for {leaveDate:dd-MM-yyyy} has been approved"
                    );

                    //  Notify manager (optional confirmation)
                    var managerInfo = await context.Employees
                        .Where(c => c.Code == approverId)
                        .Select(c => new { c.Code, c.FcmToken })
                        .FirstOrDefaultAsync();

                    if (managerInfo != null)
                    {
                        await _notificationService.SendFcmNotificationAsync(
                            managerInfo.Code,
                            managerInfo.FcmToken,
                            "Leave Cancellation Approved",
                            $"You approved leave cancellation of {employee.Name} for {leaveDate:dd-MM-yyyy}"
                        );
                    }

                    return cancelLeave;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        //public async Task<CancelLeave> ApproveCancelLeaves(string id, string approverId)
        //{
        //    using (var context = new AppDbContext())
        //    {
        //        try
        //        {
        //            var cancelLeave = await context.CancelLeaves.FirstOrDefaultAsync(c => c.Id == Guid.Parse(id) && c.ApproverEmployeeCode == approverId);
        //            var employee = await context.Employees.FirstOrDefaultAsync(c => c.Code == cancelLeave.EmployeeCode);
        //            if (cancelLeave != null)
        //            {
        //                cancelLeave.ApproveDate = DateTime.Today;
        //                cancelLeave.State = "A";
        //            }

        //            var ListDates = RangeDates(cancelLeave.FromDate, cancelLeave.ToDate);
        //            foreach (var a in ListDates)
        //            {
        //                var RemoveLeave = await context.AttendanceLeaves.FirstOrDefaultAsync(c => c.EmployeeCode == cancelLeave.EmployeeCode 
        //                                  && c.Year == a.Year && c.Month == a.Month && c.Day == a.Day);

        //                if (RemoveLeave != null)
        //                {
        //                    context.AttendanceLeaves.Remove(RemoveLeave);
        //                }
        //                await context.SaveChangesAsync();
        //                Log.Information("leave cancellation has been approved by {0}", approverId);

        //                // Notify employee
        //                await _notificationService.SendFcmNotificationAsync(employee.Code, employee.FcmToken,
        //                    "Leave Cancellation Approved", $"Your leave cancellation has been approved");

        //                var managerInfo = await context.Employees.Where(c => c.Code == approverId).Select(c => new { c.Code, c.FcmToken }).FirstOrDefaultAsync();

        //                // Notify team lead
        //                await _notificationService.SendFcmNotificationAsync(managerInfo.Code, managerInfo.FcmToken,
        //                        "Leave Cancellation Approved", $"You approved leave cancellation of {employee.Name}");

        //            }

        //            return cancelLeave;
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //    }
        //}
        public async Task<IList<CancelLeaveResponse>> GetPendingCancelLeaves(string ApproverId)
        {
            using (var context = new AppDbContext())
            {
                try
                {
                    var getleaves = await (from c in context.CancelLeaves
                                     join e in context.Employees on c.EmployeeCode equals e.Code
                                     where c.State == "S" && c.ApproverEmployeeCode == ApproverId
                                     select new CancelLeaveResponse
                                     {
                                         Id = c.Id.ToString(),
                                         EmployeeCode = c.EmployeeCode,
                                         EmployeeName = e.Name,
                                         //FromDate = c.FromDate,
                                         //ToDate = c.ToDate,
                                         LeaveDate = c.LeaveDate,
                                         ApproverEmployeeCode = c.ApproverEmployeeCode,
                                         Purpose = c.Purpose,
                                         State =    c.State == "S" ? "submitted" :
                                                    c.State == "R" ? "rejected" :
                                                    c.State == "A" ? "approved"
                                                    : "unknown",
                                         SubmissionDate = c.SubmissionDate
                                     }).ToListAsync();

                    return getleaves;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public async Task<List<DateTime>> GetCancelLeaveDatesAsync(int no)
        {
            using (var context = new AppDbContext())
            {
                try
                {
                    var Lists = new List<DateTime>();
                    var getLeaveDuration = await context.LeaveRequisitionDetails.FirstOrDefaultAsync(c => c.MasterId == no);
                    if (getLeaveDuration != null)
                    {
                        Lists = RangeDates(getLeaveDuration.FromDate, getLeaveDuration.ToDate); 
                    }
                    return Lists;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task<bool> GetCheckTodayLeave(string employeeId, string companyCode)
        {
            using (var context = new AppDbContext())
            {
                var day = DateTime.Today.Day;
                var leave = await context.AttendanceLeaves.Where(a => a.EmployeeCode == employeeId && a.Day == day && a.Month == DateTime.Today.Month && a.Year == DateTime.Today.Year).ToListAsync();
                return leave.Count == 0 ? false : true;
            }
        }

        public async Task<LeaveStatsResponse> GetLeaveStatisticsAsync(string employeeCode)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    int currentYear = DateTime.Now.Year;
                    var leaveConf = await context.AttendanceLeaves.Where(a => a.EmployeeCode == employeeCode && a.Year == currentYear).ToListAsync();

                    var halfdayLeaves = leaveConf.Where(c => c.TypeCode == "HD").ToList();
                    double availedLeaves = leaveConf.Except(halfdayLeaves).Count();
                    availedLeaves = availedLeaves + (halfdayLeaves.Count() / 2.0);

                    double totalLeaves = 24;
                    //double availedLeaves = leaveConf.Count();

                    double remainLeaves = totalLeaves - availedLeaves;

                    return new LeaveStatsResponse
                    {
                        TotalLeaves = totalLeaves,
                        YearlyAvailed = availedLeaves,
                        AvailableLeaves = remainLeaves
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting leaves stats: {ex.Message}");
            }
        }
        public async Task<CancelLeave> GetCancelLeaveRequestForRejectionByIdAsync(string Id, RejectLeaveRequest request)
        {
            using (var context = new AppDbContext())
            {
                try
                {
                    var getCancelLeave = await context.CancelLeaves.FirstOrDefaultAsync(c => c.Id == Guid.Parse(Id) && c.ApproverEmployeeCode == request.ApproverId);
                    if (getCancelLeave != null)
                    {
                        getCancelLeave.Reason = request.Reason;
                        getCancelLeave.RejectDate = DateTime.Today;
                        getCancelLeave.State = "R";
                        context.CancelLeaves.Update(getCancelLeave);
                        await context.SaveChangesAsync();
                    }
                    return getCancelLeave;
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }
    }
}
