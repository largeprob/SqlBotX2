using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using SqlBoTx.Net.Domain.BusinessMetrics;
using SqlBoTx.Net.Domain.BusinessObjectives;
using SqlBoTx.Net.Domain.DatabaseConnections;
using SqlBoTx.Net.Domain.TableStructures;
using System;

namespace SqlBoTx.Net.Domain
{
    public static class DomainExtensions
    {
        public static IServiceCollection AddDomainManagers(this IServiceCollection services)
        {

          
            services.AddScoped<DatabaseConnectionManager>();
            services.AddScoped<TableStructureManager>();
            services.AddScoped<BusinessObjectiveManager>();
            services.AddScoped<BusinessMetricManager>();

            return services;
        }
    }
}
