using AttendanceAPI.Data.Builder;
using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<UserVerifyOPT> UserVerifyOPTs { get; set; }
        public DbSet<AttendanceLeave> AttendanceLeaves { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords  { get; set; }
        public DbSet<AttendenceHistory> AttendenceHistories  { get; set; }
        public DbSet<Company> Company  { get; set; }
        public DbSet<Department> Departments  { get; set; }
        public DbSet<Designation> Designations  { get; set; }
        public DbSet<MachineDevice> MachineDevices  { get; set; }
        public DbSet<UserDevice> UserDevices  { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Holiday> Holiday { get; set; }
        public DbSet<LeaveBalance> LeaveBalances { get; set; }
        public DbSet<LeaveRequisitionBalance> LeaveRequisitionBalances { get; set; }
        public DbSet<LeaveRequisitionDetail> LeaveRequisitionDetails { get; set; }
        public DbSet<LeaveRequisitionMaster> LeaveRequisitionMaster { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<RequisitionPurposeType> RequisitionPurposeType { get; set; }
        public DbSet<RoleType> RoleTypes { get; set; }
        public DbSet<StandardHour> StandardHours { get; set; }
        public DbSet<StandardHourGroup> StandardHourGroups { get; set; }
        public DbSet<UserTask> Tasks { get; set; }
        public DbSet<TeamTask> TeamTasks { get; set; }
        public DbSet<TaskOvertime> TaskOvertimes { get; set; }
        public DbSet<TaskTimeLogs> TaskTimeLogs { get; set; }
        public DbSet<UserRole> UserRoles  { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Entity> Entities { get; set; }
        public DbSet<RegistrationImage> RegistrationImages { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<CancelLeave> CancelLeaves { get; set; }
        public DbSet<FaceImage> FaceImages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=10.1.5.239 ;database=AttendenceApp;User ID=user_development;Password=PresidentInfo;TrustServerCertificate=True;");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var pipeline = new List<Action<ModelBuilder>>
            {
                AppUserBuilder.BuildAppUser,
                AttendanceLeaveBuilder.BuildAttendanceLeave,
                AttendanceRecordBuilder.BuildAttendanceBuilder,
                AttendanceHistoryBuilder.BuildAttendanceHistoryBuilder,
                CompanyBuilder.BuildCompany,
                DepartmentBuilder.BuildDepartment,
                DesignationBuilder.BuildDesignation,
                DeviceBuilder.BuildDevice,
                UserDeviceBuilder.BuildUserDevice,
                EmployeeBuilder.BuildEmployee,
                HolidayBuilder.BuildHoliday,
                LeaveBalanceBuilder.BuildLeaveBalance,
                LeaveRequisitionBalanceBuilder.BuildLeaveRequisitionBalance,
                LeaveRequisitionDetailBuilder.BuildLeaveRequisitionDetail,
                LeaveRequisitionMasterBuilder.BuildLeaveRequisitionMaster,
                LeaveTypeBuilder.BuildLeaveType,
                NotificationBuilder.BuildNotification,
                RequisitionPurposeTypeBuilder.BuildRequisitionPurposeType,
                RoleTypeBuilder.BuildRoleType,
                StandardHourBuilder.BuildStandardHour,
                StandardHourGroupBuilder.BuildStandardHourGroup,
                UserTaskBuilder.BuildTask,
                TeamTaskBuilder.BuildTeamTask,
                TaskOvertimeBuilder.BuildTaskOvertime,
                TaskTimeLogBuilder.BuildTaskTimeLog,
                UserRoleBuilder.BuildUserRole,
                RefreshTokenBuilder.BuildRefreshToken,
                EntityBuilder.BuildEntity,
                RegistrationImageBuilder.BuildRegistrationImage,
                SettingsConfigurationBuilder.BuildConfiguration,
                CancelLeaveBuilder.BuildCancelLeave,
                FaceImageBuilder.BuildFaceImage,
                UserVerifyBuilder.BuildUserVerify,
            };

            pipeline.ForEach(build => build(modelBuilder));
        }
    }
} 
