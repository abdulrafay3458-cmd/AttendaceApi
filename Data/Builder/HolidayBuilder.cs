using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class HolidayBuilder
    {
        public static void BuildHoliday(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<Holiday>();
            builder.ToTable("Holiday", "App").HasKey(h => new { h.Day, h.Month, h.Year});
            builder.Property(h => h.Day).HasColumnName("holiday_day").IsRequired().HasMaxLength(4);
            builder.Property(h => h.Month).HasColumnName("holiday_month").IsRequired().HasMaxLength(4);
            builder.Property(h => h.Year).HasColumnName("holiday_year").IsRequired().HasMaxLength(4);
            builder.Property(h => h.Fortnight).HasColumnName("holiday_fortnight").IsRequired().HasMaxLength(4);
            builder.Property(h => h.Weekday).HasColumnName("holiday_weekday").IsRequired().HasMaxLength(4);
            builder.Property(h => h.HolidayDate).HasColumnName("holiday_date").IsRequired();
            builder.Property(h => h.Title).HasColumnName("holiday_title").HasMaxLength(50).IsRequired().IsUnicode(false);

        }
    }
}
