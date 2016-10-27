using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Text;
using System.Data;
using System.Data.Common;
using Plato.Data.Query.Dialect;

namespace Plato.Data.Query
{
    public class DefaultQuery : IQuery
    {

        private readonly DbConnection _connection;
        private readonly ISqlDialect _dialect;
        private readonly DbTransaction _transaction;
        private readonly IDataProvider _session;

        private List<Type> _bound = new List<Type>();
        private string _lastParameterName;
        private SqlBuilder _sqlBuilder;

        public DefaultQuery(
         DbConnection connection,
         DbTransaction transaction,
         IDataProvider session,
         string tablePrefix)
        {
            
            _connection = connection;
            _transaction = transaction;
            _session = session;
            _dialect = SqlDialectFactory.For(connection);
            _sqlBuilder = new SqlBuilder {TablePrefix = tablePrefix};
        }


        public static Dictionary<MethodInfo, Action<DefaultQuery, StringBuilder, MethodCallExpression>> MethodMappings =
            new Dictionary<MethodInfo, Action<DefaultQuery, StringBuilder, MethodCallExpression>>();

        public void ConvertFragment(StringBuilder builder, Expression expression)
        {

            if (!IsParameterBased(expression))
            {
                var value = Expression.Lambda(expression).Compile().DynamicInvoke();
                expression = Expression.Constant(value);
            }

            switch (expression.NodeType)
            {
                case ExpressionType.LessThan:
                    ConvertComparisonBinaryExpression(builder, (BinaryExpression) expression, " < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    ConvertComparisonBinaryExpression(builder, (BinaryExpression) expression, " <= ");
                    break;
                case ExpressionType.GreaterThan:
                    ConvertComparisonBinaryExpression(builder, (BinaryExpression) expression, " > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    ConvertComparisonBinaryExpression(builder, (BinaryExpression) expression, " >= ");
                    break;
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    ConvertEqualityBinaryExpression(builder, (BinaryExpression) expression, " and ");
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    ConvertEqualityBinaryExpression(builder, (BinaryExpression) expression, " or ");
                    break;
                case ExpressionType.Equal:
                    ConvertComparisonBinaryExpression(builder, (BinaryExpression) expression, " = ");
                    break;
                case ExpressionType.NotEqual:
                    ConvertComparisonBinaryExpression(builder, (BinaryExpression) expression, " <> ");
                    break;
                case ExpressionType.IsTrue:
                    ConvertFragment(builder, ((UnaryExpression) expression).Operand);
                    break;
                case ExpressionType.IsFalse:
                    builder.Append(" not ");
                    ConvertFragment(builder, ((UnaryExpression) expression).Operand);
                    break;
                case ExpressionType.MemberAccess:
                    var memberExpression = (MemberExpression) expression;
                    builder.Append(_sqlBuilder.FormatColumn(_bound.Last().Name, memberExpression.Member.Name));
                    break;
                case ExpressionType.Constant:
                    _lastParameterName = "@p" + _sqlBuilder.Parameters.Count.ToString();
                    _sqlBuilder.Parameters.Add(_lastParameterName, ((ConstantExpression) expression).Value);
                    builder.Append(_lastParameterName);
                    break;
                case ExpressionType.Call:
                    var methodCallExpression = (MethodCallExpression) expression;
                    var methodInfo = methodCallExpression.Method;
                    Action<DefaultQuery, StringBuilder, MethodCallExpression> action;
                    if (MethodMappings.TryGetValue(methodInfo, out action))
                    {
                        action(this, builder, methodCallExpression);
                    }
                    else
                    {
                        throw new ArgumentException("Not supported method: " + methodInfo.Name);
                    }
                    break;
                default:
                    throw new ArgumentException("Not supported expression: " + expression);
            }

        }

        public void ConvertComparisonBinaryExpression(StringBuilder builder, BinaryExpression expression, string operation)
        {
            builder.Append("(");
            ConvertFragment(builder, expression.Left);
            builder.Append(operation);
            ConvertFragment(builder, expression.Right);
            builder.Append(")");
        }


        public bool IsParameterBased(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Parameter:
                
                    return true;
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.And:
                case ExpressionType.Or:
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    var binaryExpression = (BinaryExpression)expression;
                    return IsParameterBased(binaryExpression.Left) || IsParameterBased(binaryExpression.Right);
                case ExpressionType.IsTrue:
                case ExpressionType.IsFalse:
                case ExpressionType.Constant:
                    return false;
                case ExpressionType.Call:
                    var methodCallExpression = (MethodCallExpression)expression;

                    if (methodCallExpression.Object == null)
                    {
                        // Static call
                        return IsParameterBased(methodCallExpression.Arguments[0]);
                    }

                    return IsParameterBased(methodCallExpression.Object);
                case ExpressionType.MemberAccess:
                    var memberExpression = (MemberExpression)expression;

                    if (memberExpression.Expression == null)
                    {
                        // Static method
                        return false;
                    }

                    return IsParameterBased(memberExpression.Expression);
                default:
                    return false;
            }
        }

        public void ConvertEqualityBinaryExpression(StringBuilder builder, BinaryExpression expression, string operation)
        {
            builder.Append("(");
            ConvertPredicate(builder, expression.Left);
            builder.Append(operation);
            ConvertPredicate(builder, expression.Right);
            builder.Append(")");
        }

        public void ConvertPredicate(StringBuilder builder, Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    ConvertFragment(builder, expression);
                    break;
                case ExpressionType.Call:
                    // A method call is something like x.Name.StartsWith("B") thus it is supposed
                    // to be part of the method mappings
                    ConvertFragment(builder, expression);
                    break;
                case ExpressionType.MemberAccess:
                    ConvertFragment(builder, Expression.Equal(expression, Expression.Constant(true, typeof(bool))));
                    break;
                case ExpressionType.Constant:
                    ConvertFragment(builder, Expression.Equal(expression, Expression.Constant(true, typeof(bool))));
                    break;
                default:
                    throw new ArgumentException("Not supported expression: " + expression);
            }
        }


    }
}
