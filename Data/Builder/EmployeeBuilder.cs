using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class EmployeeBuilder
    {
        public static void BuildEmployee (ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<Employee>();
            builder.ToTable("Employee", "General").HasKey(e => e.Code);
            builder.Property(e => e.Code).HasColumnName("employee_code").HasMaxLength(20).IsUnicode(false).ValueGeneratedNever();
            builder.Property(e => e.ZkUserId).HasColumnName("employee_zk_user_id").HasMaxLength(10);
            builder.Property(e => e.Name).HasColumnName("employee_name").HasMaxLength(50).IsRequired().IsUnicode(false);
            builder.Property(e => e.LeadCode).HasColumnName("employee_lead_code").HasMaxLength(20).IsRequired(false);
            builder.Property(e => e.CompanyCode).HasColumnName("employee_company_code").HasMaxLength(4).IsRequired();
            builder.Property(e => e.Email).HasColumnName("employee_email").HasMaxLength(50).IsRequired().IsUnicode(false);
            builder.Property(e => e.Phone).HasColumnName("employee_phone").HasMaxLength(20).IsRequired().IsUnicode(false);
            builder.Property(e => e.DepartmentCode).HasColumnName("employee_department_code").HasMaxLength(8).IsRequired();
            builder.Property(e => e.DesignationCode).HasColumnName("employee_designation_code").HasMaxLength(4).IsRequired();
            builder.Property(e => e.StandardHourCode).HasColumnName("employee_standard_hour_code").HasMaxLength(4).IsRequired();
            builder.Property(e => e.CardNumber).HasColumnName("employee_cardnumber").HasMaxLength(20).IsRequired(false).IsUnicode(false);
            builder.Property(e => e.IsActive).HasColumnName("employee_is_active").IsRequired();
            builder.Property(e => e.FcmToken).HasColumnName("employee_fcm_token").IsRequired(false).IsUnicode(false);
            builder.Property(e => e.JoinDate).HasColumnName("employee_join_date").IsRequired(true);
            builder.Property(e => e.LeaveDate).HasColumnName("employee_leave_date").IsRequired(false);
            builder.Property(e => e.CreatedAt).HasColumnName("employee_created_at").IsRequired(true);
            builder.Property(e => e.FaceImage).HasColumnName("employee_face_embedding").IsRequired(false);
            builder.Property(e => e.IsAppUser).HasColumnName("employee_is_app_user").IsRequired();

            builder.HasOne(e => e.Designation)
                .WithMany(e => e.Employees)
                .HasForeignKey(e => e.DesignationCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.StandardHourGroup)
                .WithMany(e => e.Employees)
                .HasForeignKey(e => e.StandardHourCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Department)
                .WithMany(e => e.Employees)
                .HasForeignKey(e => e.DepartmentCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Company)
                .WithMany(e => e.Employees)
                .HasForeignKey(e => e.CompanyCode)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
