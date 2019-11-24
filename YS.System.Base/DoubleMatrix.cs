using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace System
{
    /// <summary>
    /// Matrix2 的摘要说明。
    /// 实现矩阵的基本运算
    /// </summary>
    public class DoubleMatrix
    {
        //私有数据成员
        private double[,] m_data;

        #region 构造函数

        /// <summary>
        /// 初始化 <see cref="DoubleMatrix" /> 的新实例.
        /// </summary>
        /// <param name="row">矩阵的秩</param>
        public DoubleMatrix(int row)
        {
            m_data = new double[row, row];

        }
        /// <summary>
        /// 初始化 <see cref="DoubleMatrix" /> 的新实例.
        /// </summary>
        /// <param name="row">矩阵的行</param>
        /// <param name="col">矩阵的列</param>
        public DoubleMatrix(int row, int col)
        {
            m_data = new double[row, col];
        }
        /// <summary>
        /// 根据已有的矩阵，初始化<see cref="DoubleMatrix"/>的新实例
        /// </summary>
        /// <param name="m">给定的矩阵</param>
        public DoubleMatrix(DoubleMatrix m)
        {
            int row = m.Row;
            int col = m.Column;
            m_data = new double[row, col];

            for (int i = 0; i < row; i++)
                for (int j = 0; j < col; j++)
                    m_data[i, j] = m[i, j];
        }
        #endregion


        /// <summary>
        /// 将矩阵设置为单位矩阵
        /// </summary>
        public void SetToUnit()
        {
            for (int i = 0; i < m_data.GetLength(0); i++)
                for (int j = 0; j < m_data.GetLength(1); j++)
                    m_data[i, j] = ((i == j) ? 1 : 0);
        }
        /// <summary>
        /// 将矩阵设置为零矩阵
        /// </summary>
        public void SetToZero()
        {
            for (int i = 0; i < m_data.GetLength(0); i++)
                for (int j = 0; j < m_data.GetLength(1); j++)
                    m_data[i, j] = 0;
        }

        /// <summary>
        /// 将矩阵的所有元素都设置为指定的值
        /// </summary>
        /// <param name="d">指定的值</param>
        public void SetToValue(double d)
        {
            for (int i = 0; i < m_data.GetLength(0); i++)
                for (int j = 0; j < m_data.GetLength(1); j++)
                    m_data[i, j] = d;
        }
        /// <summary>
        /// 将某一行设置为指定的值
        /// </summary>
        /// <param name="d"></param>
        public void SetRowToValue(int row, double d)
        {
            for (int j = 0; j < m_data.GetLength(1); j++)
                m_data[row, j] = d;
        }
        /// <summary>
        /// 将某一行设置为指定的值
        /// </summary>
        /// <param name="d"></param>
        public void SetColumnToValue(int column, double d)
        {
            for (int i = 0; i < m_data.GetLength(0); i++)
                m_data[i, column] = d;
        }
        #region 属性
        /// <summary>
        /// 获取矩阵的行数目
        /// </summary>
        public int Row
        {
            get
            {
                return m_data.GetLength(0);
            }
        }

        /// <summary>
        /// 获取矩阵的列数目
        /// </summary>
        public int Column
        {
            get
            {
                return m_data.GetLength(1);
            }
        }
        #endregion

        #region 索引器
        /// <summary>
        /// Gets or sets the <see cref="System.Double" /> with the specified row.
        /// </summary>
        /// <value>
        /// The <see cref="System.Double" />.
        /// </value>
        /// <param name="row">The row.</param>
        /// <param name="col">The col.</param>
        /// <returns></returns>
        public double this[int row, int col]
        {
            get
            {
                return m_data[row, col];
            }
            set
            {
                m_data[row, col] = value;
            }
        }
        #endregion


        /// <summary>
        /// 初等变换，对调两行
        /// </summary>
        /// <param name="i">行数1</param>
        /// <param name="j">行数2</param>
        public void Exchange(int i, int j)
        {
            double temp;

            for (int k = 0; k < Column; k++)
            {
                temp = m_data[i, k];
                m_data[i, k] = m_data[j, k];
                m_data[j, k] = temp;
            }
        }


        //初等变换　第index 行乘以mul
        DoubleMatrix Multiple(int index, double mul)
        {
            for (int j = 0; j < Column; j++)
            {
                m_data[index, j] *= mul;
            }
            return this;
        }


        //初等变换 第src行乘以mul加到第index行
        DoubleMatrix MultipleAdd(int index, int src, double mul)
        {
            for (int j = 0; j < Column; j++)
            {
                m_data[index, j] += m_data[src, j] * mul;
            }

            return this;
        }

        /// <summary>
        /// 获取矩阵的转置矩阵
        /// </summary>
        /// <returns></returns>
        public DoubleMatrix Transpose()
        {
            DoubleMatrix ret = new DoubleMatrix(Column, Row);

            for (int i = 0; i < Row; i++)
                for (int j = 0; j < Column; j++)
                {
                    ret[j, i] = m_data[i, j];
                }
            return ret;
        }

        #region 运算符重载
        //binary addition 矩阵加
        public static DoubleMatrix operator +(DoubleMatrix lhs, DoubleMatrix rhs)
        {
            if (lhs.Row != rhs.Row)    //异常
            {
                System.Exception e = new Exception("相加的两个矩阵的行数不等");
                throw e;
            }
            if (lhs.Column != rhs.Column)     //异常
            {
                System.Exception e = new Exception("相加的两个矩阵的列数不等");
                throw e;
            }

            int row = lhs.Row;
            int col = lhs.Column;
            DoubleMatrix ret = new DoubleMatrix(row, col);

            for (int i = 0; i < row; i++)
                for (int j = 0; j < col; j++)
                {
                    double d = lhs[i, j] + rhs[i, j];
                    ret[i, j] = d;
                }
            return ret;

        }

        //binary subtraction 矩阵减
        public static DoubleMatrix operator -(DoubleMatrix lhs, DoubleMatrix rhs)
        {
            if (lhs.Row != rhs.Row)    //异常
            {
                System.Exception e = new Exception("相减的两个矩阵的行数不等");
                throw e;
            }
            if (lhs.Column != rhs.Column)     //异常
            {
                System.Exception e = new Exception("相减的两个矩阵的列数不等");
                throw e;
            }

            int row = lhs.Row;
            int col = lhs.Column;
            DoubleMatrix ret = new DoubleMatrix(row, col);

            for (int i = 0; i < row; i++)
                for (int j = 0; j < col; j++)
                {
                    double d = lhs[i, j] - rhs[i, j];
                    ret[i, j] = d;
                }
            return ret;
        }


        //binary multiple 矩阵乘
        public static DoubleMatrix operator *(DoubleMatrix lhs, DoubleMatrix rhs)
        {
            if (lhs.Column != rhs.Row)    //异常
            {
                System.Exception e = new Exception("相乘的两个矩阵的行列数不匹配");
                throw e;
            }
            DoubleMatrix ret = new DoubleMatrix(lhs.Row, rhs.Column);
            double temp;
            for (int i = 0; i < lhs.Row; i++)
            {
                for (int j = 0; j < rhs.Column; j++)
                {
                    temp = 0;
                    for (int k = 0; k < lhs.Column; k++)
                    {
                        temp += lhs[i, k] * rhs[k, j];
                    }
                    ret[i, j] = temp;
                }
            }

            return ret;
        }


        //binary division 矩阵除
        public static DoubleMatrix operator /(DoubleMatrix lhs, DoubleMatrix rhs)
        {
            return lhs * rhs.Inverse();
        }

        //unary addition单目加
        public static DoubleMatrix operator +(DoubleMatrix m)
        {
            DoubleMatrix ret = new DoubleMatrix(m);
            return ret;
        }

        //unary subtraction 单目减
        public static DoubleMatrix operator -(DoubleMatrix m)
        {
            DoubleMatrix ret = new DoubleMatrix(m);
            for (int i = 0; i < ret.Row; i++)
                for (int j = 0; j < ret.Column; j++)
                {
                    ret[i, j] = -ret[i, j];
                }

            return ret;
        }

        //number multiple 数乘
        public static DoubleMatrix operator *(double d, DoubleMatrix m)
        {
            DoubleMatrix ret = new DoubleMatrix(m);
            for (int i = 0; i < ret.Row; i++)
                for (int j = 0; j < ret.Column; j++)
                    ret[i, j] *= d;

            return ret;
        }

        //number division 数除
        public static DoubleMatrix operator /(double d, DoubleMatrix m)
        {
            return d * m.Inverse();
        }
        #endregion
        //功能：返回列主元素的行号
        //参数：row为开始查找的行号
        //说明：在行号[row,Col)范围内查找第row列中绝对值最大的元素，返回所在行号
        int Pivot(int row)
        {
            int index = row;

            for (int i = row + 1; i < Row; i++)
            {
                if (m_data[i, row] > m_data[index, row])
                    index = i;
            }

            return index;
        }

        //inversion 逆阵：使用矩阵的初等变换，列主元素消去法
        public DoubleMatrix Inverse()
        {
            if (Row != Column)    //异常,非方阵
            {
                System.Exception e = new Exception("求逆的矩阵不是方阵");
                throw e;
            }
            // StreamWriter sw = new StreamWriter("..\\annex\\close_matrix.txt");
            DoubleMatrix tmp = new DoubleMatrix(this);
            DoubleMatrix ret = new DoubleMatrix(Row);    //单位阵
            ret.SetToUnit();

            int maxIndex;
            double dMul;

            for (int i = 0; i < Row; i++)
            {
                maxIndex = tmp.Pivot(i);

                if (tmp.m_data[maxIndex, i] == 0)
                {
                    System.Exception e = new Exception("求逆的矩阵的行列式的值等于0,");
                    throw e;
                }

                if (maxIndex != i)    //下三角阵中此列的最大值不在当前行，交换
                {
                    tmp.Exchange(i, maxIndex);
                    ret.Exchange(i, maxIndex);

                }

                ret.Multiple(i, 1 / tmp[i, i]);
                tmp.Multiple(i, 1 / tmp[i, i]);

                for (int j = i + 1; j < Row; j++)
                {
                    dMul = -tmp[j, i] / tmp[i, i];
                    tmp.MultipleAdd(j, i, dMul);

                }
                // sw.WriteLine("tmp=\r\n"+tmp);
                // sw.WriteLine("ret=\r\n"+ret);
            }//end for


            // sw.WriteLine("**=\r\n"+ this*ret);

            for (int i = Row - 1; i > 0; i--)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    dMul = -tmp[j, i] / tmp[i, i];
                    tmp.MultipleAdd(j, i, dMul);
                    ret.MultipleAdd(j, i, dMul);
                }
            }//end for


            //sw.WriteLine("tmp=\r\n"+tmp);
            //sw.WriteLine("ret=\r\n"+ret);
            //sw.WriteLine("***=\r\n"+ this*ret);
            //sw.Close();

            return ret;

        }//end Inverse

        #region


        #endregion

        /// <summary>
        /// 判断矩阵是否是方阵（行数目和列数目相等）
        /// </summary>
        /// <returns></returns>
        public bool IsSquare()
        {
            return Row == Column;
        }

        /// <summary>
        /// 判断矩阵是否是对称矩阵
        /// </summary>
        /// <returns></returns>
        public bool IsSymmetric()
        {
            if (Row != Column)
                return false;

            for (int i = 0; i < Row; i++)
                for (int j = i + 1; j < Column; j++)
                    if (m_data[i, j] != m_data[j, i])
                        return false;

            return true;
        }

        //一阶矩阵->实数
        public double ToDouble()
        {
            Trace.Assert(Row == 1 && Column == 1);

            return m_data[0, 0];
        }

        #region 重载ToString
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Column; j++)
                    s.AppendFormat("{0} ", m_data[i, j]);
                s.AppendLine();
            }
            return s.ToString();
        }
        #endregion

    }

    /// <summary>
    /// Matrix2 的摘要说明。
    /// 实现矩阵的基本运算
    /// </summary>
    public class SingleMatrix
    {
        //私有数据成员
        private float[,] m_data;

        #region 构造函数

        /// <summary>
        /// 初始化 <see cref="SingleMatrix" /> 的新实例.
        /// </summary>
        /// <param name="row">矩阵的秩</param>
        public SingleMatrix(int row)
        {
            m_data = new float[row, row];

        }
        /// <summary>
        /// 初始化 <see cref="SingleMatrix" /> 的新实例.
        /// </summary>
        /// <param name="row">矩阵的行</param>
        /// <param name="col">矩阵的列</param>
        public SingleMatrix(int row, int col)
        {
            m_data = new float[row, col];
        }
        /// <summary>
        /// 根据已有的矩阵，初始化<see cref="SingleMatrix"/>的新实例
        /// </summary>
        /// <param name="m">给定的矩阵</param>
        public SingleMatrix(SingleMatrix m)
        {
            int row = m.Row;
            int col = m.Column;
            m_data = new float[row, col];

            for (int i = 0; i < row; i++)
                for (int j = 0; j < col; j++)
                    m_data[i, j] = m[i, j];
        }
        #endregion


        /// <summary>
        /// 将矩阵设置为单位矩阵
        /// </summary>
        public void SetToUnit()
        {
            for (int i = 0; i < m_data.GetLength(0); i++)
                for (int j = 0; j < m_data.GetLength(1); j++)
                    m_data[i, j] = ((i == j) ? 1 : 0);
        }
        /// <summary>
        /// 将矩阵设置为零矩阵
        /// </summary>
        public void SetToZero()
        {
            for (int i = 0; i < m_data.GetLength(0); i++)
                for (int j = 0; j < m_data.GetLength(1); j++)
                    m_data[i, j] = 0;
        }

        /// <summary>
        /// 将矩阵的所有元素都设置为指定的值
        /// </summary>
        /// <param name="d">指定的值</param>
        public void SetToValue(float d)
        {
            for (int i = 0; i < m_data.GetLength(0); i++)
                for (int j = 0; j < m_data.GetLength(1); j++)
                    m_data[i, j] = d;
        }
        /// <summary>
        /// 将某一行设置为指定的值
        /// </summary>
        /// <param name="d"></param>
        public void SetRowToValue(int row, float d)
        {
            for (int j = 0; j < m_data.GetLength(1); j++)
                m_data[row, j] = d;
        }
        /// <summary>
        /// 将某一行设置为指定的值
        /// </summary>
        /// <param name="d"></param>
        public void SetColumnToValue(int column, float d)
        {
            for (int i = 0; i < m_data.GetLength(0); i++)
                m_data[i, column] = d;
         
        }
        #region 属性
        /// <summary>
        /// 获取矩阵的行数目
        /// </summary>
        public int Row
        {
            get
            {
                return m_data.GetLength(0);
            }
        }

        /// <summary>
        /// 获取矩阵的列数目
        /// </summary>
        public int Column
        {
            get
            {
                return m_data.GetLength(1);
            }
        }
        #endregion

        #region 索引器
        /// <summary>
        /// Gets or sets the <see cref="System.float" /> with the specified row.
        /// </summary>
        /// <value>
        /// The <see cref="System.float" />.
        /// </value>
        /// <param name="row">The row.</param>
        /// <param name="col">The col.</param>
        /// <returns></returns>
        public float this[int row, int col]
        {
            get
            {
                return m_data[row, col];
            }
            set
            {
                m_data[row, col] = value;
            }
        }
        #endregion


        /// <summary>
        /// 初等变换，对调两行
        /// </summary>
        /// <param name="i">行数1</param>
        /// <param name="j">行数2</param>
        public void Exchange(int i, int j)
        {
            float temp;

            for (int k = 0; k < Column; k++)
            {
                temp = m_data[i, k];
                m_data[i, k] = m_data[j, k];
                m_data[j, k] = temp;
            }
        }


        //初等变换　第index 行乘以mul
        SingleMatrix Multiple(int index, float mul)
        {
            for (int j = 0; j < Column; j++)
            {
                m_data[index, j] *= mul;
            }
            return this;
        }


        //初等变换 第src行乘以mul加到第index行
        SingleMatrix MultipleAdd(int index, int src, float mul)
        {
            for (int j = 0; j < Column; j++)
            {
                m_data[index, j] += m_data[src, j] * mul;
            }

            return this;
        }

        /// <summary>
        /// 获取矩阵的转置矩阵
        /// </summary>
        /// <returns></returns>
        public SingleMatrix Transpose()
        {
            SingleMatrix ret = new SingleMatrix(Column, Row);

            for (int i = 0; i < Row; i++)
                for (int j = 0; j < Column; j++)
                {
                    ret[j, i] = m_data[i, j];
                }
            return ret;
        }

        #region 运算符重载
        //binary addition 矩阵加
        public static SingleMatrix operator +(SingleMatrix lhs, SingleMatrix rhs)
        {
            if (lhs.Row != rhs.Row)    //异常
            {
                System.Exception e = new Exception("相加的两个矩阵的行数不等");
                throw e;
            }
            if (lhs.Column != rhs.Column)     //异常
            {
                System.Exception e = new Exception("相加的两个矩阵的列数不等");
                throw e;
            }

            int row = lhs.Row;
            int col = lhs.Column;
            SingleMatrix ret = new SingleMatrix(row, col);

            for (int i = 0; i < row; i++)
                for (int j = 0; j < col; j++)
                {
                    float d = lhs[i, j] + rhs[i, j];
                    ret[i, j] = d;
                }
            return ret;

        }

        //binary subtraction 矩阵减
        public static SingleMatrix operator -(SingleMatrix lhs, SingleMatrix rhs)
        {
            if (lhs.Row != rhs.Row)    //异常
            {
                System.Exception e = new Exception("相减的两个矩阵的行数不等");
                throw e;
            }
            if (lhs.Column != rhs.Column)     //异常
            {
                System.Exception e = new Exception("相减的两个矩阵的列数不等");
                throw e;
            }

            int row = lhs.Row;
            int col = lhs.Column;
            SingleMatrix ret = new SingleMatrix(row, col);

            for (int i = 0; i < row; i++)
                for (int j = 0; j < col; j++)
                {
                    float d = lhs[i, j] - rhs[i, j];
                    ret[i, j] = d;
                }
            return ret;
        }


        //binary multiple 矩阵乘
        public static SingleMatrix operator *(SingleMatrix lhs, SingleMatrix rhs)
        {
            if (lhs.Column != rhs.Row)    //异常
            {
                System.Exception e = new Exception("相乘的两个矩阵的行列数不匹配");
                throw e;
            }
            SingleMatrix ret = new SingleMatrix(lhs.Row, rhs.Column);
            float temp;
            for (int i = 0; i < lhs.Row; i++)
            {
                for (int j = 0; j < rhs.Column; j++)
                {
                    temp = 0;
                    for (int k = 0; k < lhs.Column; k++)
                    {
                        temp += lhs[i, k] * rhs[k, j];
                    }
                    ret[i, j] = temp;
                }
            }

            return ret;
        }


        //binary division 矩阵除
        public static SingleMatrix operator /(SingleMatrix lhs, SingleMatrix rhs)
        {
            return lhs * rhs.Inverse();
        }

        //unary addition单目加
        public static SingleMatrix operator +(SingleMatrix m)
        {
            SingleMatrix ret = new SingleMatrix(m);
            return ret;
        }

        //unary subtraction 单目减
        public static SingleMatrix operator -(SingleMatrix m)
        {
            SingleMatrix ret = new SingleMatrix(m);
            for (int i = 0; i < ret.Row; i++)
                for (int j = 0; j < ret.Column; j++)
                {
                    ret[i, j] = -ret[i, j];
                }

            return ret;
        }

        //number multiple 数乘
        public static SingleMatrix operator *(float d, SingleMatrix m)
        {
            SingleMatrix ret = new SingleMatrix(m);
            for (int i = 0; i < ret.Row; i++)
                for (int j = 0; j < ret.Column; j++)
                    ret[i, j] *= d;

            return ret;
        }

        //number division 数除
        public static SingleMatrix operator /(float d, SingleMatrix m)
        {
            return d * m.Inverse();
        }
        #endregion
        //功能：返回列主元素的行号
        //参数：row为开始查找的行号
        //说明：在行号[row,Col)范围内查找第row列中绝对值最大的元素，返回所在行号
        int Pivot(int row)
        {
            int index = row;

            for (int i = row + 1; i < Row; i++)
            {
                if (m_data[i, row] > m_data[index, row])
                    index = i;
            }

            return index;
        }

        //inversion 逆阵：使用矩阵的初等变换，列主元素消去法
        public SingleMatrix Inverse()
        {
            if (Row != Column)    //异常,非方阵
            {
                System.Exception e = new Exception("求逆的矩阵不是方阵");
                throw e;
            }
            // StreamWriter sw = new StreamWriter("..\\annex\\close_matrix.txt");
            SingleMatrix tmp = new SingleMatrix(this);
            SingleMatrix ret = new SingleMatrix(Row);    //单位阵
            ret.SetToUnit();

            int maxIndex;
            float dMul;

            for (int i = 0; i < Row; i++)
            {
                maxIndex = tmp.Pivot(i);

                if (tmp.m_data[maxIndex, i] == 0)
                {
                    System.Exception e = new Exception("求逆的矩阵的行列式的值等于0,");
                    throw e;
                }

                if (maxIndex != i)    //下三角阵中此列的最大值不在当前行，交换
                {
                    tmp.Exchange(i, maxIndex);
                    ret.Exchange(i, maxIndex);

                }

                ret.Multiple(i, 1 / tmp[i, i]);
                tmp.Multiple(i, 1 / tmp[i, i]);

                for (int j = i + 1; j < Row; j++)
                {
                    dMul = -tmp[j, i] / tmp[i, i];
                    tmp.MultipleAdd(j, i, dMul);

                }
                // sw.WriteLine("tmp=\r\n"+tmp);
                // sw.WriteLine("ret=\r\n"+ret);
            }//end for


            // sw.WriteLine("**=\r\n"+ this*ret);

            for (int i = Row - 1; i > 0; i--)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    dMul = -tmp[j, i] / tmp[i, i];
                    tmp.MultipleAdd(j, i, dMul);
                    ret.MultipleAdd(j, i, dMul);
                }
            }//end for


            //sw.WriteLine("tmp=\r\n"+tmp);
            //sw.WriteLine("ret=\r\n"+ret);
            //sw.WriteLine("***=\r\n"+ this*ret);
            //sw.Close();

            return ret;

        }//end Inverse

        #region


        #endregion

        /// <summary>
        /// 判断矩阵是否是方阵（行数目和列数目相等）
        /// </summary>
        /// <returns></returns>
        public bool IsSquare()
        {
            return Row == Column;
        }

        /// <summary>
        /// 判断矩阵是否是对称矩阵
        /// </summary>
        /// <returns></returns>
        public bool IsSymmetric()
        {
            if (Row != Column)
                return false;

            for (int i = 0; i < Row; i++)
                for (int j = i + 1; j < Column; j++)
                    if (m_data[i, j] != m_data[j, i])
                        return false;

            return true;
        }

        //一阶矩阵->实数
        public float Tofloat()
        {
            Trace.Assert(Row == 1 && Column == 1);

            return m_data[0, 0];
        }

        #region 重载ToString
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Column; j++)
                    s.AppendFormat("{0} ", m_data[i, j]);
                s.AppendLine();
            }
            return s.ToString();
        }
        #endregion

    }
}

