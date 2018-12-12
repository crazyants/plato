using Plato.Internal.Abstractions;

namespace Plato.Reputations.Services
{
    public interface IUserReputationManager<TUserReputation> : ICommandManager<TUserReputation> where TUserReputation : class
    {
    }

}
