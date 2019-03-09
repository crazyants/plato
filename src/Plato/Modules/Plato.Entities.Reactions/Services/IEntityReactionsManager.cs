using Plato.Internal.Abstractions;

namespace Plato.Entities.Reactions.Services
{
    public interface IEntityReactionsManager<TReaction> : ICommandManager<TReaction> where TReaction : class
    {
    }

}
