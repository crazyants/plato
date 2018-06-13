using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Models.Abstract;

namespace Plato.Internal.Repositories.Abstract
{
    public interface IDocumentRepository<T> : IRepository<T> where T : class
    {
    
    }

}
