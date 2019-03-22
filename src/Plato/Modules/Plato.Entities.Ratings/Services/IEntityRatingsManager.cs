using Plato.Internal.Abstractions;

namespace Plato.Entities.Ratings.Services
{
    public interface IEntityRatingsManager<TReaction> : ICommandManager<TReaction> where TReaction : class
    {
    }

}
