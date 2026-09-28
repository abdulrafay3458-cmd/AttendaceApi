using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class UserRoleBuilder
    {
        public static void BuildUserRole(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<UserRole>();
            builder.ToTable("UserRole", "App").HasKey(d => new {d.UserCode, d.RoleId});
            builder.Property(d => d.UserCode).HasColumnName("user_role_user_Code").IsRequired().HasMaxLength(50).IsUnicode(false);
            builder.Property(d => d.RoleId).HasColumnName("user_role_role_id").IsRequired();

            builder.HasOne(d => d.User)
                .WithMany(d => d.UserRoles)
                .HasForeignKey(d => d.UserCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.UserRolesTypes)
                .WithMany(d => d.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
