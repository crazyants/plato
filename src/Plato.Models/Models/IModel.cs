using System.Data;

namespace Plato.Models
{
    public interface IModel<T> where T : class
    {

        void PopulateModel(IDataReader dr);

    }
}
