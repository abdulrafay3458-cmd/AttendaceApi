using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class AppUserBuilder
    {
        public static void BuildAppUser (ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<AppUser>();
            builder.ToTable("AppUser", "Settings").HasKey(a => a.Code);
            builder.Property(a => a.Code).HasColumnName("user_code").HasMaxLength(50).IsUnicode(false);
            builder.Property(a => a.Name).HasColumnName("user_name").IsRequired().HasMaxLength(50).IsUnicode(false);
            builder.Property(a => a.EmployeeCode).HasColumnName("user_employee_code").IsUnicode(false).HasMaxLength(20);
            builder.Property(a => a.IsActive).HasColumnName("user_is_active");
            builder.Property(a => a.Salt).HasColumnName("user_salt").IsRequired().IsUnicode(false);
            builder.Property(a => a.Password).HasColumnName("user_password").IsRequired().IsUnicode(false);

            builder.HasOne(a => a.Employee)
                .WithOne(a => a.AppUser)
                .HasForeignKey<AppUser>(a => a.EmployeeCode)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
