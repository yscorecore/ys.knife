using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace System.Record
{
    /// <summary>
    /// 表示对属性的操作。
    /// </summary>
    public class PropertyOperation:OperationBase
    {
        #region 私有成员变量
        private Object m_Target;
        private Object m_OldValue;
        private Object m_NewValue;
        private PropertyInfo m_propinfo;
        #endregion

        #region 构造函数
        /// <summary>
        /// 用指定的信息初始化<see cref="PropertyOperation"/>的新实例。
        /// </summary>
        /// <param name="targer">要操作的目标对象</param>
        /// <param name="prop"></param>
        /// <param name="oldvalue"></param>
        /// <param name="newvalue"></param>
        public PropertyOperation(Object targer, String prop, Object oldvalue, Object newvalue)
        {
            this.Target = targer;
            this.Property = prop;
            this.OldValue = oldvalue;
            this.NewValue = newvalue;
            
        }
        #endregion

        #region 属性
        /// <summary>
        /// 目标对象
        /// </summary>
        public Object Target
        {
            get
            {
                return this.m_Target;
            }
            private set
            {
                if (value == null) 
                    throw new ArgumentNullException("操作对象不能为空。");
                if (value is ValueType)
                {
                    throw new ArgumentException("操作对象不能为值类型");
                }
                this.m_Target = value;
              
            }
        }
        /// <summary>
        /// 属性名称
        /// </summary>
        public String Property
        {
            get
            {
                return m_propinfo.Name;
            }
            private  set
            {
                if (value == null || value.Trim() == String.Empty)
                {
                    throw new ArgumentNullException("属性名称不能为空。");
                }
                else
                {
                    if (this.m_Target != null)
                    {
                        m_propinfo = this.Target.GetType().GetProperty(value.Trim());
                        if (m_propinfo == null)
                        {
                            throw new ArgumentException(String.Format("类型{0}中不存在属性{1}。",this.Target.GetType().ToString(),value));
                        }
                        if (!m_propinfo.CanRead)
                        {
                            throw new ArgumentException(string.Format("指定的属性“{0}”不可读", value));
                        }
                        if (!m_propinfo.CanWrite)
                        {
                            throw new ArgumentException(string.Format("指定的属性“{0}”不可写", value));
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 操作前的值
        /// </summary>
        public Object OldValue
        {
            get
            {
                return this.m_OldValue;
            }
            private set
            {
                this.m_OldValue = value;
            }
        }
        /// <summary>
        /// 操作后的值
        /// </summary>
        public Object NewValue
        {
            get
            {
                return this.m_NewValue;
            }
            private set
            {
                this.m_NewValue = value;
            }
        }


        #endregion

        #region 公共方法
        /// <summary>
        /// 将两个操作进行合并，合并要求操作ID和属性均相同，否则不进行操作
        /// 合并后新值是item的新值，旧值是本操作的旧值
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void Merge (PropertyOperation item)
        {
            if (item == null) return;
            if (this.Target!=item.Target) return;
            if (this.Property.Trim() != item.Property.Trim()) return;
            this.NewValue = item.NewValue;
        }
        /// <summary>
        /// 将目标对象的值设置为新值
        /// </summary>
        public override void SetNewValue()
        {
            if (m_propinfo == null) return;
            m_propinfo.SetValue(this.Target, this.NewValue, null);

        }
        /// <summary>
        /// 将目标对性的值设置为原来的值
        /// </summary>
        public override void SetOldValue ()
        {
            if (m_propinfo == null) return;
            m_propinfo.SetValue(this.Target, this.OldValue, null);
            
        }
        #endregion
    }
}
