using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SqlBoTx.Net.Domain;
using SqlBoTx.Net.Domain.BusinessMetrics;
using SqlBoTx.Net.Domain.BusinessObjectives;
using SqlBoTx.Net.Domain.DatabaseConnections;
using SqlBoTx.Net.Domain.TableRelationships;
using SqlBoTx.Net.Domain.TableStructures;
using SqlBoTx.Net.EFCore.Repositorys;
using SqlBoTx.Net.Share.Exceptions;

namespace SqlBoTx.Net.EFCore
{
    public static class EFCoreExtensions
    {
        public static IServiceCollection AddEFCore(this IServiceCollection services, IConfiguration configuration)
        {
            string connectStr = configuration.GetConnectionString("SqlBotx").ThrowIfNull();
            services.AddDbContextPool<SqlBotxDBContext>((o) =>
            {
                o.UseSqlServer(connectStr);
                //var loggerFactory = new LoggerFactory();
                //o.UseLoggerFactory(loggerFactory).EnableSensitiveDataLogging();
            }, 60);

            services.AddEFCoreRepository();

            return services;
        }

        private static IServiceCollection AddEFCoreRepository(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDatabaseConnectionRepository, DatabaseConnectionRepository>();
            services.AddScoped<ITableStructureRepository, TableStructureRepository>();
            services.AddScoped<IBusinessObjectiveRepository, BusinessObjectiveRepository>();
            services.AddScoped<ITableRelationshipRepository, TableRelationshipRepository>();
            services.AddScoped<IBusinessMetricRepository, BusinessMetricRepository>();
            return services;
        }
    }
}
