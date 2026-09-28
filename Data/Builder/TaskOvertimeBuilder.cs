using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class TaskOvertimeBuilder
    {
        public static void BuildTaskOvertime(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<TaskOvertime>();

            builder.ToTable("TaskOvertime", "App")
                .HasKey(d => d.Id);

            builder.Property(d => d.Id)
                .HasColumnName("overtime_id");

            builder.Property(d => d.UserTaskId)
                .HasColumnName("overtime_task_id")
                .IsRequired();

            builder.Property(d => d.EmployeeId)
                .HasColumnName("overtime_employee_id")
                .HasMaxLength(50)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(d => d.OvertimeDate)
                .HasColumnName("overtime_date")
                .IsRequired();

            builder.Property(d => d.OvertimeHours)
                .HasColumnName("overtime_hours")
                .IsRequired();

            builder.Property(d => d.Reason)
                .HasColumnName("overtime_reason")
                .HasMaxLength(500)
                .IsRequired(false)
                .IsUnicode(false);

            builder.Property(d => d.Status)
                .HasColumnName("overtime_status")
                .HasMaxLength(20)
                .IsRequired()
                .IsUnicode(false)
                .HasDefaultValue("pending");

            builder.Property(d => d.ApprovedBy)
                .HasColumnName("overtime_approved_by")
                .HasMaxLength(50)
                .IsRequired(false)
                .IsUnicode(false);


            builder.Property(d => d.ApprovedAt)
                .HasColumnName("overtime_approved_at")
                .IsRequired(false);

            builder.Property(d => d.RejectionBy)
                .HasColumnName("overtime_rejection_by")
                .HasMaxLength(50)
                .IsRequired(false)
                .IsUnicode(false);

            builder.Property(d => d.RejectionReason)
                .HasColumnName("overtime_rejection_reason")
                .HasMaxLength(500)
                .IsRequired(false)
                .IsUnicode(false);

            builder.Property(d => d.CreatedAt)
                .HasColumnName("overtime_created_at")
                .IsRequired();

            builder.HasOne(e => e.UserTask)
                .WithMany(e => e.TaskOvertimes)
                .HasForeignKey(e => e.UserTaskId)
                .OnDelete(DeleteBehavior.Cascade);           
        }
    }
}
