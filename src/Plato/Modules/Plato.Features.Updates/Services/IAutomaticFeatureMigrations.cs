using System.Threading.Tasks;
using Plato.Internal.Abstractions;

namespace Plato.Features.Updates.Services
{
    public interface IAutomaticFeatureMigrations
    {
        Task<ICommandResultBase> InitialMigrationsAsync();
    }

}
