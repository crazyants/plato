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


    public interface IQuery
    {
   
        IQuery Page(int pageIndex, int pageSize);

        IQuery Where(string sql);

        IQuery Where<T>(Expression<Func<T, bool>> predicate);

        Task<IEnumerable<T>> ToListAsync<T>() where T : class;
        
        List<QueryExpression> Expressions { get; }

    }

    public class DefaultQuery : IQuery
    {

        private readonly StringBuilder _builder;
     
        public DefaultQuery()
        {
            _builder = new StringBuilder();
        }
        
        private int _pageIndex;
        private int _pageSize;

        public IQuery Page(int pageIndex, int pageSize)
        {
            _pageIndex = pageIndex;
            _pageSize = pageSize;
            return this;
        }

        public IQuery Where(string sql)
        {

            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" AND ");
            _builder.Append(sql);
            return this;

        }

        public IQuery Where<T>(Expression<Func<T, bool>> predicate)
        {

            dynamic operation = predicate.Body;
            dynamic left = operation.Left;
            dynamic right = operation.Right;

            var ops = new Dictionary<ExpressionType, string>();
            ops.Add(ExpressionType.Equal, "=");
            ops.Add(ExpressionType.GreaterThan, ">");
            ops.Add(ExpressionType.GreaterThanOrEqual, ">=");
            ops.Add(ExpressionType.LessThan, "<");
            ops.Add(ExpressionType.LessThanOrEqual, "<=");
            
            if (_expressions == null)
                _expressions = new List<QueryExpression>();
            
            _expressions.Add(new QueryExpression()
            {
                Name = left.Member.Name,
                ExpressionType = operation.NodeType,
                ExpressionOperator = ops[operation.NodeType],
                Value = right.Value,
            });

            return this;

        }
        
        public virtual Task<IEnumerable<T>> ToListAsync<T>() where T : class
        {
            return null;
        }

        // ---------------------
        
        public List<QueryExpression> Expressions => _expressions;

        private List<QueryExpression> _expressions;



        }
        
  
    public class QueryExpression
    {
        public string Name { get; set; }

        public ExpressionType ExpressionType { get; set; }
        
        public string ExpressionOperator { get; set; }

        public object Value { get; set; }

    }

    public enum QuerySortOrder
    {
        Asc,
        Desc
    }



}
