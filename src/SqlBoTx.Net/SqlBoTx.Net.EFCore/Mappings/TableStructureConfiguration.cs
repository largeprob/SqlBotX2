using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlBoTx.Net.Domain.DatabaseConnections;
using SqlBoTx.Net.Domain.TableStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlBoTx.Net.EFCore.Mappings
{

    public class TableStructureConfiguration : IEntityTypeConfiguration<TableStructure>
    {
        public void Configure(EntityTypeBuilder<TableStructure> builder)
        {
            //主键
            builder.HasKey(x => x.TableId);
            builder.Property(x => x.TableId).ValueGeneratedOnAdd();

            builder.Property(x => x.ConnectionId).IsRequired();
            builder.Property(x => x.TableName).IsRequired().HasColumnType("NVARCHAR(255)");
            builder.Property(x => x.SchemaName);
            builder.Property(x => x.Alias).HasColumnType("NVARCHAR(255)");
            builder.Property(x => x.FieldCount).IsRequired();
            builder.Property(x => x.Description).IsRequired().HasColumnType("NVARCHAR(255)");

        }
    }
}
