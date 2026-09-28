using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class UserDeviceBuilder
    {
        public static void BuildUserDevice(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<UserDevice>();
            builder.ToTable("UserDevices", "App").HasKey(c => new { c.EmployeeCode, c.DeviceHash });
            builder.Property(c => c.EmployeeCode).HasColumnName("employee_code").ValueGeneratedNever();
            builder.Property(c => c.DeviceHash).HasColumnName("device_hash").HasMaxLength(64).IsRequired().IsUnicode(false);
            builder.Property(c => c.DeviceManufacturer).HasColumnName("device_manufacturer").HasMaxLength(30).IsRequired(false).IsUnicode(false);
            builder.Property(c => c.DeviceModel).HasColumnName("device_model").HasMaxLength(30).IsRequired(false).IsUnicode(false);
            builder.Property(c => c.isApproved).HasColumnName("is_approved").IsRequired(false);
            builder.Property(c => c.RequestedAt).HasColumnName("requested_at").IsRequired(false);
            builder.Property(c => c.ApprovedAt).HasColumnName("approved_at").IsRequired(false);
            builder.Property(c => c.ApprovedBy).HasColumnName("approved_by").IsRequired(false).HasMaxLength(20).IsUnicode(false);
            builder.Property(c => c.RejectedAt).HasColumnName("rejected_at").IsRequired(false);
            builder.Property(c => c.RejectedBy).HasColumnName("rejected_by").IsRequired(false).HasMaxLength(20).IsUnicode(false);
            builder.Property(c => c.RejectedReason).HasColumnName("rejected_reason").HasMaxLength(100).IsRequired(false).IsUnicode(false);
            builder.HasOne(c => c.DeviceEmployee)
                .WithOne(c => c.EmpDevice)
                .HasForeignKey<UserDevice>(c => c.EmployeeCode);
        }
    }
}
