using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System
{
    /// <summary>
    /// 表示笛卡尔三维坐标
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [TypeConverter(typeof(Descartes3DCoorConverter))]
    [Serializable]
    public struct Descartes3DCoor
    {
           /// <summary>
        /// 表示坐标原点。
        /// </summary>
        public static readonly Descartes3DCoor Zero;
        /// <summary>
        /// 初始化 <see cref="Descartes3DCoor"/> 的静态数据。
        /// </summary>
        static Descartes3DCoor () {
            Zero = new Descartes3DCoor();
        }
        private double x;
        private double y;
        private double z;
        /// <summary>
        /// 初始化 <see cref="Descartes3DCoor"/> 的新实例。
        /// </summary>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <param name="z">z坐标</param>
        public Descartes3DCoor (double x, double y ,double z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        /// <summary>
        /// 获取一个值，该值反应了当前结构是否是坐标原点。
        /// </summary>
        [Browsable(false)]
        public bool IsZero {
            get {
                return ((this.x == default(double)) && (this.y == default(double))&& (this.z ==default(double)));
            }
        }
        /// <summary>
        /// 获取或设置X坐标
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
        /// 获取或设置Y坐标
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
        /// 获取或设置Z坐标
        /// </summary>
        public double Z {
            get {
                return this.z;
            }
            set {
                this.z = value;
            }
        }
        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="pt">The pt.</param>
        /// <param name="sz">The sz.</param>
        /// <returns>The result of the operator.</returns>
        public static Descartes3DCoor operator + (Descartes3DCoor pt, Descartes3DCoor sz) {
            return Add(pt, sz);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="pt">The pt.</param>
        /// <param name="sz">The sz.</param>
        /// <returns>The result of the operator.</returns>
        public static Descartes3DCoor operator - (Descartes3DCoor pt, Descartes3DCoor sz) {
            return Subtract(pt, sz);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator == (Descartes3DCoor left, Descartes3DCoor right) {
            return ((left.X == right.X) && (left.Y == right.Y) &&(left.Z==right .Z));
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator != (Descartes3DCoor left, Descartes3DCoor right) {
            return !(left == right);
        }

        /// <summary>
        /// 将指定的大小加与坐标相加。
        /// </summary>
        /// <param name="pt">坐标位置</param>
        /// <param name="sz">指定的大小</param>
        /// <returns>返回新坐标</returns>
        public static Descartes3DCoor Add (Descartes3DCoor pt, Descartes3DCoor sz) {
            return new Descartes3DCoor(pt.X + sz.X, pt.Y + sz.Y,pt.Z +sz.Z);
            
        }

        /// <summary>
        /// 将坐标与指定的大小相加。
        /// </summary>
        /// <param name="pt">坐标位置</param>
        /// <param name="sz">指定的大小</param>
        /// <returns>返回新坐标</returns>
        public static Descartes3DCoor Subtract (Descartes3DCoor pt, Descartes3DCoor sz) {
            return new Descartes3DCoor(pt.X - sz.X, pt.Y - sz.Y,pt.Z -sz.Z);
        }
        /// <summary>
        /// 将二维平面坐标转为三维坐标。
        /// </summary>
        /// <param name="point">二维坐标点</param>
        /// <returns>返回转换后的三维坐标</returns>
        public static Descartes3DCoor From2DCoor (Descartes2DCoor point) {
            return new Descartes3DCoor(point.X, point.Y,default(double));
        }
       
        /// <summary>
        /// 测试指定的对象是否为<see cref="System.Mathematics.Descartes3DCoor"/>类型并等效于此<see cref="System.Mathematics.Descartes3DCoor"/>类型。
        /// </summary>
        /// <param name="obj">要测试的对象。</param>
        /// <returns>
        /// 如果指定的对象等效于当前的<see cref="Descartes3DCoor"/>，则为 true；否则为 false。
        /// </returns>
        public override bool Equals (object obj) {
            if (!(obj is Descartes3DCoor)) {
                return false;
            }
            Descartes3DCoor tf = (Descartes3DCoor)obj;
            return ((tf.X == this.X) && (tf.Y == this.Y) &&(tf.Z==this.Z));
        }

        /// <summary>
        /// 返回此实例的哈希代码。
        /// </summary>
        /// <returns>一个整数值，用于指定此实例的哈希代码。</returns>
        public override int GetHashCode () {
            return this.X.GetHashCode()^this.Y .GetHashCode()^this.Z.GetHashCode();
        }

        /// <summary>
        /// 返回一个 <see cref="System.String"/> ，表示当前的对象。
        /// </summary>
        /// <returns>表示当前对象的<see cref="System.String"/> 。</returns>
        public override string ToString () {
            return string.Format(CultureInfo.CurrentCulture, "{{X={0}, Y={1} ,Z={2}}}", new object[] { this.x, this.y,this.z });
        }

    }

    /// <summary>
    /// 提供 <see cref="System.Mathematics.Descartes3DCoor"/> 的转换功能。
    /// </summary>
    public class Descartes3DCoorConverter : TypeConverter
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
            if (numArray.Length != 3) {
                throw new ArgumentException();
            }
            return new Descartes3DCoor(numArray[0], numArray[1],numArray[2]);
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
            if (value is Descartes3DCoor) {
                if (destinationType == typeof(string)) {
                    Descartes3DCoor point = (Descartes3DCoor)value;
                    if (culture == null) {
                        culture = CultureInfo.CurrentCulture;
                    }
                    string separator = culture.TextInfo.ListSeparator + " ";
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(double));
                    string[] strArray = new string[3];
                    int num = 0;
                    strArray[num++] = converter.ConvertToString(context, culture, point.X);
                    strArray[num++] = converter.ConvertToString(context, culture, point.Y);
                    strArray[num++] = converter.ConvertToString(context, culture, point.Z);
                    return string.Join(separator, strArray);
                }
                if (destinationType == typeof(InstanceDescriptor)) {
                    Descartes3DCoor point2 = (Descartes3DCoor)value;
                    ConstructorInfo constructor = typeof(Descartes3DCoor).GetConstructor(new Type[] { typeof(double), typeof(double),typeof(double) });
                    if (constructor != null) {
                        return new InstanceDescriptor(constructor, new object[] { point2.X, point2.Y,point2.Z });
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
            object obj4 = propertyValues["Z"];
            if (((obj2 == null) || (obj3 == null)) ||(obj4==null)|| !(obj2 is double) || !(obj3 is double)|| (!(obj4 is double))){
                throw new ArgumentException("PropertyValueInvalidEntry");
            }
            return new Descartes3DCoor((double)obj2, (double)obj3,(double)obj4);
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
            return TypeDescriptor.GetProperties(typeof(Descartes3DCoor), attributes).Sort(new string[] { "X", "Y","Z"});
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
