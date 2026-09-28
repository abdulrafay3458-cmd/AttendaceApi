using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class TaskTimeLogBuilder
    {
        public static void BuildTaskTimeLog(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<TaskTimeLogs>();
            builder.ToTable("TaskTimeLogs", "App").HasKey(d => d.Id);
            builder.Property(d => d.Id).HasColumnName("time_log_id");
            builder.Property(d => d.TaskId).HasColumnName("time_log_task_id");
            builder.Property(d => d.LogDate).HasColumnName("time_log_date").IsRequired();
            builder.Property(d => d.StartTime).HasColumnName("time_log_start_time").IsRequired(false);
            builder.Property(d => d.StopTime).HasColumnName("time_log_stop_time").IsRequired(false);
            builder.Property(d => d.HoursWorked).HasColumnName("time_log_hours_worked").IsRequired(false);
            builder.Property(d => d.Notes).HasColumnName("time_log_notes").HasMaxLength(100).IsRequired(false);
            builder.Property(d => d.EmployeeCode).HasColumnName("time_log_employee_code").HasMaxLength(20).IsUnicode(false);
            builder.Property(d => d.IsExtraWork).HasColumnName("is_extra_work");

            builder.HasOne(d => d.Tasks)
                .WithMany(d => d.TaskTimeLogs)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(t => t.Employee)
                 .WithMany(e => e.TaskTimeLogs)
                 .HasForeignKey(t => t.EmployeeCode)
                 .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
