using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class UserTaskBuilder
    {
        public static void BuildTask(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<UserTask>();
            builder.ToTable("UserTask", "App").HasKey(d => d.Id);
            builder.Property(d => d.Id).HasColumnName("task_id");
            builder.Property(d => d.Title).HasColumnName("task_title").HasMaxLength(50).IsRequired().IsUnicode(false);
            builder.Property(d => d.Description).HasColumnName("task_description").HasMaxLength(200).IsRequired().IsUnicode(false);
            builder.Property(d => d.AssignedBy).HasColumnName("task_assigned_by").HasMaxLength(50).IsRequired().IsUnicode(false);
            builder.Property(d => d.AssignedByName).HasColumnName("task_assigned_by_name").HasMaxLength(50).IsRequired(false).IsUnicode(false);
            builder.Property(d => d.AssignedTo).HasColumnName("task_assigned_to").HasMaxLength(50).IsRequired().IsUnicode(false);
            builder.Property(d => d.AssignedToName).HasColumnName("task_assigned_to_name").HasMaxLength(50).IsRequired(false).IsUnicode(false);
            builder.Property(d => d.TotalHoursWorked).HasColumnName("task_total_hours_worked").HasMaxLength(50).IsRequired(false);
            builder.Property(d => d.AssignedDate).HasColumnName("task_assign_date").IsRequired();
            builder.Property(d => d.DueDate).HasColumnName("task_due_date").IsRequired(false);
            builder.Property(d => d.IsActive).HasColumnName("task_is_active").IsRequired();
            builder.Property(d => d.IsDelete).HasColumnName("task_is_deleted").IsRequired().HasDefaultValueSql("0");
            builder.Property(d => d.DeletedAt).HasColumnName("task_deleted_at").IsRequired(false);
            builder.Property(d => d.UpdatedAt).HasColumnName("task_updated_at").IsRequired(false);
            builder.Property(d => d.CompletedAt).HasColumnName("task_completed_at").IsRequired(false);
            builder.Property(d => d.Priority).HasColumnName("task_priority").HasMaxLength(20).IsRequired().IsUnicode(false);
            builder.Property(d => d.Status).HasColumnName("task_status").HasMaxLength(20).IsRequired().IsUnicode(false);
            builder.Property(d => d.EntityCode).HasColumnName("task_entity_code").HasMaxLength(4).IsRequired(false).IsUnicode(false);
            builder.Property(d => d.TaskPreferance).HasColumnName("task_preferance").HasMaxLength(20).IsRequired(false).IsUnicode(false);

            builder.HasOne(e => e.Entity)
                            .WithMany(e => e.UserTasks)
                            .HasForeignKey(e => e.EntityCode)
                            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
