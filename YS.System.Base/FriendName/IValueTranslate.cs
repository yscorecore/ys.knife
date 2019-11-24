using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Globalization;
using System.IO;

namespace System
{

    /// <summary>
    /// 表示元数据类型转换成另外一种类型的接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IValueTranslate<TFrom, TTo>
    {
        TTo GetTranslateValue(TFrom fromValue);
    }

    public class ManageredResourceValueTranslate<TFrom, TTo> : IValueTranslate<TFrom, TTo>
    {

        #region 构造函数
        public ManageredResourceValueTranslate()
        {

        }
        public ManageredResourceValueTranslate(ResourceManager resourceManager)
        {
            if (resourceManager == null) throw new ArgumentNullException("resourceManager");
            this.ResourceManager = resourceManager;
        }
        public ManageredResourceValueTranslate(ResourceManager resourceManager, CultureInfo cultureInfo)
        {
            if (resourceManager == null) throw new ArgumentNullException("resourceManager");
            this.ResourceManager = resourceManager;
            this.CultureInfo = cultureInfo;
        }
        #endregion

        public virtual ResourceManager ResourceManager { get; protected set; }

        public virtual CultureInfo CultureInfo { get; set; }


        public TTo GetTranslateValue(TFrom fromValue)
        {
            if (ResourceManager == null) throw new ApplicationException("the ResourceManager should not be null");
            string key = GetResourceKey(fromValue);
            return GetTranslateValueByResourceKey(ResourceManager, CultureInfo, key);
        }

        protected virtual string GetResourceKey(TFrom fromValue)
        {
            return Convert.ToString(fromValue);
        }

        protected virtual TTo GetTranslateValueByResourceKey(ResourceManager resourceManager, CultureInfo cultureInfo, string key)
        {
            
            if (typeof(TTo) == typeof(string))
            {
                return (TTo)(object)resourceManager.GetString(key, cultureInfo);
            }
            else if (typeof(Stream).IsAssignableFrom(typeof(TTo)))
            {
                return (TTo)(object)resourceManager.GetStream(key, cultureInfo);
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }

    public static class ValueTranslateEX
    {
        //public static TTo GetTranslateValue<TFrom, TTo>(TFrom fromValue, IValueTranslate<TFrom, TTo> translator)
        //{
        //    if (translator == null) throw new ArgumentNullException("translator");
        //    return translator.GetTranslateValue(fromValue);
        //}
        public static string GetTranslateString<TFrom>(this TFrom fromValue, IValueTranslate<TFrom, string> translator)
        {
            if (translator == null) throw new ArgumentNullException("translator");
            return translator.GetTranslateValue(fromValue);
        }
    }
}
