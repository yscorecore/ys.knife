using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace System.Test
{
    public static class TestUtility
    {
        /// <summary>
        /// 执行一个方法
        /// </summary>
        /// <param name="type">方法所在的类型</param>
        /// <param name="source">对其调用方法或构造函数的对象，如果方法是静态的，则忽略此参数</param>
        /// <param name="method">方法的名称，方法可以是公共的，私有的，静态的或者实例化的</param>
        /// <param name="args">执行方法的参数</param>
        /// <returns>方法执行的结果，包含方法的执行时间，发生的异常和方法的返回值</returns>
        public static MethodTestResult ExecMethod(Type type, object source, string method, params object[] args)
        {
            MethodInfo me = type.GetMethod(method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            if (me != null)
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                try
                {
                    object res = me.Invoke(source, args);
                    sw.Stop();
                    return new MethodTestResult(res, sw.Elapsed);
                }
                catch (Exception ex)
                {
                    sw.Stop();
                    return new MethodTestResult(ex, sw.Elapsed);
                }
            }
            else
            {
                return new MethodTestResult(new Exception(string.Format("can not find the method '{0}'", method)));
            }
        }
        /// <summary>
        /// 循环执行某一个方法
        /// </summary>
        /// <param name="type"></param>
        /// <param name="source"></param>
        /// <param name="method"></param>
        /// <param name="loopCount">循环的次数</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static MethodTestResult LoopExecMethod(Type type, object source, string method, int loopCount, params object[] args)
        {
            MethodInfo me = type.GetMethod(method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            if (me != null)
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                try
                {
                    object res = null;
                    for (int i = 0; i < loopCount; i++)
                    {
                        res = me.Invoke(source, args);
                    }
                    sw.Stop();
                    return new MethodTestResult(res, sw.Elapsed);
                }
                catch (Exception ex)
                {
                    sw.Stop();
                    return new MethodTestResult(ex, sw.Elapsed);
                }
            }
            else
            {
                return new MethodTestResult(new Exception(string.Format("can not find the method '{0}'", method)));
            }
        }

        /// <summary>
        /// 快速获取对象的简单描述信息
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="silptChars"></param>
        /// <returns></returns>
        public static string GetObjectInfo(this object obj, string silptChars = "=")
        {
            return CommonUtility.CreateObjectInfo(obj, silptChars);
        }
    }
}
