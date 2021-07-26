using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace YS.Knife.Data.Query.Expressions
{
    class ObjectMemberVisitor<T> : IMemberVisitor
    {
        static readonly Dictionary<string, IFilterMemberInfo> AllMembers = new Dictionary<string, IFilterMemberInfo>(StringComparer.InvariantCultureIgnoreCase);

        static ObjectMemberVisitor()
        {
            // if some member name equal when ignore case, next will over the pre one
            foreach (var field in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public))
            {

                AllMembers[field.Name] = new ObjectFieldFilterMemberInfo(typeof(T), field);
            }
            foreach (var property in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (property.GetIndexParameters().Length == 0)
                {
                    AllMembers[property.Name] = new ObjectPropertyFilterMemberInfo(typeof(T), property);
                }

            }
        }

        public Type CurrentType => typeof(T);

        public IFilterMemberInfo GetSubMemberInfo(string memberName)
        {
            if (AllMembers.TryGetValue(memberName, out var filterMember))
            {
                return filterMember;
            }
            return default;
        }

        class ObjectPropertyFilterMemberInfo : IFilterMemberInfo
        {
            private readonly PropertyInfo propertyInfo;

            public ObjectPropertyFilterMemberInfo(Type hostType, PropertyInfo propertyInfo)
            {
                this.propertyInfo = propertyInfo;
                var param0 = Expression.Parameter(hostType);
                this.SelectExpression = Expression.Lambda(Expression.Property(param0, propertyInfo), param0);
            }
            public Type ExpressionValueType => propertyInfo.PropertyType;

            public LambdaExpression SelectExpression { get; }

            public IMemberVisitor SubProvider { get => IMemberVisitor.GetObjectVisitor(ExpressionValueType); }
        }
        class ObjectFieldFilterMemberInfo : IFilterMemberInfo
        {
            private readonly FieldInfo fieldInfo;

            public ObjectFieldFilterMemberInfo(Type hostType, FieldInfo fieldInfo)
            {
                this.fieldInfo = fieldInfo;
                var param0 = Expression.Parameter(hostType);
                this.SelectExpression = Expression.Lambda(Expression.Field(param0, fieldInfo), param0);
            }
            public Type ExpressionValueType => fieldInfo.FieldType;

            public LambdaExpression SelectExpression { get; }

            public IMemberVisitor SubProvider { get => IMemberVisitor.GetObjectVisitor(ExpressionValueType); }
        }
    }
}
