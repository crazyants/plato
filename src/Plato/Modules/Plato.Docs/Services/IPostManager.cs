using Plato.Internal.Abstractions;

namespace Plato.Docs.Services
{
    public interface IPostManager<TEntity> : ICommandManager<TEntity> where TEntity : class
    {
    }

}
