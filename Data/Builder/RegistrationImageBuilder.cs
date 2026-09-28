using Microsoft.EntityFrameworkCore;
using AttendanceAPI.Entities;

namespace AttendanceAPI.Data.Builder
{
    public class RegistrationImageBuilder
    {
        public static void BuildRegistrationImage(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RegistrationImage>(entity =>
            {
                entity.ToTable("registration_image", "App").HasKey(e => e.Code);
                entity.Property(e => e.Code).HasColumnName("registration_image_code");
                entity.Property(e => e.EmployeeCode).HasColumnName("registration_image_employee_code").IsRequired().HasMaxLength(20).IsUnicode(false);
                entity.Property(e => e.AdminCode).HasColumnName("registration_imaget_admin_code").IsRequired().HasMaxLength(20).IsRequired();

                entity.Property(e => e.CompanyCode)
                       .HasColumnName("registration_image_company_code")
                       .IsRequired().HasMaxLength(8);

                entity.Property(e => e.RequestDate)
                      .HasColumnName("registration_image_request_date")
                      .IsRequired();

                entity.Property(e => e.ApproveDate)
                      .HasColumnName("registration_image_approve_date");

                entity.Property(e => e.RejectionDate)
                       .HasColumnName("registration_image_rejection_date");

                entity.Property(e => e.RejectionReason).HasColumnName("registration_image_rejection_reason").HasMaxLength(250).IsUnicode(false);
                entity.Property(e => e.FaceImage).HasColumnName("registration_image_face_image").IsRequired().IsUnicode(false);

                entity.Property(e => e.Status)
                       .HasColumnName("registration_image_status")
                       .IsRequired();

                entity.Property(e => e.HistoryStatus)
                       .HasColumnName("registration_image_history_status")
                       .IsRequired();


                entity.HasOne(r => r.Employee)
                      .WithMany(e => e.RegistrationImage)
                      .HasForeignKey(r => r.EmployeeCode)                      
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
