using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class RoleTypeBuilder
    {
        public static void BuildRoleType (ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<RoleType>();
            builder.ToTable("RoleType", "General").HasKey(d => d.Id);
            builder.Property(d => d.Id).HasColumnName("role_id");
            builder.Property(d => d.Title).HasColumnName("role_title").HasMaxLength(50).IsRequired().IsUnicode(false);
            builder.Property(d => d.IsHidden).HasColumnName("role_is_hidden").IsRequired();
        }
    }
}
