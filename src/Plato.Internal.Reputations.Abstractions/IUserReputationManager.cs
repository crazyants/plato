using Plato.Internal.Abstractions;

namespace Plato.Internal.Reputations.Abstractions
{
    public interface IUserReputationManager<TUserReputation> : ICommandManager<TUserReputation> where TUserReputation : class
    {
    }

}
