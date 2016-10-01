using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Repositories.Models
{
    public interface IModel<T> where T : class
    {

        void BuildModelFromDataReader(IDataReader dr);

    }
}
