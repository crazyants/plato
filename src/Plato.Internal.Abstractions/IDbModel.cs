using System.Data;

namespace Plato.Internal.Abstractions
{
    
    public interface IDbModel<T> where T : class
    {
        
        void PopulateModel(IDataReader dr);
        
    }
}
