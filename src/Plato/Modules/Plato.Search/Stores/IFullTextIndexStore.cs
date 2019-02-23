using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Search.Models;

namespace Plato.Search.Stores
{
    public interface IFullTextIndexStore
    {
        Task<IEnumerable<FullTextIndex>> SelectIndexesAsync();
    }

}
