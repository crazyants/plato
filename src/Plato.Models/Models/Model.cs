using System.Data;

namespace Plato.Models
{
    public class Model<T> : IModel<T> where T : class
    {
        public virtual void PopulateModelFromDataReader(IDataReader dr)
        {        
        }

    }
}
