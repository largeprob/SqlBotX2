using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlBoTx.Net.Domain.Share.TableRelationships;
using SqlBoTx.Net.Domain.TableRelationships;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SqlBoTx.Net.EFCore.Mappings
{
    public class TableRelationshipConfiguration : IEntityTypeConfiguration<TableRelationship>
    {
        public void Configure(EntityTypeBuilder<TableRelationship> builder)
        {
            //主键
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.SourceTableId).IsRequired();
            builder.Property(x => x.TargetTableId).IsRequired();
            builder.Property(x => x.SourceCardinality).IsRequired();
            builder.Property(x => x.TargetCardinality).IsRequired();
            builder.Property(x => x.Conditions).IsRequired().HasColumnType("NVARCHAR(500)").HasConversion(
             //存
             v => v == null || v.Count <= 0 ? "[]" : JsonSerializer.Serialize(v),
             //取
             v => JsonSerializer.Deserialize<List<TableRelationshipCondition>>(v)
             );
            builder.Property(x => x.RelationshipDescription);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt);

            //设置外键关系 - 双向导航
            builder.HasOne(x => x.SourceTable)
                   .WithMany(t => t.SourceTableRelationships)
                   .HasForeignKey(x => x.SourceTableId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.TargetTable)
                   .WithMany(t => t.TargetTableRelationships)
                   .HasForeignKey(x => x.TargetTableId)
                   .OnDelete(DeleteBehavior.NoAction);

            // 添加索引以提高查询性能
            builder.HasIndex(x => x.SourceTableId);
            builder.HasIndex(x => x.TargetTableId);
        }
    }
}
