using Plato.Internal.Abstractions;

namespace Plato.Tags.Services
{
    public interface IEntityTagManager<TEntityLabel> : ICommandManager<TEntityLabel> where TEntityLabel : class
    {
    }


}
