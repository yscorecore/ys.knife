using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace System
{
    public static class RandomUtility
    {
        static char[] varNameChs2 = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '_', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        static char[] varNameChs1 = new char[] { '_', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        static string fullCode = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
        static Random ran = new Random();
        public static int Next()
        {
            return ran.Next();
        }
        public static int Next(int maxValue)
        {
            return ran.Next(maxValue);
        }
        public static bool NextBoolean()
        {
            return ran.Next(2) == 1;
        }
        public static int Next(int minValue, int maxValue)
        {
            return ran.Next(minValue, maxValue);
        }
       
        public static void FillBytes(byte[] buffer)
        {
            ran.NextBytes(buffer);
        }
        public static byte[] NextBytes(int arrayLength)
        {
            byte[] bys = new byte[arrayLength];
            ran.NextBytes(bys);
            return bys;
        }
        public static int[] NextArray(int count)
        {
            int[] res = new int[count];
            for (int i = 0; i < count; i++)
            {
                res[i] = Next();
            }
            return res;
        }
        public static int[] NextArray(int count, int maxValue)
        {
            int[] res = new int[count];
            for (int i = 0; i < count; i++)
            {
                res[i] = Next(maxValue);
            }
            return res;
        }
        /// <summary>
        /// 随机生成不重复的数组(适用与样本较多，抽样较少的情况)
        /// </summary>
        /// <param name="count"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int[] NextUniqueArray(int count, int maxValue)
        {
            int[] result = new int[count];
            HashSet<int> has = new HashSet<int>();
            int r;
            for (int i = 0; i < count; i++)
            {
                //2016年4月21修正bug
                do
                {
                    r = ran.Next(maxValue);
                }
                while (has.Contains(r));
                has.Add(r);
                result[i] = r;
                //while (!has.Contains(r = ran.Next(maxValue)))
                //{
                //    result[i] = r;
                //    has.Add(r);
                //    break;
                //}
            }
            return result;
        }
        /// <summary>
        /// 随机生成不重复的数组(适用与样本较多，抽样较少的情况)
        /// </summary>
        /// <param name="count"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int[] NextUniqueArray(int count, int minValue, int maxValue)
        {
            int[] result = new int[count];
            HashSet<int> has = new HashSet<int>();
            int r;
            for (int i = 0; i < count; i++)
            {
                //2016年4月21修正bug
                do
                {
                    r = ran.Next(maxValue);
                }
                while (has.Contains(r));
                has.Add(r);
                result[i] = r;
            }
            return result;
        }

        public static int[] NextArray(int count, int minValue, int maxValue)
        {
            int[] res = new int[count];
            for (int i = 0; i < count; i++)
            {
                res[i] = Next(minValue, maxValue);
            }
            return res;
        }
        public static double NextDouble()
        {
            return ran.NextDouble();
        }
        public static double RandomValue(double baseValue, double scope, RandomScopeKind scopeKind = RandomScopeKind.All)
        {
            double flag = 0;
            if (scopeKind == RandomScopeKind.LessThan) flag = -1;
            else if (scopeKind == RandomScopeKind.GreaterThan) flag = 1;
            else flag = ran.Next(2) == 1 ? 1 : -1;
            return baseValue + flag * scope * ran.NextDouble();
        }
        /// <summary>
        /// 随机生成一个合法的变量名称
        /// </summary>
        /// <param name="length">需要生成变量名称的长度,要求大于0</param>
        /// <returns>返回生成的变量名称</returns>
        public static string RandomVarName(int length)
        {
            if (length < 0) throw new ArgumentException("the length of the variable name must greater than 0.");
            char[] res = new char[length];
            res[0] = varNameChs1[ran.Next(varNameChs1.Length)];
            for (int i = 1; i < length; i++) res[i] = varNameChs2[ran.Next(varNameChs2.Length)];
            return new string(res);
            //return RandomCode(varNameChs2, length);
        }
        public static string RandomCode(string codeBase, int len)
        {
            if (string.IsNullOrEmpty(codeBase))
            {
                throw new ArgumentNullException("codeBase");
            }
            else
            {
                StringBuilder sb = new StringBuilder(len);
                int clen = codeBase.Length;
                for (int i = 0; i < len; i++)
                    sb.Append(codeBase[Next(clen)]);
                return sb.ToString();
            }
        }
        public static string RandomCode(char[] codeBase, int len)
        {
            if (codeBase == null || codeBase.Length == 0)
            {
                throw new ArgumentException("the codeBase array must be not null or empty", "codeBase");
            }
            else
            {
                StringBuilder sb = new StringBuilder(len);
                int clen = codeBase.Length;
                for (int i = 0; i < len; i++)
                    sb.Append(codeBase[Next(clen)]);
                return sb.ToString();
            }
        }
        public static string RandomCode(RandomCodeKind codeKind, int len)
        {
            StringBuilder sb = new StringBuilder();
            if (codeKind.HasFlagValue<RandomCodeKind>(RandomCodeKind.Digit))
            {
                sb.Append("1234567890");
            }
            if (codeKind.HasFlagValue<RandomCodeKind>(RandomCodeKind.LowerLetter))
            {
                sb.Append("abcdefghijklmnopqrstuvwxyz");
            }
            if (codeKind.HasFlagValue<RandomCodeKind>(RandomCodeKind.UpperLetter))
            {
                sb.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            }
            return RandomCode(sb.ToString(), len);
        }
        public static string RandomCode(int len)
        {
            return RandomCode(fullCode, len);
        }
        /// <summary>
        /// 随机打乱一个数组中所有元素的顺序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        public static void RandomSequence<T>(T[] array)
        {
            int count = array.Length;
            for (int i = 0; i < count; i++)
            {
                int index1 = i;
                int index2 = Next(count);
                T temp = array[index1];
                array[index1] = array[index2];
                array[index2] = temp;
            }
        }
        /// <summary>
        /// 随机打乱一个列表中所有元素的顺序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        public static void RandomSequence<T>(IList<T> lst)
        {
            int count = lst.Count;
            for (int i = 0; i < count; i++)
            {
                int index1 = i;
                int index2 = Next(count);
                T temp = lst[index1];
                lst[index1] = lst[index2];
                lst[index2] = temp;
            }
        }
        /// <summary>
        /// 随机打乱一个列表中所有元素的顺序
        /// </summary>
        /// <param name="lst"></param>
        public static void RandomSequence(IList lst)
        {
            int count = lst.Count;
            for (int i = 0; i < count; i++)
            {
                int index1 = i;
                int index2 = Next(count);
                object temp = lst[index1];
                lst[index1] = lst[index2];
                lst[index2] = temp;
            }
        }

        /// <summary>
        /// 随机抽取列表中的其中一个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T RandomOne<T>(IList<T> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.Count <= 0) throw new ArgumentException("the source  must has one value at least");
            return source[ran.Next(source.Count)];
        }

        /// <summary>
        /// 随机抽取数组中的其中一个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T RandomOne<T>(T[] source)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.Length <= 0) throw new ArgumentException("the source  must has one value at least");
            return source[ran.Next(source.Length)];
        }
        /// <summary>
        /// 随机抽取列表中的多个值（不重复）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T[] RandomMany<T>(IList<T> source, int count)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (count == 0) return new T[0];
            if (count < 0) throw new ArgumentException("the count must be greater than zero");
            if (count > source.Count) throw new ArgumentException("抽样数应该小于样本的总数");
            if (count == source.Count)
            {
                var arr = source.ToArray();
                RandomSequence<T>(arr);
                return arr;
            }
            else if (count > source.Count / 2)//抽样数目超过一半以上
            {
                var arr = source.ToArray();
                RandomSequence<T>(arr);
                return arr.SubArray(0, count);
            }
            else
            {
                int[] indexs = NextUniqueArray(count, source.Count);
                T[] arr = new T[count];
                for (int i = 0; i < count; i++)
                {
                    arr[i] = source[indexs[i]];
                }
                return arr;
            }
        }
        /// <summary>
        /// 随机抽取列表中的多个值（不重复）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T[] RandomMany<T>(T[] source, int count)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (count == 0) return new T[0];
            if (count < 0) throw new ArgumentException("the count must be greater than zero");
            if (count > source.Length) throw new ArgumentException("抽样数应该小于样本的总数");
            if (count == source.Length)
            {
                var arr = source.ToArray();
                RandomSequence<T>(arr);
                return arr;
            }
            else if (count > source.Length / 2)//抽样数目超过一半以上
            {
                var arr = source.ToArray();
                RandomSequence<T>(arr);
                return arr.SubArray(0, count);
            }
            else
            {
                int[] indexs = NextUniqueArray(count, source.Length);
                T[] arr = new T[count];
                for (int i = 0; i < count; i++)
                {
                    arr[i] = source[indexs[i]];
                }
                return arr;
            }
        }

        /// <summary>
        ///  将一个整数随机切割成一个数组，使得该数组之和等于指定的值
        /// </summary>
        /// <param name="sum"></param>
        /// <param name="count"></param>
        /// <param name="minValue"></param>
        /// <returns></returns>
        public static int[] RandomCut(int sum, int count, int minValue)
        {
            if (count < 0) throw new ArgumentOutOfRangeException("count");
            if (sum < 0) throw new ArgumentOutOfRangeException("sum");
            if (minValue < 0)
                throw new ArgumentOutOfRangeException("minValue", "minValue不能为负数");
            else if (minValue == 0)
                return RandomCut(sum, count);
            else
            {
                int minneed = minValue * count;
                if (minneed > sum)
                {
                    throw new ArgumentException("sum值不够分配");
                }
                else if (minneed == sum)
                {
                    int[] res = new int[count];
                    for (int i = 0; i < count; i++) res[i] = minValue;
                    return res;
                }
                else
                {
                    int[] res2 = RandomCut(sum - minneed, count);
                    for (int i = 0; i < count; i++) res2[i] = res2[i] + minValue;
                    return res2;
                }
            }

        }
        /// <summary>
        /// 将一个整数随机切割成一个数组，使得该数组之和等于指定的值
        /// </summary>
        /// <param name="sum"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static int[] RandomCut(int sum, int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException("count");
            if (sum < 0) throw new ArgumentOutOfRangeException("sum");
            if (count == 1) return new int[] { sum };
            if (sum == 0) return new int[count];
            List<int> positions = new List<int>();
            for (int i = 0; i < count - 1; i++)
            {
                positions.Add(ran.Next(sum));
            }
            positions.Add(sum);
            var sorted = (from p in positions
                          orderby p
                          select p).ToArray();

            for (int i = sorted.Length - 1; i > 0; i--)
            {
                sorted[i] = sorted[i] - sorted[i - 1];
            }
            return sorted;
        }

        /// <summary>
        /// 以一定的概率测试是否命中
        /// </summary>
        /// <param name="hitCount">命中样本的数量</param>
        /// <param name="sampleCount">样本总数</param>
        /// <returns>返回命中的结果，命中返回true，否则返回false</returns>
        public static bool HitTest(int hitCount, int sampleCount)
        {
            return ran.Next(sampleCount) < hitCount;
        }
        /// <summary>
        /// 以一定的概率测试是否命中
        /// </summary>
        /// <param name="probability">命中的概率[0,1]</param>
        /// <returns>返回命中的结果，命中返回true，否则返回false</returns>
        public static bool HitTest(double probability)
        {
            //2147483647,int的最大值，所以采用乘以1000000000既保证了不溢出，又保证了足够概率的精度
            if (probability >= 1) return true;
            if (probability <= 0) return false;
            return HitTest((int)(probability * 1000000000), 1000000000);
        }

        /// <summary>
        /// 利用随机分布组合一个新的随机函数
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="except">期望值</param>
        /// <returns></returns>
        public static double CombinNext(double min, double max, double except)
        {
            if (except > (min + max) / 2)
            {
                return min + max - CombinNext(min, max, min + max - except);
            }
            else
            {
                double p = (max - except) / (max - min);
                double M = max - (min + max - 2 * except) / p;

                if (ran.NextDouble() <= p)
                {
                    return ran.NextDouble() * (M - min) + min;
                }
                else
                {
                    return ran.NextDouble() * (max - min) + min;
                }
            }
        }
    }
    [Flags]
    public enum RandomScopeKind
    {
        /// <summary>
        /// 大于
        /// </summary>
        GreaterThan = 1,
        /// <summary>
        /// 小于
        /// </summary>
        LessThan = 2,
        /// <summary>
        /// 大于或小于
        /// </summary>
        All = GreaterThan| LessThan,

    }
    [Flags]
    public enum RandomCodeKind
    {
        /// <summary>
        /// 数字
        /// </summary>
        Digit=1,
        /// <summary>
        /// 小写字母
        /// </summary>
        LowerLetter=2,
        /// <summary>
        /// 大写字母
        /// </summary>
        UpperLetter=4,
        /// <summary>
        /// 数字或者小写字母或者大写字母
        /// </summary>
        All=Digit|LowerLetter|UpperLetter
    }
}
