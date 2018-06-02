
namespace Plato.Abstractions.Query
{
    public interface IDbQuery
    {
        IQuery ConfigureQuery(IQuery query);
    }

}
