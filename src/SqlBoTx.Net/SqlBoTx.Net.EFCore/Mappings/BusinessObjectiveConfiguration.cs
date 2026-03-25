using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlBoTx.Net.Domain.BusinessObjectives;
using System.Reflection.Emit;
using System.Text.Json;

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
            builder.Property(x => x.ParentId);
            builder.Property(x => x.BusinessName).IsRequired().HasColumnType("NVARCHAR(255)");
            builder.Property(x => x.Synonyms).HasColumnType("NVARCHAR(500)");
            builder.Property(x => x.Description).HasColumnType("NVARCHAR(500)");
            builder.Property(x => x.Summary).HasColumnType("NVARCHAR(500)");
            builder.Property(x => x.KeyWords).HasColumnType("NVARCHAR(500)");
            builder.Property(x => x.Tags).HasColumnType("NVARCHAR(500)").HasConversion(
              //存
              v => string.Join(',', v ?? Array.Empty<string>()),
              //取
              v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
           );
            builder.Property(x => x.CreatedDate).IsRequired();
            builder.Property(x => x.UpdatedDate);

            builder.HasOne(x => x.Parent)
                   .WithMany(x => x.Children)
                   .HasForeignKey(x => x.ParentId)
                   .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
