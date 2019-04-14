using System.Threading.Tasks;
using Plato.Entities.Services;
using Plato.Internal.Abstractions;

namespace Plato.Docs.Services
{
    public interface IPostManager<TEntity> : ICommandManager<TEntity> where TEntity : class
    {
        Task<ICommandResult<TEntity>> Move(TEntity model, MoveDirection direction);

    }

}
