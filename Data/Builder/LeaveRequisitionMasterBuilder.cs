using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class LeaveRequisitionMasterBuilder
    {
        public static void BuildLeaveRequisitionMaster(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<LeaveRequisitionMaster>();
            builder.ToTable("LeaveRequisitionMaster", "App").HasKey(d => d.Id);

            builder.Property(e => e.Id).HasColumnName("leave_requisition_master_id").ValueGeneratedNever().HasMaxLength(4);
            builder.Property(e => e.EmployeeCode).HasColumnName("leave_requisition_master_employee_code").HasMaxLength(20).IsRequired().IsUnicode(false);
            builder.Property(d => d.DisplayId).HasColumnName("leave_requisition_master_display_id").HasMaxLength(10).IsRequired().IsUnicode(false);
            builder.Property(d => d.Status).HasColumnName("leave_requisition_master_status").HasMaxLength(5).IsRequired().IsUnicode(false);
            builder.Property(d => d.SubmissionDate).HasColumnName("leave_requisition_master_submission_date").IsRequired(false);
            builder.Property(d => d.ApprovedDate).HasColumnName("leave_requisition_master_approved_date").HasMaxLength(20).IsRequired(false).IsUnicode(false);
            builder.Property(d => d.ApproverEmployeeCode).HasColumnName("leave_requisition_master_approver_employee_code").HasMaxLength(20).IsRequired().IsUnicode(false);
            builder.Property(d => d.PreviousHajjYear).HasColumnName("leave_requisition_master_previous_hajj_year").HasMaxLength(4).IsRequired(false);
            builder.Property(d => d.RejectionDate).HasColumnName("leave_requisition_master_rejection_date").IsRequired(false);


            builder.HasOne(e => e.Employee)
              .WithMany(e => e.LeaveRequisitionMasters)
              .HasForeignKey(e => e.EmployeeCode)
              .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
