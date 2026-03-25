using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SqlBoTx.Net.Domain;
using SqlBoTx.Net.Domain.BusinessEntities;
using SqlBoTx.Net.Domain.BusinessMetrics;
using SqlBoTx.Net.Domain.BusinessObjectives;
using SqlBoTx.Net.Domain.DatabaseConnections;
using SqlBoTx.Net.Domain.DomainTerms;
using SqlBoTx.Net.Domain.TableRelationships;
using SqlBoTx.Net.Domain.TableStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Wolverine.EntityFrameworkCore;

namespace SqlBoTx.Net.EFCore
{
    public class SqlBotxDBContext : ApplicationDbContextBase
    {
        private ILogger<SqlBotxDBContext> _logger;

        public SqlBotxDBContext(DbContextOptions contextOptions, IConfiguration configuration, ILogger<SqlBotxDBContext> logger) : base(contextOptions, configuration)
        {
            _logger = logger;
        }
        public DbSet<DatabaseConnection> DatabaseConnection { get; set; }
        public DbSet<TableStructure> TableStructure { get; set; }
        public DbSet<TableStructureColumn> TableStructureColumn { get; set; }
        public DbSet<TableRelationship> TableRelationship { get; set; }


        public DbSet<BusinessObjective> BusinessDomain { get; set; }
        public DbSet<DomainEntity> DomainEntity { get; set; }
        public DbSet<DomainEntityAttr> DomainEntityAttr { get; set; }
        public DbSet<DomainEntityRel> DomainEntityRel { get; set; }
        public DbSet<DomainMetric> DomainMetric { get; set; }
        public DbSet<MetricFieldPlaceholder> MetricFieldPlaceholder { get; set; }
        public DbSet<DomainTerm> DomainTerm { get; set; }

        /// <summary>
        /// 注册模型
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entityTypes = this.GetType().GetProperties()
                .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .Select(p => p.PropertyType.GetGenericArguments()[0])
                .ToList();

            foreach (var entityType in entityTypes)
            {
                EntityTypeBuilder? typeBuilder = modelBuilder.Entity(entityType);
                try
                {
                    base.BuilderTable(typeBuilder, entityType);
                }
                catch (Exception ex)
                {
                    throw new Exception("表映射失败" + ex);
                }
                try
                {
                    base.BuilderProperty(typeBuilder, entityType);
                }
                catch (Exception ex)
                {
                    throw new Exception("列映射失败" + ex);
                }
            }
            try
            {
                base.OnModelCreating(modelBuilder);
                // 关键代码：自动扫描当前程序集（EFCore 层）中所有的配置类
                modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                throw new Exception("创建映射失败" + ex);
            }

            try
            {
                modelBuilder.MapWolverineEnvelopeStorage();
            }
            catch (Exception ex)
            {
                throw new Exception("创建 Wolverine 事件总线 失败" + ex);
            }

        }
    }
   
}
