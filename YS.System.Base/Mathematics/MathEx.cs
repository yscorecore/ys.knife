using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace System
{
    public static class MathEx
    {

        #region 三角函数
        /// <summary>
        /// 返回<see cref="System.Double"/>数字的正割
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double Sec(double num)
        {   
            return 1 / Math.Cos(num);
        }
        /// <summary>
        /// 返回<see cref="System.Double"/>数字的余割
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double Csc(double num)
        {
            return 1 / Math.Sin(num);
        }
        /// <summary>
        /// 返回<see cref="System.Double"/>数字的余切
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double Cot(double num)
        {
            return 1 / Math.Tan(num);
        }
        /// <summary>
        /// 返回<see cref="System.Double"/>数字的反正弦
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double Asin(double num)
        {
            return Math.Atan(num / Math.Sqrt(-num * num + 1));
        }
        /// <summary>
        /// 返回<see cref="System.Double"/>数字的反余弦
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double Acos(double num)
        {
            return Math.Atan(-num / Math.Sqrt(-num * num + 1)) + 2 * Math.Atan(1);
        }
        /// <summary>
        /// 返回<see cref="System.Double"/>数字的反正割
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double Asec(double num)
        {
            return 2 * Math.Atan(1) - Math.Atan(Math.Sign(num) / Math.Sqrt(num * num - 1));
        }
        /// <summary>
        /// 返回<see cref="System.Double"/>数字的反余割
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double Acsc(double num)
        {
            return Math.Atan(Math.Sign(num) / Math.Sqrt(num * num - 1));
        }
        /// <summary>
        /// 返回<see cref="System.Double"/>数字的反余切
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double Acot(double num)
        {
            return 2 * Math.Atan(1) - Math.Atan(num);
        }
        /// <summary>
        /// 返回<see cref="System.Double"/>数字的双曲正弦
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double Sinh(double num)
        {
            return (Math.Exp(num) - Math.Exp(-num)) / 2;
        }
        /// <summary>
        /// 返回<see cref="System.Double"/>数字的双曲余弦
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double Cosh(double num)
        {
            return (Math.Exp(num) + Math.Exp(-num)) / 2;
        }
        /// <summary>
        /// 返回<see cref="System.Double"/>数字的双曲正切
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double Tanh(double num)
        {
            return (Math.Exp(num) - Math.Exp(-num)) / (Math.Exp(num) + Math.Exp(-num));
        }
        /// <summary>
        /// 返回<see cref="System.Double"/>数字的双曲正割
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double Sech(double num)
        {
            return 2 / (Math.Exp(num) + Math.Exp(-num));
        }
        /// <summary>
        /// 返回<see cref="System.Double"/>数字的双曲余割
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double Csch(double num)
        {
            return 2 / (Math.Exp(num) - Math.Exp(-num));
        }
        /// <summary>
        /// 返回<see cref="System.Double"/>数字的双曲余切
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double Coth(double num)
        {
            return (Math.Exp(num) + Math.Exp(-num)) / (Math.Exp(num) - Math.Exp(-num));
        }
        /// <summary>
        /// 返回<see cref="System.Double"/>数字的反双曲正弦
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double Asinh(double num)
        {
            return Math.Log(num + Math.Sqrt(num * num + 1));
        }
        /// <summary>
        /// 返回<see cref="System.Double"/>数字的反双曲余弦
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double Acosh(double num)
        {
            return Math.Log(num + Math.Sqrt(num * num - 1));
        }
        /// <summary>
        /// 返回<see cref="System.Double"/>数字的反双曲正切
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double Atanh(double num)
        {
            return Math.Log((1 + num) / (1 - num)) / 2;
        }
        /// <summary>
        /// 返回<see cref="System.Double"/>数字的反双曲正割
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double AsecH(double num)
        {
            return Math.Log((Math.Sqrt(-num * num + 1) + 1) / num);
        }
        /// <summary>
        /// 返回<see cref="System.Double"/>数字的反双曲余割
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double Acsch(double num)
        {
            return Math.Log((Math.Sign(num) * Math.Sqrt(num * num + 1) + 1) / num);
        }
        /// <summary>
        /// 返回<see cref="System.Double"/>数字的反双曲余切
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double Acoth(double num)
        {
            return Math.Log((num + 1) / (num - 1)) / 2;
        }   

        #endregion
        /// <summary>
        /// 计算指定数字的自然对数
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static double Ln(double d)
        {
            return Math.Log(d);
        }
        /// <summary>
        /// 计算指定数字的以10为底的对数
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static double Lg(double d)
        {
            return Math.Log10(d);
        }
        /// <summary>
        /// 求两个数的最大公约数
        /// </summary>
        /// <param name="num1">数字1</param>
        /// <param name="num2">数字2</param>
        /// <returns></returns>
        public static int GCD(int num1,int num2)
        {
            return num1 % num2 == 0 ? num2 : GCD(num2,num1 % num2);
        }
        /// <summary>
        /// 求两个数的最小公倍数
        /// </summary>
        /// <param name="num1">数字1</param>
        /// <param name="num2">数字2</param>
        /// <returns></returns>
        public static int LCM(int num1,int num2)
        {
            return num1 * num2 / GCD(num1,num2);
        }
        /// <summary>
        /// 列取指定数字的指数次幂
        /// </summary>
        /// <param name="number">底数</param>
        /// <param name="exponent">指数</param>
        /// <returns>幂</returns>
        public static IEnumerable<int> Power(int number,int exponent)
        {
            int counter = 0;
            int result = 1;
            while(counter++ < exponent)
            {
                result = result * number;
                yield return result;
            }
        }
        /// <summary>
        /// 判断两个数是否能进行整除
        /// </summary>
        /// <param name="divisor">除数</param>
        /// <param name="dividend">被除数</param>
        /// <returns>如果能整除则返回<c>true</c>,否则返回<c>false</c></returns>
        public static bool Divisible(int divisor,int dividend)
        {
            return divisor % dividend == 0;
        }
        /// <summary>
        /// 判断给定的数字是否是素数
        /// </summary>
        /// <param name="num">需要判断的数字</param>
        /// <returns>
        ///   如果是素数，则返回<c>true</c>，否则返回<c>false</c>.
        /// </returns>
        public static bool IsPrime(int num)
        {
            if(num > 2)
            {
                if(Divisible(num,2))
                {
                    return false;
                }
                for(int i = 3;i <= Math.Sqrt(num);i += 2)
                {
                    if(Divisible(num,i))
                    {
                        return false;
                    }
                }
                return true;
            }
            else if(num == 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 列取指定范围内的所有素数，包含给定的最小值和最大值
        /// </summary>
        /// <param name="min">起始的数</param>
        /// <param name="max">结束的数</param>
        /// <returns></returns>)
        public static IEnumerable<int> GetPrimes(int min,int max)
        {
            for(int n = min;n <= max;n++)
            {
                if(IsPrime(n))
                {
                    yield return n;
                }
            }
        }
        /// <summary>
        /// 列取不大于指定数字范围之内的所有素数
        /// </summary>
        /// <param name="max">结束的数</param>
        /// <returns></returns>
        public static IEnumerable<int> GetPrimes(int max)
        {
            return GetPrimes(2,max);
        }

        /// <summary>
        /// 列取所有的排列（不可重复）
        /// </summary>
        /// <param name="baseValue"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<int[]> GetPermutations(int baseValue,int count)
        {
            if(count > baseValue || count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            else
            {
                int[] array = new int[count];
                bool[] flags = new bool[baseValue];
                return PermutationInternal(flags,array,baseValue,count,0);
            }
        }
        private static IEnumerable<int[]> PermutationInternal(bool[] flags,int[] array,int baseValue,int count,int curentIndex)
        {
            if(curentIndex == count)
            {
                yield return array;
            }
            else
            {
                for(int i = 1;i <= baseValue ;i++)
                {
                    if(flags[i - 1]) continue;
                    array[curentIndex] = i;
                    flags[i - 1] = true;
                    foreach(var v in PermutationInternal(flags,array,baseValue,count,curentIndex + 1))
                    {
                        yield return v;
                    }
                    flags[i - 1] = false;
                }
            }
        }
        /// <summary>
        /// 列取所有的组合
        /// </summary>
        /// <param name="baseValue"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<int[]> GetCombinations(int baseValue,int count)
        {
            if(count > baseValue || count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            else
            {
                int[] array = new int[count];
                return CombinationInternal(array,baseValue,count,0,0);
            }
        }
        private static IEnumerable<int[]> CombinationInternal(int[] array,int baseValue,int count,int curentIndex,int currentValue)
        {
            if(curentIndex == count)
            {
                yield return array;
            }
            else
            {
                for(int i = currentValue + 1;i <= baseValue;i++)
                {
                    array[curentIndex] = i;
                    foreach(var v in CombinationInternal(array,baseValue,count,curentIndex + 1,i))
                    {
                        yield return v;
                    }
                }
            }
        }
        /// <summary>
        /// 列取所有可重复的排列
        /// </summary>
        /// <param name="baseValue"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<int[]> GetPermutationExs(int baseValue,int count)
        {
            if(count > baseValue || count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            else
            {
                int[] array = new int[count];
                return PermutationExInternal(array,baseValue,count,0);
            }
        }
        private static IEnumerable<int[]> PermutationExInternal(int[] array,int baseValue,int count,int curentIndex)
        {
            if(curentIndex == count)
            {
                yield return array;
            }
            else
            {
                for(int i = 1;i <= baseValue;i++)
                {
                    array[curentIndex] = i;
                    foreach(var v in PermutationExInternal(array,baseValue,count,curentIndex + 1))
                    {
                        yield return v;
                    }
                }
            }
        }


        /// <summary>
        /// 计算多边形的面积
        /// </summary>
        /// <param name="allPoints">多边形的顶点坐标，要求按照顺时针或者逆时针排序</param>
        /// <returns></returns>
        public static double PolygonArea(this Descartes2DCoor[] allPoints)
        {
            if(allPoints == null || allPoints.Length < 2)
            {
                return 0.0; //不存在面积
            }
            int length = allPoints.Length;
            double area = 0.0;
            //for(int i = 0;i < length - 1;++i)
            //{
            //    area += integrationArea(allPoints[i],allPoints[i + 1]);
            //}
            ////最后一个点 和 开始的点
            //area += integrationArea(allPoints[length - 1],allPoints[0]);
            for(int i = 0;i < length;i++)
            {
                var point1 = allPoints[i];
                var point2 = allPoints[(i + 1) % length];
                area += (point2.X - point1.X) * (point2.Y + point1.Y);
            }
            return area >= 0.0 ? area / 2 : -area / 2;
        }

        #region 算术平均数
        /// <summary>
        /// 求输入的数组的算数平均数。
        /// </summary>
        /// <param name="nums">输入的数组</param>
        /// <returns>算数平均数</returns>
        public static double Average(double[] nums)
        {
            if (nums != null)
            {
                if (nums.Length == 0) return 0;
                double sum = 0;
                foreach (double d in nums)
                {
                    sum += d;
                }
                return sum / nums.Length;
            }
            else
            {
                throw new ArgumentNullException("nums");
            }
        }
        /// <summary>
        /// 求输入点的算术平均。
        /// </summary>
        /// <param name="points">输入点的数组</param>
        /// <returns>算数平均</returns>
        public static Descartes2DCoor Average(Descartes2DCoor[] points)
        {
            if (points != null)
            {
                if (points.Length == 0) return Descartes2DCoor.Zero;
                double sumx = 0f;
                double sumy = 0f;
                foreach (Descartes2DCoor pt in points)
                {
                    sumx += pt.X;
                    sumy += pt.Y;
                }
                return new Descartes2DCoor(sumx / points.Length, sumy / points.Length);
            }
            else
            {
                throw new ArgumentNullException("nums");
            }
        }

        /// <summary>
        /// 求输入坐标数组的X分量的算数平均数
        /// </summary>
        /// <param name="points">输入的坐标数组</param>
        /// <returns>算数平均数</returns>
        public static double AverageX(Descartes2DCoor[] points)
        {
            if (points != null)
            {
                if (points.Length == 0) return 0;
                double sum = 0f;
                foreach (Descartes2DCoor pt in points)
                {
                    sum += pt.X;
                }
                return (sum / points.Length);
            }
            else
            {
                throw new ArgumentNullException("nums");
            }
        }

        /// <summary>
        /// 求输入坐标数组的Y分量的算数平均数
        /// </summary>
        /// <param name="points">输入的坐标数组</param>
        /// <returns>算数平均数</returns>
        public static double AverageY(Descartes2DCoor[] points)
        {
            if (points != null)
            {
                if (points.Length == 0) return 0;
                double sum = 0f;
                foreach (Descartes2DCoor pt in points)
                {
                    sum += pt.Y;
                }
                return (sum / points.Length);
            }
            else
            {
                throw new ArgumentNullException("nums");
            }
        }
        #endregion

        #region 3D
        /// <summary>
        /// 求输入点的算术平均。
        /// </summary>
        /// <param name="points">输入点的数组</param>
        /// <returns>算数平均</returns>
        public static Descartes3DCoor Average(Descartes3DCoor[] points)
        {
            if (points != null)
            {
                if (points.Length == 0) return Descartes3DCoor.Zero;
                double sumx = 0f;
                double sumy = 0f;
                double sumz = 0f;
                foreach (Descartes3DCoor pt in points)
                {
                    sumx += pt.X;
                    sumy += pt.Y;
                    sumz += pt.Z;
                }
                return new Descartes3DCoor(sumx / points.Length, sumy / points.Length, sumz / points.Length);
            }
            else
            {
                throw new ArgumentNullException("nums");
            }
        }

        /// <summary>
        /// 求输入坐标数组的X分量的算数平均数
        /// </summary>
        /// <param name="points">输入的坐标数组</param>
        /// <returns>算数平均数</returns>
        public static double AverageX(Descartes3DCoor[] points)
        {
            if (points != null)
            {
                if (points.Length == 0) return 0;
                double sum = 0f;
                foreach (Descartes3DCoor pt in points)
                {
                    sum += pt.X;
                }
                return (sum / points.Length);
            }
            else
            {
                throw new ArgumentNullException("nums");
            }
        }
        /// <summary>
        /// 求输入坐标数组的Y分量的算数平均数
        /// </summary>
        /// <param name="points">输入的坐标数组</param>
        /// <returns>算数平均数</returns>
        public static double AverageY(Descartes3DCoor[] points)
        {
            if (points != null)
            {
                if (points.Length == 0) return 0;
                double sum = 0f;
                foreach (Descartes3DCoor pt in points)
                {
                    sum += pt.Y;
                }
                return (sum / points.Length);
            }
            else
            {
                throw new ArgumentNullException("nums");
            }
        }

        /// <summary>
        /// 求输入坐标数组的Z分量的算数平均数
        /// </summary>
        /// <param name="points">输入的坐标数组</param>
        /// <returns>算数平均数</returns>
        public static double AverageZ(Descartes3DCoor[] points)
        {
            if (points != null)
            {
                if (points.Length == 0) return 0;
                double sum = 0f;
                foreach (Descartes3DCoor pt in points)
                {
                    sum += pt.Z;
                }
                return (sum / points.Length);
            }
            else
            {
                throw new ArgumentNullException("nums");
            }
        }
        #endregion

        /// <summary>
        /// 求输入数组的算数平均数与各个数据的差的平方和。
        /// </summary>
        /// <param name="nums">输入的数组</param>
        /// <returns>计算结果</returns>
        public static double AverageSubSquareSum(double[] nums)
        {
            if (nums != null)
            {
                double ave = Average(nums);
                double result = 0;
                foreach (double d in nums)
                {
                    double temp = (ave - d);
                    result += temp * temp;
                }
                return result;
            }
            else
            {
                throw new ArgumentNullException("nums");
            }
        }
        /// <summary>
        /// 求输入数组的X分量的算数平均数与各个数据的差的平方和.
        /// </summary>
        /// <param name="datas">输入的数组</param>
        /// <returns>所求得的值</returns>
        public static double AverageSubSquareSumX(Descartes2DCoor[] datas)
        {
            if (datas != null)
            {
                double ave = AverageX(datas);
                double result = 0;
                foreach (Descartes2DCoor pt in datas)
                {
                    double temp = ave - pt.X;
                    result += temp * temp;
                }
                return result;
            }
            else
            {
                throw new ArgumentNullException("datas");
            }
        }
        /// <summary>
        /// 求输入数组的Y分量的算数平均数与各个数据的差的平方和.
        /// </summary>
        /// <param name="datas">输入的数组</param>
        /// <returns>所求得的值</returns>
        public static double AverageSubSquareSumY(Descartes2DCoor[] datas)
        {
            if (datas != null)
            {
                double ave = AverageY(datas);
                double result = 0;
                foreach (Descartes2DCoor pt in datas)
                {
                    double temp = ave - pt.Y;
                    result += temp * temp;
                }
                return result;
            }
            else
            {
                throw new ArgumentNullException("datas");
            }
        }
        /// <summary>
        ///  求输入数组的各个分量的算数平均数与各个数据的差的积和.
        /// </summary>
        /// <param name="datas">输入的数组</param>
        /// <returns>所求得的值</returns>
        public static float AverageSubProductSumXY(Descartes2DCoor[] datas)
        {
            if (datas != null)
            {
                double aveX = AverageX(datas);
                double aveY = AverageY(datas);
                double result = 0;
                foreach (Descartes2DCoor pt in datas)
                {
                    double temp = aveY - pt.Y;
                    result += (pt.Y - aveY) * (pt.X - aveX);
                }
                return (float)result;
            }
            else
            {
                throw new ArgumentNullException("datas");
            }
        }
        /// <summary>
        /// 求输入的数组的标准方差。
        /// </summary>
        /// <param name="nums">输入的数组的标准方差</param>
        /// <returns></returns>
        public static double StandardDeviation(double[] nums)
        {
            if (nums != null && nums.Length > 0)
            {
                double temp = AverageSubSquareSum(nums);
                return Math.Sqrt(temp / nums.Length);
            }
            else
            {
                throw new ArgumentNullException("nums");
            }
        }


        /// <summary>
        /// 求标准正态函数的值
        /// </summary>
        /// <param name="x">当前x的值</param>
        /// <returns>返回f(x)的值</returns>
        public static double GetStandardNormalDistributionValue(double x)
        {
            return GetNormalDistributionValue(0, 1, x);
        }
        /// <summary>
        /// 求正态曲线的函数值
        /// </summary>
        /// <param name="μ">期望值</param>
        /// <param name="σ">标准差</param>
        /// <param name="x">当前x的值</param>
        /// <returns>返回f(x)的值</returns>
        public static double GetNormalDistributionValue(double μ, double σ, double x)
        {
            return (1.0 / (Math.Sqrt(2 * Math.PI) * σ)) * Math.Exp(-(x - μ) * (x - μ) / (2 * σ * σ));
        }

        /// <summary>
        /// 求指定数字阶乘
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double Factorial(int n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException("n");
            double res = 1;
            for (int i = 2; i < n; i++)
            {
                res *= i;
            }
            return res;
        }
        /// <summary>
        /// 求指定数字阶乘
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static long Factorial_Int64(int n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException("n");
            long res = 1;
            for (int i = 2; i < n; i++)
            {
                res *= i;
            }
            return res;
        }
        /// <summary>
        /// 求指定数字阶乘
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int Factorial_Int32(int n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException("n");
            int res = 1;
            for (int i = 2; i < n; i++)
            {
                res *= i;
            }
            return res;
        }
        /// <summary>
        /// 求泊松分布的函数值
        /// </summary>
        /// <param name="λ"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static double GetPoissonDistributionValue(double λ, int k)
        {
            var val = Math.Exp(-λ);

            for (var i = 1; i <= k; i++)
            {
                val = val * λ / i;
            }
            return val;
        }
    }
}
