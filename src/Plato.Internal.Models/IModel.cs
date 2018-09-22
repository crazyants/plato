using System.Data;

namespace Plato.Internal.Models
{
    public interface IModel<T> where T : class
    {
        
        void PopulateModel(IDataReader dr);
        
    }
}
