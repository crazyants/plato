namespace Plato.Internal.Stores.Abstractions
{
    public interface IQueryBuilder
    {
        string BuildSqlPopulate();

        string BuildSqlCount();
    }

}
