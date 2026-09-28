using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class LeaveTypeBuilder
    {
        public static void BuildLeaveType(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<LeaveType>();
            builder.ToTable("LeavesType", "App").HasKey(d => new { d.Code});
            builder.Property(d => d.Code).HasColumnName("leave_type_code").HasMaxLength(4).IsRequired().IsUnicode(false);
            builder.Property(d => d.Title).HasColumnName("leave_type_title").HasMaxLength(20).IsRequired().IsUnicode(false);
            builder.Property(d => d.NoOfDays).HasColumnName("leave_type_no_of_days").IsRequired(false).HasMaxLength(4);
        }
    }
}
