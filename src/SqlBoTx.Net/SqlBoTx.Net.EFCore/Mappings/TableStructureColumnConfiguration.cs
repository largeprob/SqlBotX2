using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlBoTx.Net.Domain.DatabaseConnections;
using SqlBoTx.Net.Domain.TableStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlBoTx.Net.EFCore.Mappings
{
    public class TableStructureColumnConfiguration : IEntityTypeConfiguration<TableStructureColumn>
    {
        public void Configure(EntityTypeBuilder<TableStructureColumn> builder)
        {
            //主键
            builder.HasKey(x => x.FieldId);
            builder.Property(x => x.FieldId).ValueGeneratedOnAdd();

            builder.Property(x => x.TableId).IsRequired();
            builder.Property(x => x.Label).IsRequired();
            builder.Property(x => x.ColumnName).IsRequired();
            builder.Property(x => x.DataType).IsRequired();
            builder.Property(x => x.DataTypeSchema);
            builder.Property(x => x.DefaultValue);
            builder.Property(x => x.Description);
            builder.Property(x => x.IsPrimaryKey).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.IsIdentity).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.IsRequired).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.IsUnique).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.IsReference).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.ReferenceTableName);
            builder.Property(x => x.ReferenceColumn);
            builder.Property(x => x.IsComputed).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.Expression).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.IsIndex).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.Indexs).HasColumnType("NVARCHAR(500)").HasConversion(
             //存
             v => string.Join(',', v ?? Array.Empty<string>()),
             //取
             v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
            );

            //设置外键关系
            builder.HasOne(x => x.Table)
                   .WithMany(x => x.Columns)
                   .HasForeignKey(x => x.TableId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
