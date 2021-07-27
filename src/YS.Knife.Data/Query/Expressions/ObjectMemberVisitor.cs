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
            LoadTypeMembers(typeof(T), false);

            if (typeof(T).IsNullableType())
            {
                LoadTypeMembers(Nullable.GetUnderlyingType(typeof(T)), true);
            }

            static void LoadTypeMembers(Type hostType, bool fromNullableValue)
            {
                foreach (var field in hostType.GetFields(BindingFlags.Instance | BindingFlags.Public))
                {

                    AllMembers[field.Name] = new ObjectFieldFilterMemberInfo(typeof(T), field, fromNullableValue);
                }
                foreach (var property in hostType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (property.GetIndexParameters().Length == 0)
                    {
                        AllMembers[property.Name] = new ObjectPropertyFilterMemberInfo(typeof(T), property, fromNullableValue);
                    }

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

            public ObjectPropertyFilterMemberInfo(Type hostType, PropertyInfo propertyInfo, bool fromNullableValue)
            {
                this.propertyInfo = propertyInfo;
                var param0 = Expression.Parameter(hostType);
                Expression expression = fromNullableValue ? Expression.Property(param0, "Value") : param0;
                this.SelectExpression = Expression.Lambda(Expression.Property(expression, propertyInfo), param0);
            }
            public Type ExpressionValueType => propertyInfo.PropertyType;

            public LambdaExpression SelectExpression { get; }

            public IMemberVisitor SubProvider { get => IMemberVisitor.GetObjectVisitor(ExpressionValueType); }
        }
        class ObjectFieldFilterMemberInfo : IFilterMemberInfo
        {
            private readonly FieldInfo fieldInfo;

            public ObjectFieldFilterMemberInfo(Type hostType, FieldInfo fieldInfo, bool fromNullableValue)
            {
                this.fieldInfo = fieldInfo;
                var param0 = Expression.Parameter(hostType);
                Expression expression = fromNullableValue ? Expression.Property(param0, "Value") : param0;
                this.SelectExpression = Expression.Lambda(Expression.Field(expression, fieldInfo), param0);
            }
            public Type ExpressionValueType => fieldInfo.FieldType;

            public LambdaExpression SelectExpression { get; }

            public IMemberVisitor SubProvider { get => IMemberVisitor.GetObjectVisitor(ExpressionValueType); }
        }
    }
}
