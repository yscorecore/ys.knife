using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Data;
namespace System
{
    /// <summary>
    /// 提供数组类型的一些常用辅助功能
    /// </summary>
    public static class ArrayEx
    {
        #region topvalue
        public static T[] TopValue<T>(this List<T> lst, int topNum)
            where T : IComparable
        {
            if (lst == null) throw new ArgumentNullException("lst");
            return TopValue(lst.ToArray(), topNum);
        }
        public static T[] TopValue<T>(this T[] array, int topNum)
           where T : IComparable
        {
            if (array == null) throw new ArgumentNullException("array");
            if (array.Length <= topNum) throw new ArgumentOutOfRangeException("topNum");
            array = (T[])array.Clone();
            int len = array.Length;
            T[] res = new T[topNum];
            for (int i = 0; i < topNum; i++)
            {
                int index = i;
                for (int j = i + 1; j < len; j++)
                {
                    if (array[index].CompareTo(array[j]) > 0)
                    {
                        index = j;
                    }
                }
                res[i] = array[index];
                if (index != i)
                {
                    T tmp = array[i]; array[i] = array[index]; array[index] = tmp;
                }


            }
            return res;
        }

        public static T[] TopValue<T>(this List<T> lst, int topNum, Comparison<T> comparison)
        {
            if (lst == null) throw new ArgumentNullException("lst");
            return TopValue(lst.ToArray(), topNum, comparison);
        }
        public static T[] TopValue<T>(this T[] array, int topNum, Comparison<T> comparison)
        {
            if (comparison == null) throw new ArgumentNullException("comparison");
            if (array == null) throw new ArgumentNullException("array");
            if (array.Length <= topNum) throw new ArgumentOutOfRangeException("topNum");
            array = (T[])array.Clone();
            int len = array.Length;
            T[] res = new T[topNum];
            for (int i = 0; i < topNum; i++)
            {
                int index = i;
                for (int j = i + 1; j < len; j++)
                {
                    if (comparison(array[index], array[j]) < 0)
                    {
                        index = j;
                    }
                }
                res[i] = array[index];
                if (index != i)
                {
                    T tmp = array[i]; array[i] = array[index]; array[index] = tmp;
                }
            }
            return res;
        }

        public static IndexData<T>[] TopValue<T>(this IndexData<T>[] array, int topNum)
            where T : IComparable
        {
            IndexData<T>[] indexdata = TopValue(array, topNum, new Comparison<IndexData<T>>((a, b) => { return a.Value.CompareTo(b.Value); }));
            return indexdata;
        }
        public static IndexData<T>[] TopValue<T>(this List<IndexData<T>> lst, int topNum)
           where T : IComparable
        {
            return TopValue(lst.ToArray(), topNum);
        }
        public static IndexData<T>[] TopValue<T>(this IndexData<T>[] array, int topNum, Comparison<T> valueComparison)
        {
            if (valueComparison == null) throw new ArgumentNullException("comparison");
            IndexData<T>[] indexdata = TopValue(array, topNum, new Comparison<IndexData<T>>((a, b) => { return valueComparison(a.Value, b.Value); }));
            return indexdata;
        }
        public static IndexData<T>[] TopValue<T>(this List<IndexData<T>> lst, int topNum, Comparison<T> valueComparison)
        {
            return TopValue(lst.ToArray(), topNum, valueComparison);
        }
        #endregion

        #region lastValue
        public static T[] LastValue<T>(this List<T> lst, int lastNum)
         where T : IComparable
        {
            if (lst == null) throw new ArgumentNullException("lst");
            return LastValue(lst.ToArray(), lastNum);
        }
        public static T[] LastValue<T>(this T[] array, int lastNum)
           where T : IComparable
        {
            if (array == null) throw new ArgumentNullException("array");
            if (array.Length <= lastNum) throw new ArgumentOutOfRangeException("lastNum");
            array = (T[])array.Clone();
            int len = array.Length;
            T[] res = new T[lastNum];
            for (int i = 0; i < lastNum; i++)
            {
                int index = i;
                for (int j = i + 1; j < len; j++)
                {
                    if (array[index].CompareTo(array[j]) < 0)
                    {
                        index = j;
                    }
                }
                res[i] = array[index];
                if (index != i)
                {
                    T tmp = array[i]; array[i] = array[index]; array[index] = tmp;
                }


            }
            return res;
        }

