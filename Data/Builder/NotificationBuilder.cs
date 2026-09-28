using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class NotificationBuilder
    {
        public static void BuildNotification(ModelBuilder modelBuilder) 
        {
            var builder = modelBuilder.Entity<Notification>();
            builder.ToTable("Notification", "App").HasKey(d => new { d.Id });
            builder.Property(e => e.Id).HasColumnName("notification_id").ValueGeneratedOnAdd();
            builder.Property(d => d.UserCode).HasColumnName("notification_user_code").HasMaxLength(50).IsRequired().IsUnicode(false);
            builder.Property(d => d.Title).HasColumnName("notification_title").IsRequired().HasMaxLength(150).IsUnicode(false);
            builder.Property(d => d.Message).HasColumnName("notification_message").IsRequired().HasMaxLength(250).IsUnicode(false);
            builder.Property(d => d.Type).HasColumnName("notification_type").IsRequired().HasMaxLength(20).IsUnicode(false);
            builder.Property(d => d.IsRead).HasColumnName("notification_is_read").IsRequired().HasDefaultValue(false);
            builder.Property(d => d.CreatedAt).HasColumnName("notification_createdAt").IsRequired();
            builder.Property(d => d.CheckType).HasColumnName("notification_check_type").IsRequired(false).HasMaxLength(20).IsUnicode(false);
            builder.Property(d => d.CheckTime).HasColumnName("notification_check_time").IsRequired(false);
            builder.Property(d => d.Location).HasColumnName("notification_location").HasMaxLength(200).IsUnicode(false).IsRequired(false);
        }
    }
}
