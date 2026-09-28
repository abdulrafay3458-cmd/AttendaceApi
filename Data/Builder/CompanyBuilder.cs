using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class CompanyBuilder
    {
        public static void BuildCompany (ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<Company>();
            builder.ToTable("Company", "General").HasKey(c => c.Code);
            builder.Property(c => c.Code).HasColumnName("company_code").HasMaxLength(4).ValueGeneratedNever();
            builder.Property(c => c.Name).HasColumnName("company_name").HasMaxLength(50).IsRequired().IsUnicode(false);
            builder.Property(c => c.NTN).HasColumnName("company_ntn").HasMaxLength(20).IsRequired(false).IsUnicode(false);
            builder.Property(c => c.IsTaxApplicable).HasColumnName("company_is_tax_applicable");
            builder.Property(c => c.IsTaxExemption).HasColumnName("company_is_tax_exemption");
            builder.Property(c => c.GlCode).HasColumnName("company_gl_code").HasMaxLength(10).IsRequired(false).IsUnicode(false);
            builder.Property(c => c.FunctionalCurrency).HasColumnName("company_functional_currency").HasMaxLength(4).IsRequired(false).IsUnicode(false);
            builder.Property(c => c.DefaultBankAccountCode).HasColumnName("company_default_bank_acc_code").HasMaxLength(4).IsRequired(false).IsUnicode(false);
            builder.Property(c => c.DefaultBankCode).HasColumnName("company_default_bank_code").HasMaxLength(4).IsRequired(false).IsUnicode(false);
        }
    }
}
