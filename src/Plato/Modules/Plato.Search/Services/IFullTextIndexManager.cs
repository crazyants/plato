using Plato.Internal.Abstractions;

namespace Plato.Search.Services
{
    public interface IFullTextIndexManager<TIndex> : ICommandManager<TIndex> where TIndex : class
    {
    }
}
