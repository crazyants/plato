
namespace Plato.Internal.Data.Abstractions
{
    public interface IDbQuery
    {
        IQuery ConfigureQuery(IQuery query);
    }

    public interface IDbQuery2
    {
        IQuery<T> ConfigureQuery<T>(IQuery<T> query) where T : class;
    }

}
