using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlBoTx.Net.Domain.BusinessEntities;
using SqlBoTx.Net.Domain.BusinessMetrics;
using SqlBoTx.Net.Domain.DomainTerms;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlBoTx.Net.EFCore.Mappings
{
    public class DomainTermMapping : IEntityTypeConfiguration<DomainTerm>
    {
        public void Configure(EntityTypeBuilder<DomainTerm> builder)
        {
            // 主键
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            // 属性配置
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Alias).HasColumnType("NVARCHAR(500)").HasConversion(
               //存
               v => string.Join(',', v ?? Array.Empty<string>()),
               //取
               v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
            );
            builder.Property(x => x.Description).HasColumnType("NVARCHAR(500)");

            builder.HasOne(x => x.Domain)
           .WithMany(x => x.Terms)
           .HasForeignKey(x => x.DomainId)
           .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
