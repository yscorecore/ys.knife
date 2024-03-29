﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using YS.Knife.Data.Mappers;
using YS.Knife.Data.Query;
using YS.Knife.Data.Query.Functions;

namespace YS.Knife.Data.Query.Expressions
{
    internal class PathValueLambdaProvider : IValueLambdaProvider
    {
        private readonly List<ValuePath> paths;
        private readonly IMemberVisitor memberVisitor;

        public PathValueLambdaProvider(Type sourceType, List<ValuePath> paths, IMemberVisitor memberVisitor)
        {
            this.SourceType = sourceType;
            this.paths = paths;
            this.memberVisitor = memberVisitor ?? throw new ArgumentNullException(nameof(memberVisitor));
        }
        public Type SourceType { get; }
        public LambdaExpression GetLambda(ParameterExpression parameter)
        {
            var (type, body) = CreateLambdaBody(parameter, paths, memberVisitor);
            return Expression.Lambda(typeof(Func<,>).MakeGenericType(SourceType, type), body, parameter);
        }
        public LambdaExpression GetLambda(ParameterExpression parameter, Type targetType)
        {
            var (type, body) = CreateLambdaBody(parameter, paths, memberVisitor);
            if (type == targetType)
            {
                return Expression.Lambda(typeof(Func<,>).MakeGenericType(SourceType, targetType), body, parameter);
            }
            else
            {
                var targetBody = Expression.Convert(body, targetType);
                return Expression.Lambda(typeof(Func<,>).MakeGenericType(SourceType, targetType), targetBody, parameter);
            }
        }
        private (Type ReturnType, Expression Body) CreateLambdaBody(ParameterExpression p, List<ValuePath> pathInfos, IMemberVisitor memberProvider)
        {
            IMemberVisitor currentMemberProvider = memberProvider;
            Type currentExpressionType = currentMemberProvider.CurrentType;
            Expression currentExpression = p;
            foreach (var pathInfo in pathInfos.TrimNotNull())
            {
                if (pathInfo.IsFunction)
                {
                    var functionResult = IFilterFunction.ExecuteFunction(pathInfo.Name, pathInfo.FunctionArgs, new ExecuteContext
                    {
                        CurrentExpression = currentExpression,
                        MemberVisitor = currentMemberProvider,
                        CurrentType = currentExpressionType,
                    });
                    currentExpressionType = functionResult.LambdaValueType;
                    currentExpression = currentExpression.Connect(functionResult.LambdaExpression);
                    currentMemberProvider = functionResult.MemberProvider;
                }
                else
                {
                    var memberInfo = currentMemberProvider.GetSubMemberInfo(pathInfo.Name);

                    if (memberInfo == null)
                    {
                        throw ExpressionErrors.MemberNotFound(pathInfo.Name);
                    }
                    else
                    {
                        currentExpressionType = memberInfo.ExpressionValueType;
                        currentExpression = currentExpression.Connect(memberInfo.SelectExpression);
                        currentMemberProvider = memberInfo.SubProvider;
                    }
                }
            }
            return (currentExpressionType, currentExpression);
        }


    }
}
