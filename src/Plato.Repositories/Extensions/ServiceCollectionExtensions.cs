using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Plato.Repositories;
using Plato.Repositories.Models;

namespace Plato.Repositories.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddRepositories(
            this IServiceCollection services)
        {

            services.AddScoped<IUserRepository<User>, UserRepository>();

            return services;

        }
     

    }
}
