using Plato.Internal.Repositories;

namespace Plato.Mentions.Repositories
{
    public interface IEntityMentionsRepository<T> : IRepository<T> where T : class
    {

    }

}
