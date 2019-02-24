using System.Threading.Tasks;
using Plato.Internal.Abstractions;

namespace Plato.Search.Services
{
    public interface IFullTextCatalogManager
    {
        Task<ICommandResultBase> CreateCatalogAsync();

        Task<ICommandResultBase> DropCatalogAsync();

        Task<ICommandResultBase> RebuildCatalogAsync();

    }

}
