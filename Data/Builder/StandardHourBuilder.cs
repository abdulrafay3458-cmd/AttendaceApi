using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class StandardHourBuilder
    {
        public static void BuildStandardHour(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<StandardHour>();
            builder.ToTable("StandardHour", "App").HasKey(s => new {s.GroupCode, s.WeekDay});
            builder.Property(s => s.GroupCode).HasColumnName("standard_hour_group_code").HasMaxLength(4).ValueGeneratedNever();
            builder.Property(s => s.WeekDay).HasColumnName("standard_hour_week_day").HasMaxLength(4).ValueGeneratedNever();
            builder.Property(s => s.WeekDayName).HasColumnName("standard_hour_week_day_name").IsRequired().HasMaxLength(3).IsUnicode(false);
            builder.Property(s => s.Hours).HasColumnName("standard_hour_hours").IsRequired().HasMaxLength(5).IsUnicode(false);
            builder.Property(s => s.Minutes).HasColumnName("standard_hour_minutes").HasMaxLength(4).IsRequired();
            builder.Property(s => s.TimeIn).HasColumnName("standard_hour_time_in").IsRequired(false).HasMaxLength(5).IsUnicode(false);
            builder.Property(s => s.TimeOut).HasColumnName("standard_hour_time_out").IsRequired(false).HasMaxLength(5).IsUnicode(false);
            builder.Property(s => s.HalfDayMinutes).HasColumnName("standard_hour_half_day_min").HasMaxLength(4).IsRequired(false);
            builder.Property(s => s.HalfDayHourOnLateArrival).HasColumnName("standard_hour_half_day_on_late_arrival").IsRequired(false).HasMaxLength(5).IsUnicode(false);

            builder.HasOne(s => s.StandardHourGroup)
                .WithMany(s => s.EmployeeStandardHour)
                .HasForeignKey(s => s.GroupCode)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
