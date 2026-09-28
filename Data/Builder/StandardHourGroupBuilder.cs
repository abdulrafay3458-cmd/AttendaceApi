using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class StandardHourGroupBuilder
    {
        public static void BuildStandardHourGroup(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<StandardHourGroup>();
            builder.ToTable("StandardHourGroup", "App").HasKey(s => s.Code);
            builder.Property(s => s.Code).HasColumnName("shg_code").HasMaxLength(4).ValueGeneratedNever();
            builder.Property(s => s.Title).HasColumnName("shg_title").IsRequired().HasMaxLength(50).IsUnicode(false);
        }
    }
}
