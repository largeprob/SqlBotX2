using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlBoTx.Net.Domain.BusinessObjectives;
using System.Reflection.Emit;

namespace SqlBoTx.Net.EFCore.Mappings
{
    public class BusinessObjectiveConfiguration : IEntityTypeConfiguration<BusinessObjective>
    {
        public void Configure(EntityTypeBuilder<BusinessObjective> builder)
        {
            // 主键
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            // 属性配置
            builder.Property(x => x.BusinessName).IsRequired().HasColumnType("NVARCHAR(255)");
            builder.Property(x => x.Synonyms).IsRequired(false).HasColumnType("NVARCHAR(500)");
            builder.Property(x => x.Description).IsRequired(false).HasColumnType("NVARCHAR(500)");
            builder.Property(x => x.CreatedDate).IsRequired();
            builder.Property(x => x.UpdatedDate).IsRequired(false);

            //从属属性
            builder.OwnsMany(x => x.DependencyTables,
                dt =>
                {
                    dt.ToTable("business_objective_dependency_table");

                    dt.Property<int>("BusinessObjectiveId").HasColumnName("business_objective_id").IsRequired();
                    dt.WithOwner().HasForeignKey("BusinessObjectiveId");

                    dt.Property(p => p.TableId).HasColumnName("table_id").ValueGeneratedNever().IsRequired();

                    dt.HasKey("BusinessObjectiveId", "TableId");
                }
            );

            //设置外键关系
            builder.HasMany(x => x.Fields)
                   .WithOne(x => x.BusinessObjective)
                   .HasForeignKey(x => x.BusinessObjectiveId)
                   .OnDelete(DeleteBehavior.Cascade);

            //设置外键关系
            builder.HasMany(x => x.Metrics)
                   .WithOne(x => x.BusinessObjective)
                   .HasForeignKey(x => x.BusinessObjectiveId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }


    public class BusinessObjectivesFieldConfiguration : IEntityTypeConfiguration<BusinessObjectiveField>
    {
        public void Configure(EntityTypeBuilder<BusinessObjectiveField> builder)
        {
            // 主键
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            // 属性配置
            builder.Property(x => x.FieldId).IsRequired();
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Description);
            builder.Property(x => x.BusinesBIRole);
            builder.Property(x => x.DimensionLayer);
            builder.Property(x => x.MetricLayer);

            //设置外键关系
            builder.HasOne(x => x.TableField)
                   .WithMany()
                   .HasForeignKey(x => x.FieldId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
