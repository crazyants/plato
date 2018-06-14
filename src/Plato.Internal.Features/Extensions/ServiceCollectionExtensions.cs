using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Shell;
using Plato.Internal.Stores.Abstractions.Shell;

namespace Plato.Internal.Features.Extensions
{
    public static class ServiceCollectionExtensions
    {
   
        public static IServiceCollection AddPlatoShellFeatures(
            this IServiceCollection services)
        {
         
            return services;

        }





    }
}
