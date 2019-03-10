using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Entities.Models;

namespace Plato.Entities.Services
{

    public interface IEntityViewIncrementer<TEntity> where TEntity : class, IEntity
    {

        IEntityViewIncrementer<TEntity> Contextulize(HttpContext context);

        Task<TEntity> IncrementAsync(TEntity entity);

    }

}
