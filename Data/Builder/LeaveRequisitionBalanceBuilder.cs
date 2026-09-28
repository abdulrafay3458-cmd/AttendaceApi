using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class LeaveRequisitionBalanceBuilder
    {
        public static void BuildLeaveRequisitionBalance(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<LeaveRequisitionBalance>();
            builder.ToTable("LeaveRequisitionBalance", "App")
                .HasKey(d => d.Id);
            builder.Property(d => d.Id).HasColumnName("leave_requisition_balance_id").ValueGeneratedNever().HasMaxLength(4);
            builder.Property(d => d.Balance).HasColumnName("leave_requisition_balance").HasMaxLength(9).IsRequired();
            builder.Property(d => d.MasterId).HasColumnName("leave_requisition_balance_master_id").HasMaxLength(4).IsRequired();
            builder.Property(d => d.CurrentBalance).HasColumnName("leave_requisition_current_balance").HasMaxLength(9).IsRequired();

            builder.HasOne(c => c.LeaveRequisitionMaster)
                .WithMany(c => c.LeaveRequisitionBalances)
                .HasForeignKey(c => c.MasterId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
