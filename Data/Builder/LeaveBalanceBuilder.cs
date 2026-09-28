using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class LeaveBalanceBuilder
    {
        public static void BuildLeaveBalance(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<LeaveBalance>();
            builder.ToTable("LeaveBalance", "App").HasKey(d => new { d.EmployeeCode,d.Period,d.LeaveType });          
            builder.Property(d => d.EmployeeCode).HasColumnName("leave_balance_employee_code").HasMaxLength(20).IsUnicode(false).IsRequired();
            builder.Property(d => d.Period).HasColumnName("leave_balance_period").HasMaxLength(4).IsRequired();
            builder.Property(d => d.Balance).HasColumnName("leave_balance_balance").HasMaxLength(9).IsRequired(false);
            builder.Property(d => d.LeaveType).HasColumnName("leave_balance_leave_type").IsRequired().HasMaxLength(5).IsUnicode(false);

            builder.HasOne(e => e.Employee)
             .WithMany(e => e.LeaveBalances)
             .HasForeignKey(e => e.EmployeeCode)
             .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
