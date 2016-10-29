using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Text;

namespace Plato.Data.Query
{

    // we really don't want to implement linq or parse expression trees
    // for performance reasons so have opted to implement or own very 
    // simple dynamic query builder


    #region "IQuery"


    public interface IQuery
    {

        IQuery Page(int pageIndex, int pageSize);

        IQuery Define<T>(Action<T> action) where T : class;

        Task<IEnumerable<T>> ToListAsync<T>() where T : class;
   

    }

    #endregion

    #region "DefaultQuery"

    public class DefaultQuery : IQuery
    {
     
        private int _pageIndex;
        private int _pageSize;

        public IQuery Page(int pageIndex, int pageSize)
        {
            _pageIndex = pageIndex;
            _pageSize = pageSize;
            return this;
        }
        

        public virtual Task<IEnumerable<T>> ToListAsync<T>() where T : class
        {
            return null;
        }
        
        public virtual IQuery Define<T>(Action<T> action) where T : class
        {
         
            return this;
        }

    }
    
    
    public enum QuerySortOrder
    {
        Asc,
        Desc
    }

    #endregion
    

}