        public static T[] LastValue<T>(this List<T> lst, int lastNum, Comparison<T> comparison)
        {
            if (lst == null) throw new ArgumentNullException("lst");
            return LastValue(lst.ToArray(), lastNum, comparison);
        }
        public static T[] LastValue<T>(this T[] array, int lastNum, Comparison<T> comparison)
        {
            if (comparison == null) throw new ArgumentNullException("comparison");
            if (array == null) throw new ArgumentNullException("array");
            if (array.Length <= lastNum) throw new ArgumentOutOfRangeException("lastNum");
            array = (T[])array.Clone();
            int len = array.Length;
            T[] res = new T[lastNum];
            for (int i = 0; i < lastNum; i++)
            {
                int index = i;
                for (int j = i + 1; j < len; j++)
                {
                    if (comparison(array[index], array[j]) < 0)
                    {
                        index = j;
                    }
                }
                res[i] = array[index];
                if (index != i)
                {
                    T tmp = array[i]; array[i] = array[index]; array[index] = tmp;
                }
            }
            return res;
        }

        public static IndexData<T>[] LastValue<T>(this IndexData<T>[] array, int lastNum)
            where T : IComparable
        {
            IndexData<T>[] indexdata = LastValue(array, lastNum, new Comparison<IndexData<T>>((a, b) => { return a.Value.CompareTo(b.Value); }));
            return indexdata;
        }
        public static IndexData<T>[] LastValue<T>(this List<IndexData<T>> lst, int lastNum)
            where T : IComparable
        {
            return LastValue(lst.ToArray(), lastNum, new Comparison<IndexData<T>>((a, b) => { return a.Value.CompareTo(b.Value); }));
        }
        public static IndexData<T>[] LastValue<T>(this IndexData<T>[] array, int topNum, Comparison<T> comparison)
        {
            if (comparison == null) throw new ArgumentNullException("comparison");
            IndexData<T>[] indexdata = LastValue(array, topNum, new Comparison<IndexData<T>>((a, b) => { return comparison(a.Value, b.Value); }));
            return indexdata;
        }
        public static IndexData<T>[] LastValue<T>(this List<IndexData<T>> lst, int topNum, Comparison<T> comparison)
        {
            return LastValue(lst.ToArray(), topNum, comparison);
        }
        #endregion

        public static IndexData<T>[] ToIndexDataArray<T>(this T[] array, int offset = 0)
        {
            if (array == null) throw new ArgumentNullException("array");
            IndexData<T>[] res = new IndexData<T>[array.Length];
            for (int i = 0; i < array.Length; i++) res[i] = new IndexData<T>(i + offset, array[i]);
            return res;
        }

        //public static List<IndexData<T>> ToIndexDataList<T>(this List<T> lst,int offset=0)
        //{
        //    if (lst == null) throw new ArgumentNullException("lst");
        //    List<IndexData<T>> res = new List<IndexData<T>>(lst.Count);
        //    for (int i = 0; i < lst.Count; i++) res[i] = new IndexData<T>(i + offset, lst[i]);
        //    return res;
        //}

        /// <summary>
        /// 返回循环访问 的枚举器。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="seg"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetItems<T>(this ArraySegment<T> seg)
        {
            for (int i = seg.Offset; i < seg.Offset + seg.Count; i++)
            {
                yield return seg.Array[i];
            }
        }
        /// <summary>
        /// 获取对应的数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="seg"></param>
        /// <returns></returns>
        public static T[] GetArray<T>(this ArraySegment<T> seg)
        {
            T[] res = new T[seg.Count];
            Array.Copy(seg.Array, seg.Offset, res, 0, seg.Count);
            return res;
        }
        /// <summary>
        /// 获取指定数组的子数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static T[] SubArray<T>(this T[] array, int index)
        {
            if (index > array.Length || index < 0) throw new ArgumentOutOfRangeException("index");
            T[] res = new T[array.Length - index];
            Array.Copy(array, index, res, 0, res.Length);
            return res;
        }
        /// <summary>
        /// 获取指定数组的子数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static T[] SubArray<T>(this T[] array, int index, int length)
        {
            if (index > array.Length || index < 0) throw new ArgumentOutOfRangeException("index");
            if (length > array.Length - index) throw new ArgumentOutOfRangeException("length");
            T[] res = new T[length];
            Array.Copy(array, index, res, 0, length);
            return res;
        }
        /// <summary>
        /// 复制数据到指定的数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="seg"></param>
        /// <param name="destinationArray"></param>
        /// <param name="destinationIndex"></param>
        public static void CopyTo<T>(this ArraySegment<T> seg, Array destinationArray, int destinationIndex)
        {
            Array.Copy(seg.Array, seg.Offset, destinationArray, destinationIndex, seg.Count);
        }

