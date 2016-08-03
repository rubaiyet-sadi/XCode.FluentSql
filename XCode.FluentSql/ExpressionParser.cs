using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using XCode.FluentSql.DbConvension;
using XCode.FluentSql.Extensions;

namespace XCode.FluentSql
{
    internal class ExpressionParser
    {
        private readonly BaseConvension _dbConvention;
        private readonly bool _getValueFromRhf;

        public ExpressionParser(BaseConvension dbConvension)
        {
            _dbConvention = dbConvension;
        }

        public ExpressionParser(BaseConvension dbConvension, bool getValueFromRhf) : this(dbConvension)
        {
            _getValueFromRhf = getValueFromRhf;
        }

        public IList<string> ParsePredicate(Expression expression)
        {
            IList<string> builder = new List<string>();
            TreeWalker(expression, ref builder);
            return builder;
        } 

        private void TreeWalker(Expression expression, ref IList<string> builder)
        {
            if (expression == null) return;

            BinaryExpression binaryExpression;
            switch (expression.NodeType)
            {
                case ExpressionType.Lambda:
                    TreeWalker((expression as LambdaExpression).Body, ref builder);
                    break;
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                    binaryExpression = expression as BinaryExpression;
                    if (binaryExpression != null)
                    {
                        TreeWalker(binaryExpression.Left, ref builder);
                        builder.Add(GetOperator(expression.NodeType));
                        TreeWalker(binaryExpression.Right, ref builder);
                    }
                    break;
                case ExpressionType.Conditional:
                case ExpressionType.Equal:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    binaryExpression = expression as BinaryExpression;

                    if (binaryExpression != null)
                    {
                        var prdicateString = GetExpressionString(binaryExpression.Left) +
                                              GetOperator(expression.NodeType) +
                                              (_getValueFromRhf 
                                                ? _dbConvention.ToValueNameConvention(GetExpressionValue(binaryExpression.Right))
                                                : GetExpressionString(binaryExpression.Right));
                        builder.Add(prdicateString);
                    }
                    break;
                case ExpressionType.Convert:
                    expression = ((UnaryExpression)expression).Operand;
                    break;
                case ExpressionType.Call:
                    builder.Add(ProcessMethodCall(expression));
                    break;
                default:
                    break;
            }
            return;
        }

        private string ProcessMethodCall(Expression expression)
        {

            MethodCallExpression methodCallExpression = expression as MethodCallExpression;

            var caller = methodCallExpression.Object;

            var arguments = methodCallExpression.Arguments;

            return methodCallExpression != null ? methodCallExpression.Parse(_dbConvention) : string.Empty;
        }

        private string GetExpressionString(Expression expression)
        {
            var lambdaExpression = expression as LambdaExpression;
            if (lambdaExpression != null)
            {
                var dynamicObject = lambdaExpression.Compile().DynamicInvoke();
                if (IsSimpleType(dynamicObject.GetType()))
                {
                    _dbConvention.ToValueNameConvention(dynamicObject.ToString());
                }
            }
            
            var memberExpression = expression as MemberExpression;
            if (memberExpression != null)
            {
               
                //if (memberExpression.Expression.Type.ReflectedType != null)
                var quntifiedName = IsSimpleType(memberExpression.Expression.Type.BaseType) ?
                        _dbConvention.ToValueNameConvention(memberExpression.Expression.Type.Name):
                        _dbConvention.ToQuantifiedNameConvention(memberExpression.Expression.Type.Name,
                    memberExpression.Member.Name);
                return quntifiedName;
            }
            var constantExpression = expression as ConstantExpression;
            if (constantExpression != null)
                return _dbConvention.ToValueNameConvention(constantExpression.Value.ToString());
            var methodCallExpression = expression as MethodCallExpression;
            if (methodCallExpression != null)
            {
                var dynamicValue = Expression.Lambda(expression).Compile().DynamicInvoke().ToString();
                return _dbConvention.ToValueNameConvention(dynamicValue);
            }
            return string.Empty;
        }

        private string GetExpressionValue(Expression expression)
        {
            var methodCallExpression = expression as MethodCallExpression;

            var memberExpression = expression as MemberExpression;
            if (memberExpression != null)
            {
                return memberExpression.GetValue<object>().ToString();
            }
            var constantExpression = expression as ConstantExpression;

            if(constantExpression != null)
            {
                return constantExpression.Value.ToString();
            }
            throw new InvalidOperationException("");
        }

        private string GetOperator(ExpressionType expressionType)
        {
            switch (expressionType)
            {
                case ExpressionType.AndAlso:
                    return "AND";
                case ExpressionType.OrElse:
                    return "OR";
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                default:
                    return string.Empty;
            }

            
            }

        private static bool IsSimpleType(Type type)
        {

            var simpleTypeList = new List<Type>
            {
                typeof (byte),
                typeof (sbyte),
                typeof (short),
                typeof (ushort),
                typeof (int),
                typeof (uint),
                typeof (long),
                typeof (ulong),
                typeof (float),
                typeof (double),
                typeof (decimal),
                typeof (bool),
                typeof (string),
                typeof (char),
                typeof (Guid),
                typeof (DateTime),
                typeof (DateTimeOffset),
                typeof (byte[])
            };
            return simpleTypeList.Exists(t => t == type);
        }
    }
}
