using Plato.Internal.Abstractions;

namespace Plato.Discuss.Services
{
    public interface IPostManager<TEntity> : ICommandManager<TEntity> where TEntity : class
    {
    }

}