        /// <summary>
        /// 将数组分割成小数组的表示
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="maxlength">最大是长度</param>
        /// <returns></returns>
        public static List<ArraySegment<T>> Split<T>(this T[] array, int maxlength)
        {
            if (array == null) return new List<ArraySegment<T>>();
            List<ArraySegment<T>> res = new List<ArraySegment<T>>(array.Length / maxlength + 1);
            int start = 0;
            while (start < array.Length)
            {
                if (start + maxlength <= array.Length)
                {
                    res.Add(new ArraySegment<T>(array, start, maxlength));
                    start += maxlength;
                }
                else
                {
                    res.Add(new ArraySegment<T>(array, start, array.Length - start));
                    break;
                }
            }
            return res;
        }
        /// <summary>
        /// 比较两个数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
        public static bool ArrayEquals<T>(this T[] array1, T[] array2)
        {

            if (array1 == null) return array2 == null;
            int len = array1.Length;
            if (array2 == null || array2.Length != len) return false;
            for (int i = 0; i < len; i++)
            {
                if (!object.Equals(array1[i], array2[i]))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 比较两个数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst1"></param>
        /// <param name="lst2"></param>
        /// <returns></returns>
        public static bool ListEquals<T>(this IList<T> lst1, IList<T> lst2)
        {
            if (lst1 == null) return lst2 == null;
            int len = lst1.Count;
            if (lst2 == null || lst2.Count != len) return false;
            for (int i = 0; i < len; i++)
            {
                if (!object.Equals(lst1[i], lst2[i]))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string Join<T>(this IEnumerable<T> items, string separator)
        {
            if (items == null) return string.Empty;
            return string.Join(separator, (from p in items select Convert.ToString(p)).ToArray());
        }
        /// <summary>
        /// 将字节数组转为字符串
        /// </summary>
        /// <param name="bys"></param>
        /// <returns></returns>
        public static string ToHexString(this IEnumerable<byte> bys, bool toLowerCase = true)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var v in bys)
            {
                sb.AppendFormat(toLowerCase ? "{0:x2}" : "{0:X2}", v);
            }
            return sb.ToString();
        }
        public static byte[] HexStringToBytes(string hexString)
        {
            if (hexString == null) return new byte[0];
            if (hexString.Length % 2 == 1) throw new ArgumentException("the length of the hexString must be a even number");
            byte[] res = new byte[hexString.Length / 2];
            for (int i = 0, j = 0; i < hexString.Length; i += 2, j++)
            {
                res[j] = ConvertToByte(hexString, i);
            }
            return res;
        }
        /// <summary>
        /// 将一个List集合转成成为另一个类型的List集合对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// 
        /// <param name="lst"></param>
        /// <param name="includeComplexProperty">是否包含复杂属性</param>
        /// <returns></returns>
        public static List<T> AttachNewList<T>(this IList lst, bool includeComplexProperty = true)
        {
            if (lst == null) return null;
            List<T> res = new List<T>();
            var lsttype = lst.GetType();
            Type itemType = null;
            if (lsttype.IsGenericType)
            {
                itemType = lsttype.GetGenericArguments().FirstOrDefault();
            }
            if (itemType == null)
            {
                foreach (var v in lst)
                {
                    if (v != null)
                    {
                        itemType = v.GetType();
                        break;
                    }
                }
            }
            if (itemType == null)
            {
                throw new ArgumentException("无法获取到子项类型");
            }
            Dictionary<PropertyInfo, PropertyInfo> propMaps = new Dictionary<PropertyInfo, PropertyInfo>();
            foreach (var v in typeof(T).GetProperties())
            {
                if (!v.CanWrite) continue;
                var prop2 = itemType.GetProperty(v.Name);
                if (prop2 == null) continue;
                if (!prop2.CanRead) continue;
                if (!prop2.PropertyType.IsAssignableFrom(v.PropertyType)) continue;
                if (includeComplexProperty || !v.PropertyType.IsComplexType())
                {
                    propMaps.Add(v, prop2);
                }

            }
            foreach (var o in lst)
            {
                var newobj = Activator.CreateInstance<T>();
                if (o != null)
                {
                    foreach (var pmap in propMaps)
                    {
                        pmap.Key.SetValue(newobj, pmap.Value.GetValue(o, null), null);
                    }
                }
                res.Add(newobj);
            }
            return res;
        }

        public static List<T> CloneList<T>(this IList<T> lst, bool includeComplexProperty = true)
        {
            return AttachNewList<T>((IList)lst, includeComplexProperty);
        }
        /// <summary>
        /// 展开一个List集合，使得复杂属性可以映射到平面属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        /// <returns></returns>
        public static List<T> DeployNewList<T>(this IList lst)
        {
            if (lst == null) return null;
            List<T> res = new List<T>();
            var lsttype = lst.GetType();
            Type itemType = null;
            if (lsttype.IsGenericType)
            {
                itemType = lsttype.GetGenericArguments().FirstOrDefault();
            }
            if (itemType == null)
            {
                foreach (var v in lst)
                {
                    if (v != null)
                    {
                        itemType = v.GetType();
                        break;
                    }
                }
            }
            if (itemType == null)
            {
                throw new ArgumentException("无法获取到子项类型");
            }


            var pinfos = (from p in itemType.GetProperties()
                          where p.PropertyType.IsClass && p.CanRead && Type.GetTypeCode(p.PropertyType) == TypeCode.Object
                          select p).ToDictionary(p => p, p => p.PropertyType.GetProperties());
            Dictionary<PropertyInfo, PInfo> pmaps = new Dictionary<PropertyInfo, PInfo>();
            foreach (var v in typeof(T).GetProperties())
            {
                if (!v.CanWrite) continue;
                var prop2 = itemType.GetProperty(v.Name);
                if (prop2 == null)
                {
                    foreach (var mp in pinfos)
                    {
                        bool hasfind = false;
                        foreach (var p in mp.Value)
                        {
                            if (p.Name == v.Name && p.PropertyType == v.PropertyType && p.CanRead)
                            {
                                pmaps.Add(v, new PInfo() { FirstProperty = mp.Key, LastProperty = p });
                                hasfind = true;
                                break;
                            }
                        }
                        if (hasfind)
                            break;
                    }
                }
                else if (prop2.CanRead && prop2.PropertyType == v.PropertyType)
                {
                    pmaps.Add(v, new PInfo() { FirstProperty = prop2 });
                }
            }


            foreach (var o in lst)
            {
                var newobj = Activator.CreateInstance<T>();
                if (o != null)
                {
                    Dictionary<PropertyInfo, object> cache = new Dictionary<PropertyInfo, object>();
                    foreach (var pmap in pmaps)
                    {
                        if (pmap.Value.LastProperty == null)
                        {
                            pmap.Key.SetValue(newobj, pmap.Value.FirstProperty.GetValue(o, null), null);
                        }
                        else
                        {
                            if (cache.ContainsKey(pmap.Value.FirstProperty))
                            {
                                var obj = cache[pmap.Value.FirstProperty];
                                if (obj != null)
                                {
                                    pmap.Key.SetValue(newobj, pmap.Value.LastProperty.GetValue(obj, null), null);
                                }
                            }
                            else
                            {
                                var obj = pmap.Value.FirstProperty.GetValue(o, null);
                                cache.Add(pmap.Value.FirstProperty, obj);
                                if (obj != null)
                                {
                                    pmap.Key.SetValue(newobj, pmap.Value.LastProperty.GetValue(obj, null), null);
                                }
                            }
                        }
                    }
                }
                res.Add(newobj);
            }
            return res;

        }



        private static byte ConvertToByte(string text, int startIndex)
        {
            char ch1 = text[startIndex];
            char ch2 = text[startIndex + 1];
            return (byte)(GetHexNum(ch1) * 16 + GetHexNum(ch2));
        }
        private static int GetHexNum(char ch)
        {
            switch (ch)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return ch - '0';
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                    return ch - 'A' + 10;
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                    return ch - 'a' + 10;
                default:
                    throw new ArgumentException("char is not a hex char");
            }
        }

        private class PInfo
        {
            public PropertyInfo FirstProperty { get; set; }
            public PropertyInfo LastProperty { get; set; }
        }
    }


}
