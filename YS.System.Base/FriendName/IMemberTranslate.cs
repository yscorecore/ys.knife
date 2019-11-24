


namespace System
{
    using System.Reflection;
    /// <summary>
    /// 表示元数据类型转换成另外一种类型的接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMemberTranslate<T>
    {
        T GetTranslateValue(MemberInfo memberInfo);
    }
}
