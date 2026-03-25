using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlBoTx.Net.Domain.BusinessMetrics;
using SqlBoTx.Net.Domain.BusinessObjectives;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlBoTx.Net.EFCore.Mappings
{
    public class DomainMetricConfiguration : IEntityTypeConfiguration<DomainMetric>
    {
        public void Configure(EntityTypeBuilder<DomainMetric> builder)
        {
            // 主键
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            // 属性配置
            builder.Property(x => x.MetricName).IsRequired().HasColumnType("NVARCHAR(255)");
            builder.Property(x => x.MetricCode).IsRequired().HasColumnType("NVARCHAR(500)");
            builder.Property(x => x.Alias).HasColumnType("NVARCHAR(500)").HasConversion(
               //存
               v => string.Join(',', v ?? Array.Empty<string>()),
               //取
               v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
            );
            builder.Property(x => x.Scope).IsRequired();
            builder.Property(x => x.Description).HasColumnType("NVARCHAR(500)");
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.Expression).IsRequired().HasColumnType("NVARCHAR(500)"); ;

            builder.HasOne(x => x.Domain)
                .WithMany(x => x.Metrics)
                .HasForeignKey(x => x.DomainId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
