using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class SettingsConfigurationBuilder
    {
        public static void BuildConfiguration (ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<Configuration>();
            builder.ToTable("Configuration", "Settings").HasKey(c => new { c.Heading, c.Title});
            builder.Property(p => p.Heading).HasColumnName("configuration_heading").HasMaxLength(20).IsRequired().IsUnicode(false);
            builder.Property(p => p.Title).HasColumnName("configuration_title").HasMaxLength(30).IsRequired().IsUnicode(false);
            builder.Property(p => p.ValueType).HasColumnName("configuration_value_type").HasMaxLength(7).IsRequired().IsUnicode(false);
            builder.Property(p => p.Value).HasColumnName("configuration_value").HasMaxLength(1000).IsRequired().IsUnicode(false);
        }
    }
}
