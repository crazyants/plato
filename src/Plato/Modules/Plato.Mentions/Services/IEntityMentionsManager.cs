using Plato.Internal.Abstractions;

namespace Plato.Mentions.Services
{

    public interface IEntityMentionsManager<TEntityMention> : ICommandManager<TEntityMention> where TEntityMention : class
    {

    }
    
}
