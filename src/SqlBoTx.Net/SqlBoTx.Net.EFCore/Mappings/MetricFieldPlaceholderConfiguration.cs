using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlBoTx.Net.Domain.BusinessMetrics;
using SqlBoTx.Net.Domain.BusinessObjectives;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlBoTx.Net.EFCore.Mappings
{
    public class MetricFieldPlaceholderConfiguration : IEntityTypeConfiguration<MetricFieldPlaceholder>
    {
        public void Configure(EntityTypeBuilder<MetricFieldPlaceholder> builder)
        {
            // 主键
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            // 属性配置
            builder.Property(x => x.MetricId).IsRequired();
            builder.Property(x => x.Index);
            builder.Property(x => x.EntityId).IsRequired().HasColumnType("NVARCHAR(255)");
            builder.Property(x => x.AttrId).HasColumnType("NVARCHAR(500)");
            builder.Property(x => x.AttrName).HasColumnType("NVARCHAR(500)");

            //设置外键关系
            builder.HasOne(x => x.DomainMetric)
                   .WithMany(x => x.Placeholders)
                   .HasForeignKey(x => x.MetricId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
