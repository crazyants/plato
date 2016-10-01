using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Repositories.Models
{
    public class Model<T> : IModel<T> where T : class
    {
        public virtual void PopulateModelFromDataReader(IDataReader dr)
        {
        
        }
    }
}
