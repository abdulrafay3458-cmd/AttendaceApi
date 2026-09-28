using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class DesignationBuilder
    {
        public static void BuildDesignation(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<Designation>();
            builder.ToTable("Designation", "General").HasKey(d => d.Code);
            builder.Property(d => d.Code).HasColumnName("designation_code").HasMaxLength(4).ValueGeneratedNever();
            builder.Property(d => d.Name).HasColumnName("designation_name").HasMaxLength(50).IsRequired().IsUnicode(false); 
        }
    }
}
