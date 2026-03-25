using JasperFx.Blocks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlBoTx.Net.Domain.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SqlBoTx.Net.EFCore.Mappings
{
    public class DomainEntityAttrConfiguration : IEntityTypeConfiguration<DomainEntityAttr>
    {
        public void Configure(EntityTypeBuilder<DomainEntityAttr> builder)
        {
            // 主键
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            // 属性配置
            builder.Property(x => x.EntityId).IsRequired();
            builder.Property(x => x.ColumnName).IsRequired();
            builder.Property(x => x.Label).IsRequired();
            builder.Property(x => x.IsRequired).IsRequired();
            builder.Property(x => x.DataType).IsRequired();
            builder.Property(x => x.DataTypeSchema);
            builder.Property(x => x.DefaultValue);
            builder.Property(x => x.Description);
            builder.Property(x => x.StructureRole).IsRequired();
            builder.OwnsOne(x => x.ForeignKeyMetaData, fk =>
            {
                fk.ToJson();
                fk.OwnsOne(f => f.Polymorphic, poly =>
                {
                    poly.OwnsMany(p => p.Mappings);
                });
            });
            builder.Property(x => x.SemanticType).IsRequired();
            builder.Property(x => x.DimensionCategory);
            builder.Property(x => x.TimeGranularity);
            builder.Property(x => x.SupportedAggregations);

            // 实体关系
            builder.HasOne(x => x.Entity)
                   .WithMany(x => x.Attrs)
                   .HasForeignKey(x => x.EntityId)
                   .OnDelete(DeleteBehavior.Restrict);

            // DB 字段
            builder.HasOne(x => x.Column)
                   .WithMany()
                   .HasForeignKey(x => x.ColumnId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
