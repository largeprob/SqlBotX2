using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlBoTx.Net.Domain.BusinessMetrics;
using SqlBoTx.Net.Domain.BusinessObjectives;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlBoTx.Net.EFCore.Mappings
{
    public class BusinessMetricConfiguration : IEntityTypeConfiguration<BusinessObjectiveMetric>
    {
        public void Configure(EntityTypeBuilder<BusinessObjectiveMetric> builder)
        {
            // 主键
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            // 属性配置
            builder.Property(x => x.MetricName).IsRequired().HasColumnType("NVARCHAR(255)");
            builder.Property(x => x.MetricCode).IsRequired().HasColumnType("NVARCHAR(500)");
            builder.Property(x => x.Alias).HasColumnType("NVARCHAR(500)");
            builder.Property(x => x.BusinessObjectiveId).IsRequired();
            builder.Property(x => x.Description).HasColumnType("NVARCHAR(500)");
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.MainTableId).IsRequired();
            builder.Property(x => x.MainAlias).IsRequired().HasDefaultValue("Main");
            builder.Property(x => x.Expression).IsRequired().HasColumnType("NVARCHAR(500)"); ;

            //设置外键关系
            builder.HasOne(x => x.MainTable)
                   .WithMany()
                   .HasForeignKey(x => x.MainTableId)
                   .OnDelete(DeleteBehavior.NoAction);

            //设置外键关系
            builder.HasMany(x => x.JoinPaths)
                   .WithOne()
                   .HasForeignKey(x => x.BusinessMetricId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class BusinessMetricJoinPathConfiguration : IEntityTypeConfiguration<BusinessMetricJoinPath>
    {
        public void Configure(EntityTypeBuilder<BusinessMetricJoinPath> builder)
        {
            // 主键
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            // 属性配置
            builder.Property(x => x.Order).IsRequired();
            builder.Property(x => x.TableId).IsRequired();
            builder.Property(x => x.Alias).HasColumnType("NVARCHAR(500)");
            builder.Property(x => x.JoinType).IsRequired().HasDefaultValue("LEFT JOIN");
            builder.Property(x => x.OnCondition).HasColumnType("NVARCHAR(500)");
 
            //设置外键关系
            builder.HasOne(x => x.Table)
                   .WithMany()
                   .HasForeignKey(x => x.TableId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
