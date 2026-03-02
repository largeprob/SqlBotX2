using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using SqlBoTx.Net.Application.BusinessMetrics;
using SqlBoTx.Net.Application.BusinessObjectives;
using SqlBoTx.Net.Application.Contracts.BusinessMetrics;
using SqlBoTx.Net.Application.Contracts.BusinessObjectives;
using SqlBoTx.Net.Application.Contracts.DatabaseConnections;
using SqlBoTx.Net.Application.Contracts.TableStructures;
using SqlBoTx.Net.Application.DatabaseConnections;
using SqlBoTx.Net.Application.TableStructures;
using SqlBoTx.Net.Application.Vectors;

namespace SqlBoTx.Net.Application
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            



            services.AddTransient<IDatabaseConnectionService, DatabaseConnectionService>();
            services.AddTransient<ITableStructureService, TableStructureService>();
            services.AddTransient<IBusinessObjectiveService, BusinessObjectiveService>();
            services.AddTransient<ITableRelationshipService, TableRelationshipService>();
            services.AddTransient<IBusinessMetricService, BusinessMetricService>();
            services.AddTransient<QdrantVectorService, QdrantVectorService>();
            return services;
        }
    }
}
