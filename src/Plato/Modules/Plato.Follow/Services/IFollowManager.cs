using Plato.Internal.Abstractions;

namespace Plato.Follow.Services
{
    public interface IFollowManager<TFollow> : ICommandManager<TFollow> where TFollow : class
    {
    }

}
