using Plato.Internal.Abstractions;

namespace Plato.Entities.Labels.Services
{
    public interface IEntityLabelManager<TEntityLabel> : ICommandManager<TEntityLabel> where TEntityLabel : class
    {

    }

}
