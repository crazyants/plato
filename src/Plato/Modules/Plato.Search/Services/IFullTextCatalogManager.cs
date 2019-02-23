using Plato.Internal.Abstractions;

namespace Plato.Search.Services
{
    public interface IFullTextCatalogManager<TCatalog> : ICommandManager<TCatalog> where TCatalog : class
    {
    }
}
