using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class FaceImageBuilder
    {
        public static void BuildFaceImage(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<FaceImage>();
            builder.ToTable("FaceImages", "App").HasKey(c => new { c.Id, c.EmpCode });
            builder.Property(c => c.Id).HasColumnName("face_image_id").ValueGeneratedOnAdd();
            builder.Property(c => c.EmpCode).HasColumnName("employee_code").HasMaxLength(20).IsUnicode(false).ValueGeneratedNever();
            builder.Property(c => c.EmpFaceImage).HasColumnName("face_image").HasColumnType("nvarchar(max)").IsRequired(false);

            builder.HasOne(c => c.Employee)
                .WithOne(c => c.EmployeeFaceImage)
                .HasForeignKey<FaceImage>(e => e.EmpCode)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
