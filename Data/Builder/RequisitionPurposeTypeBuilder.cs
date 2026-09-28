using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class RequisitionPurposeTypeBuilder
    {
        public static void BuildRequisitionPurposeType(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<RequisitionPurposeType>();
            builder.ToTable("RequisitionPurposeType", "App").HasKey(x => x.Id);
            builder.Property(c => c.Id).HasColumnName("requisition_type_id").HasMaxLength(4).ValueGeneratedNever();
            builder.Property(c => c.Prupose).HasColumnName("requisition_purpose").HasMaxLength(500).IsRequired().IsUnicode(false); 
        }
    }
}
