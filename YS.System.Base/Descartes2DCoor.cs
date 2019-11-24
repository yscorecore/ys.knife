using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Collections;

namespace System
{
    /// <summary>
    /// 表示笛卡尔二维坐标
    /// </summary>
    [TypeConverter(typeof(Descartes2DCoorConverter))]
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct Descartes2DCoor
    {
        /// <summary>
        /// 表示坐标原点。
        /// </summary>
        public static readonly Descartes2DCoor Zero;
        /// <summary>
        /// 初始化 <see cref="Descartes2DCoor"/> 的静态数据。
        /// </summary>
        static Descartes2DCoor () {
            Zero = new Descartes2DCoor();
        }
        private double x;
        private double y;

        /// <summary>
        /// 初始化 <see cref="Descartes2DCoor"/> 的新实例。
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public Descartes2DCoor (double x, double y) {
            this.x = x;
            this.y = y;
        }
        /// <summary>
        /// 获取一个值，该值反应了当前结构是否是坐标原点。
        /// </summary>
        [Browsable(false)]
        public bool IsZero {
            get {
                return ((this.x == default(double)) && (this.y == default(double)));
            }
        }
        /// <summary>
        /// 获取或设置横坐标
        /// </summary>
        public double X {
            get {
                return this.x;
            }
            set {
                this.x = value;
            }
        }
        /// <summary>
        /// 获取或设置纵坐标
        /// </summary>
        public double Y {
            get {
                return this.y;
            }
            set {
                this.y = value;
            }
        }
        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="pt">The pt.</param>
        /// <param name="sz">The sz.</param>
        /// <returns>The result of the operator.</returns>
        public static Descartes2DCoor operator + (Descartes2DCoor pt, Descartes2DCoor sz) {
            return Add(pt, sz);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="pt">The pt.</param>
        /// <param name="sz">The sz.</param>
        /// <returns>The result of the operator.</returns>
        public static Descartes2DCoor operator - (Descartes2DCoor pt, Descartes2DCoor sz) {
            return Subtract(pt, sz);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator == (Descartes2DCoor left, Descartes2DCoor right) {
            return ((left.X == right.X) && (left.Y == right.Y));
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator != (Descartes2DCoor left, Descartes2DCoor right) {
            return !(left == right);
        }

        /// <summary>
        /// 将指定的大小加与坐标相加。
        /// </summary>
        /// <param name="pt">坐标位置</param>
        /// <param name="sz">指定的大小</param>
        /// <returns>返回新坐标</returns>
        public static Descartes2DCoor Add (Descartes2DCoor pt, Descartes2DCoor sz) {
            return new Descartes2DCoor(pt.X + sz.X, pt.Y + sz.Y);
            
        }

        /// <summary>
        /// 将坐标与指定的大小相加。
        /// </summary>
        /// <param name="pt">坐标位置</param>
        /// <param name="sz">指定的大小</param>
        /// <returns>返回新坐标</returns>
        public static Descartes2DCoor Subtract (Descartes2DCoor pt, Descartes2DCoor sz) {
            return new Descartes2DCoor(pt.X - sz.X, pt.Y - sz.Y);
        }
        ///// <summary>
        ///// 将指定的<see cref="System.Drawing.Point"/>结构转换为<see cref="Descartes2DCoor"/>结构
        ///// </summary>
        ///// <param name="point">指定的<see cref="System.Drawing.Point"/>结构</param>
        ///// <returns>对应的<see cref="Descartes2DCoor"/>结构</returns>
        //public static Descartes2DCoor FromPoint(Point point)
        //{
        //      return new Descartes2DCoor(point.X, point.Y);
        //}
        ///// <summary>
        ///// 将指定的<see cref="System.Drawing.PointF"/>结构转换为<see cref="Descartes2DCoor"/>结构
        ///// </summary>
        ///// <param name="point">指定的<see cref="System.Drawing.Point"/>结构</param>
        //public static Descartes2DCoor FromPoint (PointF point) {
        //    return new Descartes2DCoor(point.X, point.Y);
        //}
        /// <summary>
        /// 测试指定的对象是否为<see cref="System.Mathematics.Descartes2DCoor"/>类型并等效于此<see cref="System.Mathematics.Descartes2DCoor"/>类型。
        /// </summary>
        /// <param name="obj">要测试的对象。</param>
        /// <returns>
        /// 如果指定的对象等效于当前的<see cref="Descartes2DCoor"/>，则为 true；否则为 false。
        /// </returns>
        public override bool Equals (object obj) {
            if (!(obj is Descartes2DCoor)) {
                return false;
            }
            Descartes2DCoor tf = (Descartes2DCoor)obj;
            return ((tf.X == this.X) && (tf.Y == this.Y));
        }

        /// <summary>
        /// 返回此实例的哈希代码。
        /// </summary>
        /// <returns>一个整数值，用于指定此实例的哈希代码。</returns>
        public override int GetHashCode () {
            return this.X.GetHashCode()^this.Y .GetHashCode();
        }

        /// <summary>
        /// 返回一个 <see cref="System.String"/> ，表示当前的对象。
        /// </summary>
        /// <returns>表示当前对象的<see cref="System.String"/> 。</returns>
        public override string ToString () {
            return string.Format(CultureInfo.CurrentCulture, "{{X={0}, Y={1}}}", new object[] { this.x, this.y });
        }

    }

    /// <summary>
    /// 提供 <see cref="System.Mathematics.Descartes2DCoor"/> 的转换功能。
    /// </summary>
    public sealed class Descartes2DCoorConverter : TypeConverter
    {
        /// <summary>
        /// 返回该转换器是否可以使用指定的上下文将给定类型的对象转换为此转换器的类型。
        /// </summary>
        /// <param name="context"><see cref="T:System.ComponentModel.ITypeDescriptorContext"/>，提供格式上下文。</param>
        /// <param name="sourceType">一个 <see cref="T:System.Type"/>，表示要转换的类型。</param>
        /// <returns>如果该转换器能够执行转换，则为 true；否则为 false。</returns>
        public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType) {
            return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
        }

        /// <summary>
        /// 返回此转换器是否可以使用指定的上下文将该对象转换为指定的类型。
        /// </summary>
        /// <param name="context"><see cref="T:System.ComponentModel.ITypeDescriptorContext"/>，提供格式上下文。</param>
        /// <param name="destinationType">一个 <see cref="T:System.Type"/>，表示要转换到的类型。</param>
        /// <returns>如果该转换器能够执行转换，则为 true；否则为 false。</returns>
        public override bool CanConvertTo (ITypeDescriptorContext context, Type destinationType) {
            return ((destinationType == typeof(InstanceDescriptor)) || base.CanConvertTo(context, destinationType));
        }

        /// <summary>
        /// 使用指定的上下文和区域性信息将给定的对象转换为此转换器的类型。
        /// </summary>
        /// <param name="context"><see cref="T:System.ComponentModel.ITypeDescriptorContext"/>，提供格式上下文。</param>
        /// <param name="culture">用作当前区域性的 <see cref="T:System.Globalization.CultureInfo"/>。</param>
        /// <param name="value">要转换的 <see cref="T:System.Object"/>。</param>
        /// <returns>
        /// 表示转换的 value 的 <see cref="T:System.Object"/>。
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">不能执行转换。</exception>
        public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value) {
            string str = value as string;
            if (str == null) {
                return base.ConvertFrom(context, culture, value);
            }
            string str2 = str.Trim();
            if (str2.Length == 0) {
                return null;
            }
            if (culture == null) {
                culture = CultureInfo.CurrentCulture;
            }
            char ch = culture.TextInfo.ListSeparator[0];
            string[] strArray = str2.Split(new char[] { ch });
            double[] numArray = new double[strArray.Length];
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(double));
            for (int i = 0; i < numArray.Length; i++) {
                numArray[i] = (double)converter.ConvertFromString(context, culture, strArray[i]);
            }
            if (numArray.Length != 2) {
                throw new ArgumentException();
            }
            return new Descartes2DCoor(numArray[0], numArray[1]);
        }

        /// <summary>
        /// 使用指定的上下文和区域性信息将给定的值对象转换为指定的类型。
        /// </summary>
        /// <param name="context"><see cref="T:System.ComponentModel.ITypeDescriptorContext"/>，提供格式上下文。</param>
        /// <param name="culture"><see cref="T:System.Globalization.CultureInfo"/>。如果传递 null，则采用当前区域性。</param>
        /// <param name="value">要转换的 <see cref="T:System.Object"/>。</param>
        /// <param name="destinationType"><paramref name="value"/> 参数要转换成的 <see cref="T:System.Type"/>。</param>
        /// <returns>
        /// 表示转换的 value 的 <see cref="T:System.Object"/>。
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="destinationType"/> 参数为 null。</exception>
        /// <exception cref="T:System.NotSupportedException">不能执行转换。</exception>
        public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (destinationType == null) {
                throw new ArgumentNullException("destinationType");
            }
            if (value is Descartes2DCoor) {
                if (destinationType == typeof(string)) {
                    Descartes2DCoor point = (Descartes2DCoor)value;
                    if (culture == null) {
                        culture = CultureInfo.CurrentCulture;
                    }
                    string separator = culture.TextInfo.ListSeparator + " ";
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(double));
                    string[] strArray = new string[2];
                    int num = 0;
                    strArray[num++] = converter.ConvertToString(context, culture, point.X);
                    strArray[num++] = converter.ConvertToString(context, culture, point.Y);
                    return string.Join(separator, strArray);
                }
                if (destinationType == typeof(InstanceDescriptor)) {
                    Descartes2DCoor point2 = (Descartes2DCoor)value;
                    ConstructorInfo constructor = typeof(Descartes2DCoor).GetConstructor(new Type[] { typeof(double), typeof(double) });
                    if (constructor != null) {
                        return new InstanceDescriptor(constructor, new object[] { point2.X, point2.Y });
                    }
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }


        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="propertyValues">The property values.</param>
        /// <returns></returns>
        public override object CreateInstance (ITypeDescriptorContext context, IDictionary propertyValues) {
            if (propertyValues == null) {
                throw new ArgumentNullException("propertyValues");
            }
            object obj2 = propertyValues["X"];
            object obj3 = propertyValues["Y"];
            if (((obj2 == null) || (obj3 == null)) || (!(obj2 is double) || !(obj3 is double))) {
                throw new ArgumentException("PropertyValueInvalidEntry");
            }
            return new Descartes2DCoor((double)obj2, (double)obj3);
        }

        /// <summary>
        /// 返回有关更改该对象上的某个值是否需要使用指定的上下文调用 <see cref="M:System.ComponentModel.TypeConverter.CreateInstance(System.Collections.IDictionary)"/> 以创建新值的情况。
        /// </summary>
        /// <param name="context"><see cref="T:System.ComponentModel.ITypeDescriptorContext"/>，提供格式上下文。</param>
        /// <returns>
        /// 如果更改此对象的属性需要调用 <see cref="M:System.ComponentModel.TypeConverter.CreateInstance(System.Collections.IDictionary)"/> 来创建新值，则为 true；否则为 false。
        /// </returns>
        public override bool GetCreateInstanceSupported (ITypeDescriptorContext context) {
            return true;
        }

        /// <summary>
        /// 使用指定的上下文和属性 (Attribute) 返回由 value 参数指定的数组类型的属性 (Property) 的集合。
        /// </summary>
        /// <param name="context"><see cref="T:System.ComponentModel.ITypeDescriptorContext"/>，提供格式上下文。</param>
        /// <param name="value">一个 <see cref="T:System.Object"/>，指定要为其获取属性的数组类型。</param>
        /// <param name="attributes">用作筛选器的 <see cref="T:System.Attribute"/> 类型数组。</param>
        /// <returns>
        /// 具有为此数据类型公开的属性的 <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/>；或者如果没有属性，则为 null。
        /// </returns>
        public override PropertyDescriptorCollection GetProperties (ITypeDescriptorContext context, object value, Attribute[] attributes) {
            return TypeDescriptor.GetProperties(typeof(Descartes2DCoor), attributes).Sort(new string[] { "X", "Y" });
        }

        /// <summary>
        /// 使用指定的上下文返回该对象是否支持属性 (Property)。
        /// </summary>
        /// <param name="context"><see cref="T:System.ComponentModel.ITypeDescriptorContext"/>，提供格式上下文。</param>
        /// <returns>
        /// 如果应调用 <see cref="M:System.ComponentModel.TypeConverter.GetProperties(System.Object)"/> 来查找此对象的属性，则为 true；否则为 false。
        /// </returns>
        public override bool GetPropertiesSupported (ITypeDescriptorContext context) {
            return true;
        }

    }
}
