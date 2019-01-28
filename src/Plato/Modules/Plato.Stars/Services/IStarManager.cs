using Plato.Internal.Abstractions;

namespace Plato.Stars.Services
{
    public interface IStarManager<TFollow> : ICommandManager<TFollow> where TFollow : class
    {
    }

}
