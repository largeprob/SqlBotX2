using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlBoTx.Net.Domain.DatabaseConnections;
using SqlBoTx.Net.Domain.TableFields;
using SqlBoTx.Net.Domain.TableStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlBoTx.Net.EFCore.Mappings
{
    public class TableFieldConfiguration : IEntityTypeConfiguration<TableField>
    {
        public void Configure(EntityTypeBuilder<TableField> builder)
        {
            //主键
            builder.HasKey(x => x.FieldId);
            builder.Property(x => x.FieldId).ValueGeneratedOnAdd();

            builder.Property(x => x.TableId).IsRequired();
            builder.Property(x => x.ColumnName).IsRequired();
            builder.Property(x => x.DataType).IsRequired();
            builder.Property(x => x.IsPrimaryKey).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.IsNullable).IsRequired().HasDefaultValue(true);
            builder.Property(x => x.IsIdentity).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.DefaultValue).IsRequired(false);
            builder.Property(x => x.FieldDescription).IsRequired();
        }
    }
}
