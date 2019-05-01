using Plato.Internal.Abstractions;

namespace Plato.Issues.Services
{
    public interface IPostManager<TEntity> : ICommandManager<TEntity> where TEntity : class
    {
    }

}
