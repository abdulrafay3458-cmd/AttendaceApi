using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class LeaveRequisitionDetailBuilder
    {
        public static void BuildLeaveRequisitionDetail(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<LeaveRequisitionDetail>();
            builder.ToTable("LeaveRequisitionDetail", "App").HasKey(d => new { d.Id });
            builder.Property(d => d.Id).HasColumnName("leave_requisition_detail_id").ValueGeneratedNever().HasMaxLength(4);
            builder.Property(d => d.DisplayId).HasColumnName("leave_requisition_detail_display_id").HasMaxLength(10).IsRequired().IsUnicode(false);
            builder.Property(d => d.MasterId).HasColumnName("leave_requisition_detail_master_id").HasMaxLength(4).IsRequired();
            builder.Property(d => d.FromDate).HasColumnName("leave_requisition_detail_from_date").IsRequired();
            builder.Property(d => d.ToDate).HasColumnName("leave_requisition_detail_to_date").IsRequired();
            builder.Property(d => d.LeaveTypeCode).HasColumnName("leave_requisition_detail_leave_type_code").HasMaxLength(3).IsRequired().IsUnicode(false);
            builder.Property(d => d.NoOfDays).HasColumnName("leave_requisition_detail_no_of_days").HasMaxLength(9).IsRequired();
            builder.Property(d => d.Purpose).HasColumnName("leave_requisition_detail_purpose").HasMaxLength(200).IsUnicode(false);            
            builder.Property(d => d.AdjustmentType).HasColumnName("leave_requisition_detail_adjustment_type").HasMaxLength(2).IsUnicode(false);
            builder.Property(d => d.PurposeCode).HasColumnName("leave_requisition_detail_purpose_code").HasMaxLength(4).IsRequired();
            builder.Property(d => d.RejectionReason).HasColumnName("leave_requisition_detail_rejection_reason").HasMaxLength(200).IsUnicode(false);

            builder.HasOne(e => e.LeaveRequisitionMaster)
             .WithMany(e => e.LeaveRequisitionDetails)
             .HasForeignKey(e => e.MasterId)
             .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
