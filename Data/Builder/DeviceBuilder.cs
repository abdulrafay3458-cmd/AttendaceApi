using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class DeviceBuilder
    {
        public static void BuildDevice (ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<MachineDevice>();
            builder.ToTable("Devices", "General");//.HasKey(d => d.Id);
            builder.Property(d => d.Id).HasColumnName("device_id").ValueGeneratedNever();
            builder.Property(d => d.CompanyCode).HasColumnName("device_company_code").IsRequired().HasMaxLength(4);
            builder.Property(d => d.Name).HasColumnName("device_name").HasMaxLength(50).IsRequired().IsUnicode(false);
            builder.Property(d => d.IpAddress).HasColumnName("device_ip_address").HasMaxLength(20).IsRequired().IsUnicode(false);
            builder.Property(d => d.Port).HasColumnName("device_port").HasMaxLength(10).IsRequired();
            builder.Property(d => d.Location).HasColumnName("device_location").HasMaxLength(100).IsRequired().IsUnicode(false);
            builder.Property(d => d.Status).HasColumnName("device_status").HasMaxLength(10).IsRequired().IsUnicode(false);
            builder.Property(d => d.LastSync).HasColumnName("device_last_sync").IsRequired(false);
            builder.Property(d => d.MachineNumber).HasColumnName("device_machine_number").HasMaxLength(4).IsRequired();
            builder.Property(d => d.IsActive).HasColumnName("device_is_active").IsRequired();
            builder.Property(d => d.CanReadLogs).HasColumnName("device_canreadlogs").IsRequired();
            builder.Property(d => d.IsDelete).HasColumnName("device_is_delete").IsRequired().HasDefaultValue(false);
            builder.Property(d => d.DeletedAt).HasColumnName("device_deleted_at").IsRequired(false);

            builder.HasOne(d => d.OwnerCompany)
                .WithMany(d => d.CompanyDevices)
                .HasForeignKey(d => d.CompanyCode)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
