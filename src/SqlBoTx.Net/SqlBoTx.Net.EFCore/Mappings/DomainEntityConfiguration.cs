using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlBoTx.Net.Domain.BusinessEntities;
using SqlBoTx.Net.Domain.BusinessMetrics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SqlBoTx.Net.EFCore.Mappings
{
    public class DomainEntityConfiguration : IEntityTypeConfiguration<DomainEntity>
    {
        public void Configure(EntityTypeBuilder<DomainEntity> builder)
        {
            //// 单字段普通索引
            //builder.HasIndex(x => x.EntityId)
            //       .HasDatabaseName("idx_entity_id");

            //// 多字段复合索引
            //builder.HasIndex(x => new { x.EntityId, x.DataType })
            //       .HasDatabaseName("idx_entity_id_data_type");

            //// 单字段唯一键
            //builder.HasIndex(x => x.ColumnName)
            //       .IsUnique()
            //       .HasDatabaseName("uq_column_name");

            //// 多字段复合唯一键
            //builder.HasIndex(x => new { x.EntityId, x.ColumnName })
            //       .IsUnique()
            //       .HasDatabaseName("uq_entity_column");

            // 主键
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();

            // 属性配置
            builder.Property(x => x.DomainId).IsRequired();
            builder.Property(x => x.ReferenceConnectId).IsRequired();
            builder.Property(x => x.ReferenceTableId).IsRequired();
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Alias).IsUnicode().IsRequired();
            builder.Property(x => x.Description);
            builder.Property(x => x.Tags).HasColumnType("NVARCHAR(500)").HasConversion(
              //存
              v => string.Join(',', v ?? Array.Empty<string>()),
              //取
              v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
            );

            //设置外键关系
            builder.HasOne(x => x.Database)
                   .WithMany()
                   .HasForeignKey(x => x.ReferenceConnectId)
                   .OnDelete(DeleteBehavior.NoAction);

            //设置外键关系
            builder.HasOne(x => x.Table)
                   .WithMany()
                   .HasForeignKey(x => x.ReferenceTableId)
                   .OnDelete(DeleteBehavior.NoAction);

            //设置外键关系
            builder.HasOne(x => x.Domain)
                   .WithMany(x => x.Entities)
                   .HasForeignKey(x => x.DomainId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
