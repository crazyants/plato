using Plato.Internal.Abstractions;

namespace Plato.Search.Commands
{
    public interface IFullTextCatalogCommand<TCatalog> : ICommandManager<TCatalog> where TCatalog : class
    {
    }
}
