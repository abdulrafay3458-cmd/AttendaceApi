using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class AttendanceRecordBuilder
    {
        public static void BuildAttendanceBuilder(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<AttendanceRecord>();
            builder.ToTable("AttendanceRecord", "App").HasKey(c => new {c.EmployeeCode, c.Day, c.Month, c.Year, c.Fortnight});
            builder.Property(c => c.EmployeeCode).HasColumnName("att_rec_employee_code").HasMaxLength(20).IsRequired().IsUnicode(false);
            builder.Property(c => c.Year).HasColumnName("att_rec_year").HasMaxLength(4).IsRequired();
            builder.Property(c => c.Month).HasColumnName("att_rec_month").HasMaxLength(4).IsRequired();
            builder.Property(c => c.Fortnight).HasColumnName("att_rec_fortnight").HasMaxLength(4).IsRequired();
            builder.Property(c => c.Day).HasColumnName("att_rec_day").HasMaxLength(4).IsRequired();
            builder.Property(c => c.StandardHourCode).HasColumnName("att_rec_standard_hour_code").HasMaxLength(4).IsRequired(false);
            builder.Property(c => c.StandardHourWeekday).HasColumnName("att_rec_standard_hour_weekday").HasMaxLength(4).IsRequired(false);
            builder.Property(c => c.AttendanceDate).HasColumnName("att_rec_attendance_date").IsRequired();
            builder.Property(c => c.CheckInTime).HasColumnName("att_rec_check_in_time").IsRequired(false);
            builder.Property(c => c.CheckOutTime).HasColumnName("att_rec_check_out_time").IsRequired(false);
            builder.Property(c => c.CheckInSource).HasColumnName("att_rec_check_in_source").IsRequired(false).HasMaxLength(10);
            builder.Property(c => c.CheckOutSource).HasColumnName("att_rec_check_out_source").IsRequired(false).HasMaxLength(10);
            builder.Property(c => c.CheckInLatitude).HasColumnName("att_rec_check_in_latitude").IsRequired(false);
            builder.Property(c => c.CheckInLongitude).HasColumnName("att_rec_check_in_longitude").IsRequired(false);
            builder.Property(c => c.CheckInAddress).HasColumnName("att_rec_check_in_address").IsRequired(false);
            builder.Property(c => c.CheckOutLatitude).HasColumnName("att_rec_check_out_latitude").IsRequired(false);
            builder.Property(c => c.CheckOutLongitude).HasColumnName("att_rec_check_out_longitude").IsRequired(false);
            builder.Property(c => c.CheckOutAddress).HasColumnName("att_rec_check_out_address").IsRequired(false);
            builder.Property(c => c.CheckInPhoto).HasColumnName("att_rec_check_in_photo").IsRequired(false);
            builder.Property(c => c.CheckOutPhoto).HasColumnName("att_rec_check_out_photo").IsRequired(false);
            builder.Property(c => c.TotalHours).HasColumnName("att_rec_total_hours").IsRequired(false).HasMaxLength(10);
            builder.Property(c => c.Status).HasColumnName("att_rec_status").IsRequired(false).HasMaxLength(10);
            builder.Property(c => c.CheckInDeviceId).HasColumnName("att_rec_check_in_device_id").IsRequired(false);
            builder.Property(c => c.CheckOutDeviceId).HasColumnName("att_rec_check_out_device_id").IsRequired(false);

            builder.HasOne(c => c.StandardHour)
                .WithMany(c => c.AttendanceRecords)
                .HasForeignKey(c => new { c.StandardHourCode, c.StandardHourWeekday })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.CheckInDevice)
                .WithMany(c => c.DeviceForCheckIn)
                .HasForeignKey(c => c.CheckInDeviceId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasOne(c => c.CheckOutDevice)
                .WithMany(c => c.DeviceForCheckOut)
                .HasForeignKey(c => c.CheckOutDeviceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
