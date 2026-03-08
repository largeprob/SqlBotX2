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

    public class TableStructureConfiguration : IEntityTypeConfiguration<TableStructure>
    {
        public void Configure(EntityTypeBuilder<TableStructure> builder)
        {
            //主键
            builder.HasKey(x => x.TableId);
            builder.Property(x => x.TableId).ValueGeneratedOnAdd();

            builder.Property(x => x.ConnectionId).IsRequired();
            builder.Property(x => x.TableName).IsRequired().HasColumnType("NVARCHAR(255)");
            builder.Property(x => x.DisplayName).IsRequired().HasColumnType("NVARCHAR(255)");
            builder.Property(x => x.FieldCount).IsRequired();
            builder.Property(x => x.Description).IsRequired().HasColumnType("NVARCHAR(255)");
            builder.Property(x => x.Granularity);
            builder.Property(x => x.GranularityLevel);

            //设置外键关系
            builder.HasMany(x => x.TableFields)
                   .WithOne()
                   .HasForeignKey(x => x.TableId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
