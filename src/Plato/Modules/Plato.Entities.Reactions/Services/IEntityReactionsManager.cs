using Plato.Internal.Abstractions;

namespace Plato.Reactions.Services
{
    public interface IEntityReactionsManager<TReaction> : ICommandManager<TReaction> where TReaction : class
    {
    }

}
