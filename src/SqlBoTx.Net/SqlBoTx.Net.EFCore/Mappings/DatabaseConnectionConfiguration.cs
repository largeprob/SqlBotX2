using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlBoTx.Net.Domain.DatabaseConnections;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlBoTx.Net.EFCore.Mappings
{
    public class DatabaseConnectionConfiguration : IEntityTypeConfiguration<DatabaseConnection>
    {
        public void Configure(EntityTypeBuilder<DatabaseConnection> builder)
        {
            //主键
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.CreatedDate).IsRequired() ;
            builder.Property(x => x.LastModifiedDate).IsRequired(false);
            builder.Property(x => x.ConnectionName).IsRequired(true).HasColumnType("NVARCHAR(255)");
            builder.Property(x => x.ConnectionType).IsRequired(true);
            builder.Property(x => x.ConnectionString).IsRequired(true).HasColumnType("NVARCHAR(MAX)");
            builder.Property(x => x.UserName).IsRequired(false).HasColumnType("NVARCHAR(255)");
            builder.Property(x => x.UserPassword).IsRequired(false).HasColumnType("NVARCHAR(255)");
            builder.Property(x => x.Description).IsRequired(false).HasColumnType("NVARCHAR(255)");

            //设置外键关系
            builder.HasMany(x => x.TableStructures)
                   .WithOne()
                   .HasForeignKey(x => x.ConnectionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}  
