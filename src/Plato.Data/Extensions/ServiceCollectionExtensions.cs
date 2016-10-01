using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Plato.Data.Migrations;

namespace Plato.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddDataAccess(
            this IServiceCollection services)
        {

            services.AddScoped<IDbContextt, DbContext>();

            services.AddScoped<IDataMigrationManager, DataMigrationManager>();
            services.AddScoped<AutomaticDataMigrations>();

            return services;
        }


    }
}
