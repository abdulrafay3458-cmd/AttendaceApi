using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class DepartmentBuilder
    {
        public static void BuildDepartment(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<Department>();
            builder.ToTable("Department", "General").HasKey(d => d.Code);
            builder.Property(d => d.Code).HasColumnName("department_code").HasMaxLength(8).ValueGeneratedNever();
            builder.Property(d => d.Name).HasColumnName("department_name").HasMaxLength(50).IsRequired(true).IsUnicode(false);
            builder.Property(d => d.CompanyCode).HasColumnName("department_company_code").HasMaxLength(4).IsRequired(true);
            builder.Property(d => d.LeadCode).HasColumnName("department_lead_code").HasMaxLength(20).IsRequired(false);
            builder.Property(d => d.IsActive).HasColumnName("department_is_active").HasDefaultValue(true);
            builder.Property(d => d.InactiveFromDate).HasColumnName("department_inactive_from_date").IsRequired(false);

            builder.HasOne(d => d.Company)
                .WithMany(d => d.Departments)
                .HasForeignKey(d => d.CompanyCode)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
