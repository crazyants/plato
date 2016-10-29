using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Data.Query
{


    public interface IQueryBuilder
    {

        int PageIndex { get; }

        int PageSize { get; }

        string BuildSqlStartId();

        string BuildSqlPopulate();

        string BuildSqlCount();


    }
    
    public abstract class DefaultQueryBuilder : IQueryBuilder
    {
      
        public virtual int PageIndex { get; } = 1;

        public virtual int PageSize { get; } = 10;

        public abstract string BuildSqlStartId();
        
        public abstract string BuildSqlPopulate();

        public abstract string BuildSqlCount();
        
    }

}
