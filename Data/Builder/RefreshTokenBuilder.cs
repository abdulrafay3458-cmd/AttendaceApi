using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public static class RefreshTokenBuilder
    {
        public static void BuildRefreshToken(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("refresh_tokens", "App").HasKey(c => c.UserCode);
                entity.Property(e => e.UserCode).HasColumnName("refresh_token_user_code").IsRequired().HasMaxLength(50).IsUnicode(false);
                entity.Property(e => e.Token).HasColumnName("refresh_token").HasMaxLength(500).IsRequired();

                entity.Property(e => e.ExpiresAt)
                      .HasColumnName("expires_at")
                      .IsRequired();

                entity.Property(e => e.CreatedAt)
                      .HasColumnName("created_at")
                      .IsRequired();

                entity.HasOne(r => r.user)
                      .WithOne(e => e.RefreshToken)
                      .HasForeignKey<RefreshToken>(r => r.UserCode)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }

}
