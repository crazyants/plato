using System;
using System.Data;

namespace Plato.Internal.Models
{
    public class Model<T> : IModel<T> where T : class
    {
        public void PopulateModel(Action<T> action)
        {
        }

        public virtual void PopulateModel(IDataReader dr)
        {        
        }

    }
}
