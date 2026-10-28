using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.Tags;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbTagConfigurations : IEntityTypeConfiguration<TbTag>
{
    public void Configure(EntityTypeBuilder<TbTag> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__TbTag__3214EC072088A8E9");
        builder.ToTable("TbTag");
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.NameAr).HasMaxLength(100);
        builder.Property(e => e.NameEn).HasMaxLength(100);
    }
}