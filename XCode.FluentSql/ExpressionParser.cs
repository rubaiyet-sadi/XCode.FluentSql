using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using XCode.FluentSql.DbConvension;

namespace XCode.FluentSql
{
    internal class ExpressionParser
    {
        private readonly BaseConvension _dbConvention;

        public ExpressionParser(BaseConvension dbConvension)
        {
            _dbConvention = dbConvension;
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
                                              _dbConvention.ToValueNameConvention(GetExpressionValue(binaryExpression.Right));
                        builder.Add(prdicateString);
                    }
                    break;
                case ExpressionType.Convert:
                    expression = ((UnaryExpression)expression).Operand;
                    break;
                case ExpressionType.MemberAccess:
                    //MemberExpression memberExpression = (MemberExpression)expression;
                    //MemberInfo mi = memberExpression.Member;
                    break; ;
                default:
                    break;
            }
            return;
        }

        private string GetExpressionString(Expression expression)
        {
            var lambdaExpression = expression as LambdaExpression;
            if (lambdaExpression != null)
            {
                var dynamicObject = lambdaExpression.Compile().DynamicInvoke();//.GetType();
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
            var memberExpression = expression as MemberExpression;
            if (memberExpression != null)
            {
                var objectMember = Expression.Convert(memberExpression, typeof(object));

                var getterLambda = Expression.Lambda<Func<object>>(objectMember);

                var getter = getterLambda.Compile();

                return getter().ToString();
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
                    break;
                case ExpressionType.GreaterThan:
                    return ">";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                    break;
                case ExpressionType.LessThan:
                    return "<";
                    break;
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                    break;
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
