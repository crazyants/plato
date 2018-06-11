
namespace Plato.Internal.Data.Abstractions
{
    public interface IDbQuery
    {
        IQuery ConfigureQuery(IQuery query);
    }

}
