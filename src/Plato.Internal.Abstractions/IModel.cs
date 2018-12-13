using System.Data;

namespace Plato.Internal.Abstractions
{
    public interface IModel<T> where T : class
    {
        
        void PopulateModel(IDataReader dr);
        
    }
}
