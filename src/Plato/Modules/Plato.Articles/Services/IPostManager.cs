using Plato.Internal.Abstractions;

namespace Plato.Articles.Services
{
    public interface IPostManager<TEntity> : ICommandManager<TEntity> where TEntity : class
    {
    }

}
