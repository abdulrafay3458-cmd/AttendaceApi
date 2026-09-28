using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class CancelLeaveBuilder
    {
        public static void BuildCancelLeave(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<CancelLeave>();
            builder.ToTable("CancelLeave", "App").HasKey(a => new { a.Id, a.EmployeeCode });
            builder.Property(a => a.Id).HasColumnName("cancel_leave_Id").HasMaxLength(50).IsUnicode(false);
            builder.Property(a => a.EmployeeCode).HasColumnName("cancel_leave_employee_code").IsUnicode(false).HasMaxLength(20).IsRequired();
            //builder.Property(a => a.FromDate).HasColumnName("cancel_leave_from_date");
            //builder.Property(a => a.ToDate).HasColumnName("cancel_leave_to_date");
            builder.Property(a => a.LeaveDate).HasColumnName("leave_date");
            builder.Property(a => a.ApproverEmployeeCode).HasColumnName("cancel_leave_approver_employee_code").IsUnicode(false).HasMaxLength(20).IsRequired();
            builder.Property(c => c.Purpose).HasColumnName("cancel_leave_purpose").HasMaxLength(200).IsUnicode(false);
            builder.Property(c => c.State).HasColumnName("cancel_leave_state").HasMaxLength(5).IsRequired().IsUnicode(false);
            builder.Property(c => c.SubmissionDate).HasColumnName("cancel_leave_submission_date").IsRequired();
            builder.Property(c => c.ApproveDate).HasColumnName("cancel_leave_approve_date").IsRequired(false);
            builder.Property(c => c.Reason).HasColumnName("cancel_leave_reject_reason").HasMaxLength(200);
            builder.Property(c => c.RejectDate).HasColumnName("cancel_leave_reject_at").IsRequired(false);
            modelBuilder.Entity<CancelLeave>()
                .HasOne(c => c.Leave)
                .WithMany(l => l.CancelLeaves)
                .HasForeignKey(c => c.LeaveId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
