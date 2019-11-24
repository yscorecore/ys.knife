using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace System
{
    /// <summary>
    /// 提供枚举类型的辅助功能
    /// </summary>
    public static class EnumEx
    {
        /// <summary>
        /// 判断给定的枚举中是否包含指定的枚举值。
        /// </summary>
        /// <typeparam name="EnumT"></typeparam>
        /// <param name="item"></param>
        /// <param name="flagValue"></param>
        /// <returns></returns>
        public static bool HasFlagValue<EnumT>(this Enum item,EnumT flagValue)
        {
            return Enum.Equals(CombinFlagValues<EnumT>(item,flagValue),item);
        }
        /// <summary>
        /// 判断给定的枚举中是否包含指定的枚举值。
        /// </summary>
        /// <param name="item"></param>
        /// <param name="flagValue"></param>
        /// <returns></returns>
        public static bool HasFlagValue(this Enum item,Enum flagValue)
        {
            return Enum.Equals(CombinFlagValues<Enum>(item,flagValue),item);
        }
        /// <summary>
        /// 合并指定的枚举值
        /// </summary>
        /// <typeparam name="EnumT"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static EnumT CombinFlagValues<EnumT>(params EnumT[] items)
        {
            Type ty = typeof(EnumT);
            if(!ty.IsEnum) throw new ArgumentException("the item type must be a enum type");
            items = items ?? new EnumT[0];
            switch(Type.GetTypeCode(Enum.GetUnderlyingType(ty)))
            {
                case TypeCode.Byte:
                    byte val = 0;
                    for(int i = 0;i < items.Length;val |= Convert.ToByte(items[i]),i++) ;
                    return (EnumT)Enum.ToObject(ty,val);
                case TypeCode.SByte:
                    sbyte val2 = 0;
                    for(int i = 0;i < items.Length;val2 |= Convert.ToSByte(items[i]),i++) ;
                    return (EnumT)Enum.ToObject(ty,val2);
                case TypeCode.Int16:
                    short val3 = 0;
                    for(int i = 0;i < items.Length;val3 |= Convert.ToInt16(items[i]),i++) ;
                    return (EnumT)Enum.ToObject(ty,val3);
                case TypeCode.UInt16:
                    ushort val4 = 0;
                    for(int i = 0;i < items.Length;val4 |= Convert.ToUInt16(items[i]),i++) ;
                    return (EnumT)Enum.ToObject(ty,val4);
                case TypeCode.Int32:
                    int val5 = 0;
                    for(int i = 0;i < items.Length;val5 |= Convert.ToInt32(items[i]),i++) ;
                    return (EnumT)Enum.ToObject(ty,val5);
                case TypeCode.UInt32:
                    uint val6 = 0;
                    for(int i = 0;i < items.Length;val6 |= Convert.ToUInt32(items[i]),i++) ;
                    return (EnumT)Enum.ToObject(ty,val6);
                case TypeCode.Int64:
                    long val7 = 0;
                    for(int i = 0;i < items.Length;val7 |= Convert.ToInt64(items[i]),i++) ;
                    return (EnumT)Enum.ToObject(ty,val7);
                case TypeCode.UInt64:
                    ulong val8 = 0;
                    for(int i = 0;i < items.Length;val8 |= Convert.ToUInt64(items[i]),i++) ;
                    return (EnumT)Enum.ToObject(ty,val8);
                default:
                    throw new NotSupportedException("unknow the underlying type");
            }
        }
        /// <summary>
        /// 合并指定的枚举值
        /// </summary>
        /// <typeparam name="EnumT"></typeparam>
        /// <param name="item"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static EnumT CombinFlagValues<EnumT>(this Enum item,params EnumT[] items)
        {
            //items = items ?? new EnumT[0];
            //EnumT[] values = new EnumT[items.Length + 1];
            //values[0] = (EnumT)(object)item;
            //items.CopyTo(values,1);
            //return CombinFlagValues<EnumT>(values);
            Type ty = typeof(EnumT);
            if(!ty.IsEnum) throw new ArgumentException("the item type must be a enum type");
            items = items ?? new EnumT[0];
            switch(Type.GetTypeCode(Enum.GetUnderlyingType(ty)))
            {
                case TypeCode.Byte:
                    byte val = Convert.ToByte(item);
                    for(int i = 0;i < items.Length;val |= Convert.ToByte(items[i]),i++) ;
                    return (EnumT)Enum.ToObject(ty,val);
                case TypeCode.SByte:
                    sbyte val2 = Convert.ToSByte(item);
                    for(int i = 0;i < items.Length;val2 |= Convert.ToSByte(items[i]),i++) ;
                    return (EnumT)Enum.ToObject(ty,val2);
                case TypeCode.Int16:
                    short val3 = Convert.ToInt16(item);
                    for(int i = 0;i < items.Length;val3 |= Convert.ToInt16(items[i]),i++) ;
                    return (EnumT)Enum.ToObject(ty,val3);
                case TypeCode.UInt16:
                    ushort val4 = Convert.ToUInt16(item);
                    for(int i = 0;i < items.Length;val4 |= Convert.ToUInt16(items[i]),i++) ;
                    return (EnumT)Enum.ToObject(ty,val4);
                case TypeCode.Int32:
                    int val5 = Convert.ToInt32(item);
                    for(int i = 0;i < items.Length;val5 |= Convert.ToInt32(items[i]),i++) ;
                    return (EnumT)Enum.ToObject(ty,val5);
                case TypeCode.UInt32:
                    uint val6 = Convert.ToUInt32(item);
                    for(int i = 0;i < items.Length;val6 |= Convert.ToUInt32(items[i]),i++) ;
                    return (EnumT)Enum.ToObject(ty,val6);
                case TypeCode.Int64:
                    long val7 = Convert.ToInt64(item); ;
                    for(int i = 0;i < items.Length;val7 |= Convert.ToInt64(items[i]),i++) ;
                    return (EnumT)Enum.ToObject(ty,val7);
                case TypeCode.UInt64:
                    ulong val8 = Convert.ToUInt64(item);
                    for(int i = 0;i < items.Length;val8 |= Convert.ToUInt64(items[i]),i++) ;
                    return (EnumT)Enum.ToObject(ty,val8);
                default:
                    throw new NotSupportedException("unknow the underlying type");
            }
        }
        /// <summary>
        /// 从枚举剔除指定的枚举值
        /// </summary>
        /// <typeparam name="EnumT"></typeparam>
        /// <param name="item"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static EnumT RemoveFlagValues<EnumT>(this Enum item,params EnumT[] items)
        {
            Type ty = typeof(EnumT);
            if(!ty.IsEnum) throw new ArgumentException("the item type must be a enum type");
            items = items ?? new EnumT[0];
            switch(Type.GetTypeCode(Enum.GetUnderlyingType(ty)))
            {
                case TypeCode.Byte:
                    byte val = 0;
                    for(int i = 0;i < items.Length;val |= Convert.ToByte(items[i]),i++) ;
                    return (EnumT)Enum.ToObject(ty,Convert.ToByte(item) & (~val));
                case TypeCode.SByte:
                    sbyte val2 = 0;
                    for(int i = 0;i < items.Length;val2 |= Convert.ToSByte(items[i]),i++) ;
                    return (EnumT)Enum.ToObject(ty,Convert.ToSByte(item) & (~val2));
                case TypeCode.Int16:
                    short val3 = 0;
                    for(int i = 0;i < items.Length;val3 |= Convert.ToInt16(items[i]),i++) ;
                    return (EnumT)Enum.ToObject(ty,Convert.ToInt16(item) & (~val3));
                case TypeCode.UInt16:
                    ushort val4 = 0;
                    for(int i = 0;i < items.Length;val4 |= Convert.ToUInt16(items[i]),i++) ;
                    return (EnumT)Enum.ToObject(ty,Convert.ToUInt16(item) & (~val4));
                case TypeCode.Int32:
                    int val5 = 0;
                    for(int i = 0;i < items.Length;val5 |= Convert.ToInt32(items[i]),i++) ;
                    return (EnumT)Enum.ToObject(ty,Convert.ToInt32(item) & (~val5));
                case TypeCode.UInt32:
                    uint val6 = 0;
                    for(int i = 0;i < items.Length;val6 |= Convert.ToUInt32(items[i]),i++) ;
                    return (EnumT)Enum.ToObject(ty,Convert.ToUInt32(item) & (~val6));
                case TypeCode.Int64:
                    long val7 = 0;
                    for(int i = 0;i < items.Length;val7 |= Convert.ToInt64(items[i]),i++) ;
                    return (EnumT)Enum.ToObject(ty,Convert.ToInt64(item) & (~val7));
                case TypeCode.UInt64:
                    ulong val8 = 0;
                    for(int i = 0;i < items.Length;val8 |= Convert.ToUInt64(items[i]),i++) ;
                    return (EnumT)Enum.ToObject(ty,Convert.ToUInt64(item) & (~val8));
                default:
                    throw new NotSupportedException("unknow the underlying type");
            }
        }

        public static EnumT[] SplitFlagEnums<EnumT>(this Enum item)
        {
            Enum[] items = SplitFlagEnums(item);
            EnumT[] res = new EnumT[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                res[i] = (EnumT)(Convert.ChangeType(items[i], typeof(EnumT)));
            }
            return res;
        }
        /// <summary>
        /// 将复合的枚举分解为已定义的枚举类型的数组
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Enum[] SplitFlagEnums(this Enum item)
        {
            Type ty = item.GetType();
            if (Enum.IsDefined(ty, item)) return new Enum[] { item };
            Array arr = Enum.GetValues(ty);
            List<Enum> res = new List<Enum>();
            switch (Type.GetTypeCode(Enum.GetUnderlyingType(ty)))
            {
                case TypeCode.Byte:
                    byte num = Convert.ToByte(item);
                    for (int i = arr.GetUpperBound(0); i >= arr.GetLowerBound(0); i--)
                    {
                        byte flag = (byte)arr.GetValue(i);
                        if ((flag & num) == flag)
                        {
                            res.Insert(0, (Enum)Enum.ToObject(ty, flag));
                            num = (byte)((~flag) & num);
                        }
                        if (num == 0) break;
                    }
                    if (num != 0)
                    {
                        throw new System.ComponentModel.InvalidEnumArgumentException();
                    }
                    else
                    {
                        return res.ToArray();
                    }
                case TypeCode.SByte:
                    sbyte num2 = Convert.ToSByte(item);
                    for (int i = arr.GetUpperBound(0); i >= arr.GetLowerBound(0); i--)
                    {
                        sbyte flag = (sbyte)arr.GetValue(i);
                        if ((flag & num2) == flag)
                        {
                            res.Insert(0, (Enum)Enum.ToObject(ty, flag));
                            num = (byte)((~flag) & num2);
                        }
                        if (num2 == 0) break;
                    }
                    if (num2 != 0)
                    {
                        throw new System.ComponentModel.InvalidEnumArgumentException();
                    }
                    else
                    {
                        return res.ToArray();
                    }
                case TypeCode.Int16:
                    short num3 = Convert.ToInt16(item);
                    for (int i = arr.GetUpperBound(0); i >= arr.GetLowerBound(0); i--)
                    {
                        short flag = (short)arr.GetValue(i);
                        if ((flag & num3) == flag)
                        {
                            res.Insert(0, (Enum)Enum.ToObject(ty, flag));
                            num3 = (short)((~flag) & num3);
                        }
                        if (num3 == 0) break;
                    }
                    if (num3 != 0)
                    {
                        throw new System.ComponentModel.InvalidEnumArgumentException();
                    }
                    else
                    {
                        return res.ToArray();
                    }
                case TypeCode.UInt16:
                    ushort num4 = Convert.ToUInt16(item);
                    for (int i = arr.GetUpperBound(0); i >= arr.GetLowerBound(0); i--)
                    {
                        ushort flag = (ushort)arr.GetValue(i);
                        if ((flag & num4) == flag)
                        {
                            res.Insert(0, (Enum)Enum.ToObject(ty, flag));
                            num4 = (ushort)((~flag) & num4);
                        }
                        if (num4 == 0) break;
                    }
                    if (num4 != 0)
                    {
                        throw new System.ComponentModel.InvalidEnumArgumentException();
                    }
                    else
                    {
                        return res.ToArray();
                    }
                case TypeCode.Int32:
                    int num5 = Convert.ToInt32(item);
                    for (int i = arr.GetUpperBound(0); i >= arr.GetLowerBound(0); i--)
                    {
                        int flag = (int)arr.GetValue(i);
                        if ((flag & num5) == flag)
                        {
                            res.Insert(0, (Enum)Enum.ToObject(ty, flag));
                            num5 = (int)((~flag) & num5);
                        }
                        if (num5 == 0) break;
                    }
                    if (num5 != 0)
                    {
                        throw new System.ComponentModel.InvalidEnumArgumentException();
                    }
                    else
                    {
                        return res.ToArray();
                    }
                case TypeCode.UInt32:
                    uint num6 = Convert.ToUInt32(item);
                    for (int i = arr.GetUpperBound(0); i >= arr.GetLowerBound(0); i--)
                    {
                        uint flag = (uint)arr.GetValue(i);
                        if ((flag & num6) == flag)
                        {
                            res.Insert(0, (Enum)Enum.ToObject(ty, flag));
                            num6 = (uint)((~flag) & num6);
                        }
                        if (num6 == 0) break;
                    }
                    if (num6 != 0)
                    {
                        throw new System.ComponentModel.InvalidEnumArgumentException();
                    }
                    else
                    {
                        return res.ToArray();
                    }
                case TypeCode.Int64:
                    long num7 = Convert.ToInt64(item);
                    for (int i = arr.GetUpperBound(0); i >= arr.GetLowerBound(0); i--)
                    {
                        long flag = (long)arr.GetValue(i);
                        if ((flag & num7) == flag)
                        {
                            res.Insert(0, (Enum)Enum.ToObject(ty, flag));
                            num7 = (long)((~flag) & num7);
                        }
                        if (num7 == 0) break;
                    }
                    if (num7 != 0)
                    {
                        throw new System.ComponentModel.InvalidEnumArgumentException();
                    }
                    else
                    {
                        return res.ToArray();
                    }
                case TypeCode.UInt64:
                    ulong num8 = Convert.ToUInt64(item);
                    for (int i = arr.GetUpperBound(0); i >= arr.GetLowerBound(0); i--)
                    {
                        ulong flag = (ulong)arr.GetValue(i);
                        if ((flag & num8) == flag)
                        {
                            res.Insert(0, (Enum)Enum.ToObject(ty, flag));
                            num8 = (ulong)((~flag) & num8);
                        }
                        if (num8 == 0) break;
                    }
                    if (num8 != 0)
                    {
                        throw new System.ComponentModel.InvalidEnumArgumentException();
                    }
                    else
                    {
                        return res.ToArray();
                    }
                default:
                    throw new NotSupportedException("unknow the underlying type");
            }
        }
        /// <summary>
        /// 返回指定枚举中是否存在具有指定值的常数的指示。(包括位枚举)
        /// </summary>
        /// <typeparam name="EnumT"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool IsDefinedEx(this Enum item)
        { 
            Type ty = item.GetType();
            if (!ty.IsEnum) throw new ArgumentException("the item type must be a enum type");
            if (Enum.IsDefined(ty, item)) return true;
            bool hasflag = Attribute.IsDefined(ty, typeof(FlagsAttribute));
            if (!hasflag) return false;

            Array arr = Enum.GetValues(ty);
            switch (Type.GetTypeCode(Enum.GetUnderlyingType(ty)))
            {
                case TypeCode.Byte:
                    byte num = Convert.ToByte(item);
                    for (int i = arr.GetUpperBound(0); i >= arr.GetLowerBound(0); i--)
                    {
                        byte flag = (byte)arr.GetValue(i);
                        if ((flag & num) == flag)
                        {
                            num = (byte)((~flag) & num);
                        }
                        if (num == 0) break;
                    }
                    return num == 0;
                case TypeCode.SByte:
                    sbyte num2 = Convert.ToSByte(item);
                    for (int i = arr.GetUpperBound(0); i >= arr.GetLowerBound(0); i--)
                    {
                        sbyte flag = (sbyte)arr.GetValue(i);
                        if ((flag & num2) == flag)
                        {
                            num = (byte)((~flag) & num2);
                        }
                        if (num2 == 0) break;
                    }
                    return num2 == 0;
                case TypeCode.Int16:
                    short num3 = Convert.ToInt16(item);
                    for (int i = arr.GetUpperBound(0); i >= arr.GetLowerBound(0); i--)
                    {
                        short flag = (short)arr.GetValue(i);
                        if ((flag & num3) == flag)
                        {
                            num3 = (short)((~flag) & num3);
                        }
                        if (num3 == 0) break;
                    }
                    return num3 == 0;
                case TypeCode.UInt16:
                    ushort num4 = Convert.ToUInt16(item);
                    for (int i = arr.GetUpperBound(0); i >= arr.GetLowerBound(0); i--)
                    {
                        ushort flag = (ushort)arr.GetValue(i);
                        if ((flag & num4) == flag)
                        {
                            num4 = (ushort)((~flag) & num4);
                        }
                        if (num4 == 0) break;
                    }
                    return num4 == 0;
                case TypeCode.Int32:
                    int num5 = Convert.ToInt32(item);
                    for (int i = arr.GetUpperBound(0); i >= arr.GetLowerBound(0); i--)
                    {
                        int flag = (int)arr.GetValue(i);
                        if ((flag & num5) == flag)
                        {
                            num5 = (int)((~flag) & num5);
                        }
                        if (num5 == 0) break;
                    }
                    return num5 == 0;
                case TypeCode.UInt32:
                    uint num6 = Convert.ToUInt32(item);
                    for (int i = arr.GetUpperBound(0); i >= arr.GetLowerBound(0); i--)
                    {
                        uint flag = (uint)arr.GetValue(i);
                        if ((flag & num6) == flag)
                        {
                            num6 = (uint)((~flag) & num6);
                        }
                        if (num6 == 0) break;
                    }
                    return num6 == 0;
                case TypeCode.Int64:
                    long num7 = Convert.ToInt64(item);
                    for (int i = arr.GetUpperBound(0); i >= arr.GetLowerBound(0); i--)
                    {
                        long flag = (long)arr.GetValue(i);
                        if ((flag & num7) == flag)
                        {
                            num7 = (long)((~flag) & num7);
                        }
                        if (num7 == 0) break;
                    }
                    return num7 == 0;
                case TypeCode.UInt64:
                    ulong num8 = Convert.ToUInt64(item);
                    for (int i = arr.GetUpperBound(0); i >= arr.GetLowerBound(0); i--)
                    {
                        ulong flag = (ulong)arr.GetValue(i);
                        if ((flag & num8) == flag)
                        {
                            num8 = (ulong)((~flag) & num8);
                        }
                        if (num8 == 0) break;
                    }
                    return num8 == 0;
                default:
                    throw new NotSupportedException("unknow the underlying type");
            }
          
        }

        public static MemberInfo[] GetEnumMemberInfos(this Enum item)
        {
            return  EnumMemberTranslate.GetObject(item.GetType()).GetMemberInfos(item);
        }
        /// <summary>
        /// 是否是枚举支持的数据类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsEnumUnderlyingType(this Type type)
        {
            return type.Equals(typeof(SByte)) ||
                   type.Equals(typeof(Int16)) ||
                   type.Equals(typeof(Int32)) ||
                   type.Equals(typeof(Int64)) ||
                   type.Equals(typeof(Byte)) ||
                   type.Equals(typeof(UInt16)) ||
                   type.Equals(typeof(UInt32)) ||
                   type.Equals(typeof(UInt64));
        } 
    }
    internal class EnumMemberTranslate
    {
        static Dictionary<Type, EnumMemberTranslate> caches = new Dictionary<Type, EnumMemberTranslate>();
        public static EnumMemberTranslate GetObject(Type enumType)
        {
            lock(caches)
            {
                if (caches.ContainsKey(enumType))
                {
                    return caches[enumType];
                }
                else
                {
                    EnumMemberTranslate em = new EnumMemberTranslate(enumType);
                    caches.Add(enumType, em);
                    return em;
                }
            }
        }

        private bool hasflag = false;

        public bool Hasflag
        {
            get { return hasflag; }
        }
        private EnumMemberTranslate(Type enumType)
        {
            Type ty = enumType;
            if (ty.IsEnum)
            {
                foreach (var field in ty.GetEnumFields())
                {
                    dic.Add((Enum)field.GetValue(null), new MemberInfo[] { field });
                }
                hasflag = Attribute.IsDefined(ty, typeof(FlagsAttribute));
            }
            else
            {
                throw new Exception("only supported the enum type");
            }
        }
    
        private Dictionary<Enum, MemberInfo[]> dic = new Dictionary<Enum, MemberInfo[]>();
        public MemberInfo[] GetMemberInfos(Enum item)
        {
            if (dic.ContainsKey(item))
            {
                return dic[item];
            }
            else
            {
                if (this.hasflag)//位枚举
                {
                    List<MemberInfo> lst = new List<MemberInfo>();
                    foreach (var v in item.SplitFlagEnums())
                    {
                        lst.AddRange(dic[v]);
                    }
                    lock (dic)
                    {
                        if (!dic.ContainsKey(item))
                        {
                            dic.Add(item, lst.ToArray());
                        }
                    }
                    return lst.ToArray();
                }
                else//
                {
                    throw new System.ComponentModel.InvalidEnumArgumentException();
                }
            }
        }
    }
    
   
}
