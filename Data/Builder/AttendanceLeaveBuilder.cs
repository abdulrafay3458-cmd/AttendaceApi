using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class AttendanceLeaveBuilder
    {
        public static void BuildAttendanceLeave(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<AttendanceLeave>();
            builder.ToTable("AttendanceLeave", "App").HasKey(d => new { d.EmployeeCode , d.Day,d.Month,d.Year,d.Fortnight});
            
            builder.Property(d => d.EmployeeCode).HasColumnName("att_leave_employee_code").HasMaxLength(20).IsUnicode(false).IsRequired();
            builder.Property(d => d.Day).HasColumnName("att_leave_day").IsRequired().HasMaxLength(4);
            builder.Property(d => d.Month).HasColumnName("att_leave_month").IsRequired().HasMaxLength(4);
            builder.Property(d => d.Year).HasColumnName("att_leave_year").IsRequired().HasMaxLength(4);
            builder.Property(d => d.Fortnight).HasColumnName("att_leave_fortnight").IsRequired().HasMaxLength(4);
            builder.Property(d => d.Date).HasColumnName("att_leave_date").IsRequired().HasMaxLength(8);
            builder.Property(d => d.TypeCode).HasColumnName("att_leave_type_code").HasMaxLength(4).IsRequired().IsUnicode(false);
            builder.Property(d => d.Hours).HasColumnName("att_leave_hour").HasMaxLength(5).IsRequired(false).IsUnicode(false);
            builder.Property(d => d.Minutes).HasColumnName("att_leave_minutes").HasMaxLength(4).IsRequired(false);
            builder.Property(d => d.RequisitionNumber).HasColumnName("att_leave_requisition_number").HasMaxLength(4).IsRequired(false);
            //builder.Property(d => d.PurposeCode).HasColumnName("leave_purpose_code").HasMaxLength(4).IsRequired(false);
            //builder.Property(d => d.Purpose).HasColumnName("leave_purpose").HasMaxLength(300).IsRequired(false);

            builder.HasOne(e => e.Employee)
             .WithMany(e => e.AttendanceLeaves)
             .HasForeignKey(e => e.EmployeeCode)
             .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.LeaveType)
             .WithMany(e => e.Leaves)
             .HasForeignKey(e => e.TypeCode)
             .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.AttendanceRecord)
            .WithMany(e => e.Leaves)
            .HasForeignKey(e => new { e.EmployeeCode ,e.Day,e.Month,e.Year ,e.Fortnight})
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
