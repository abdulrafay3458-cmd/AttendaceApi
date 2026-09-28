using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class AttendanceHistoryBuilder
    {
        public static void BuildAttendanceHistoryBuilder(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<AttendenceHistory>();
            builder.ToTable("AttendanceHistory", "App").HasKey(c => new { c.AttendenceHistoryId });
            builder.Property(c => c.EmployeeCode).HasColumnName("att_his_employee_code").HasMaxLength(20).IsRequired().IsUnicode(false);
            builder.Property(c => c.AttendanceDate).HasColumnName("att_his_attendance_date").IsRequired();
            builder.Property(c => c.CheckInTime).HasColumnName("att_his_check_in_time").IsRequired(false);
            builder.Property(c => c.CheckOutTime).HasColumnName("att_his_check_out_time").IsRequired(false);
            builder.Property(c => c.Source).HasColumnName("att_his_source").IsRequired(false).HasMaxLength(20);
            builder.Property(c => c.Latitude).HasColumnName("att_his_latitude").IsRequired(false);
            builder.Property(c => c.Longitude).HasColumnName("att_his_longitude").IsRequired(false);
            builder.Property(c => c.Location).HasColumnName("att_his_location").IsRequired(false);
            builder.Property(c => c.DeviceId).HasColumnName("att_his_check_in_device_id").IsRequired(false);
            builder.Property(c => c.Photo).HasColumnName("att_his_photo").IsRequired(false);
        }
    }
}
