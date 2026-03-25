using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlBoTx.Net.Domain.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SqlBoTx.Net.EFCore.Mappings
{
    public class DomainEntityRelConfiguration : IEntityTypeConfiguration<DomainEntityRel>
    {
        public void Configure(EntityTypeBuilder<DomainEntityRel> builder)
        {
            // 主键
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();

            // 属性配置
            builder.Property(x => x.Type).IsRequired();
            builder.Property(x => x.SourceEntityId).IsRequired();
            builder.Property(x => x.TargetEntityId).IsRequired();
            builder.Property(x => x.SourceRole).IsRequired();
            builder.Property(x => x.TargetRole).IsRequired();
            builder.Property(x => x.SourceCardinality).IsRequired();
            builder.Property(x => x.TargetCardinality).IsRequired();
            builder.Property(x => x.InverseOf);
            builder.Property(x => x.CascadeDelete);
            builder.Property(x => x.JoinConditions).IsRequired().HasColumnType("NVARCHAR(500)").HasConversion(
               //存
               v => v == null || v.Count <= 0 ? "[]" : JsonSerializer.Serialize(v),
               //取
               v => JsonSerializer.Deserialize<List<DomainEntityRelJoin>>(v)
            );

            //设置外键关系
            builder.HasOne(x => x.SourceEntity)
                   .WithMany(x => x.SourceRels)
                   .HasForeignKey(x => x.SourceEntityId)
                   .OnDelete(DeleteBehavior.NoAction);

            //设置外键关系
            builder.HasOne(x => x.TargetEntity)
                  .WithMany(x => x.TargetRels)
                   .HasForeignKey(x => x.TargetEntityId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
