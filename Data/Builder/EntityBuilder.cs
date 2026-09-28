using Microsoft.EntityFrameworkCore;
using AttendanceAPI.Entities;

namespace AttendanceAPI.Data.Builder
{
    public class EntityBuilder
    {
        public static void BuildEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entity>(entity =>
            {
                entity.ToTable("Entity", "General");

                entity.HasKey(e => e.Code);

                entity.Property(e => e.Code)
                      .HasColumnName("entity_code")
                      .HasMaxLength(4)
                      .IsRequired()
                      .IsUnicode(false);

                entity.Property(e => e.Name)
                      .HasColumnName("entity_name")
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.LegalName)
                      .HasColumnName("entity_legal_name")
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.ShortName)
                      .HasColumnName("entity_short_name")
                      .HasMaxLength(15)
                      .IsRequired();

                entity.Property(e => e.CountryCode)
                      .HasColumnName("entity_country_code")
                      .HasMaxLength(10)
                      .IsUnicode(false)
                      .IsRequired();

                entity.Property(e => e.Address)
                      .HasColumnName("entity_address")
                      .HasMaxLength(500);

                entity.Property(e => e.Email)
                      .HasColumnName("entity_email")
                      .HasMaxLength(50)
                      .IsUnicode(false);

                entity.Property(e => e.Phone)
                      .HasColumnName("entity_phone")
                      .HasMaxLength(50)
                      .IsUnicode(false);

                entity.Property(e => e.Fax)
                      .HasColumnName("entity_fax")
                      .HasMaxLength(30);

                entity.Property(e => e.IndustryCode)
                      .HasColumnName("entity_industry_code")
                      .HasMaxLength(8)
                      .IsUnicode(false);

                entity.Property(e => e.GlCode)
                      .HasColumnName("entity_gl_code")
                      .HasMaxLength(2)
                      .IsUnicode(false);

                entity.Property(e => e.FiscalYear)
                      .HasColumnName("entity_fiscal_year")
                      .HasMaxLength(20);

                entity.Property(e => e.latitude)
                      .HasColumnName("latitude");

                entity.Property(e => e.longitude)
                      .HasColumnName("longitude");

                entity.Property(e => e.ParentCode)
                      .HasColumnName("entity_parent_code")
                      .HasMaxLength(20)
                      .IsUnicode(false);

                entity.Property(e => e.ChildCode)
                      .HasColumnName("entity_child_code")
                      .HasMaxLength(20)
                      .IsUnicode(false);

                entity.Property(e => e.Status)
                      .HasColumnName("entity_status")
                      .HasColumnType("bit")
                      .IsRequired();
                entity.Property(c => c.ClientVendor)
                        .HasColumnName("entity_client_vendor")
                        .HasColumnType("bit")
                        .IsRequired();

                entity.Property(c => c.AllowedRadiusMeters)
                        .HasColumnName("entity_allowed_radius_meters")
                        .HasMaxLength(20);
            });
        }
    }
}
