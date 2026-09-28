using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class UserVerifyBuilder
    {
        public static void BuildUserVerify(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<UserVerifyOPT>();
            builder.ToTable("UserVerifyOPT", "App").HasKey(a=>a.UserID);
            builder.Property(c => c.UserID).HasColumnName("user_verify_id").HasMaxLength(50).IsUnicode(false);
            builder.Property(c => c.Email).HasColumnName("user_verify_email").HasMaxLength(50).IsUnicode(false).IsRequired(false);
            builder.Property(c => c.OTP).HasColumnName("user_verify_otp").HasMaxLength(50).IsUnicode(false).IsRequired(false);
        }
    }
}
