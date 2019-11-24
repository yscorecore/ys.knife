using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace System.Authority
{
    public interface IFunctionHandler
    {
        bool Handler(FunctionInfo functionInfo);
    }
    public sealed class CombinFunctionHandler :List<IFunctionHandler>,IFunctionHandler
    {
        public bool Handler(FunctionInfo functionInfo)
        {
            foreach (var handler in this)
            {
                if (handler != null && handler.Handler(functionInfo)) return true;
            }
            return false;
        }
    }
    public abstract class RegexFunctionHandler : IFunctionHandler
    {
        protected abstract Regex Regex { get; }
        public virtual bool Handler(FunctionInfo functionInfo)
        {
            var reg = this.Regex;
            if (reg != null)
            {
                var match = reg.Match(functionInfo.Code);
                if (match.Success)
                {
                    return this.OnHandler(functionInfo, match);
                }
            }
            return false;
        }
        protected abstract bool OnHandler(FunctionInfo function, Match match);
    }
    public abstract class SampleTypeFunctionHandler : RegexFunctionHandler
    {
        protected sealed override Regex Regex
        {
            get
            {
                return new Regex(@"^(?<type>\w+(\.\w+)+\s*,\s*\w+(\.\w+)*)$", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
            }
        }
        protected sealed override bool OnHandler(FunctionInfo function, Match match)
        {
            var type = match.Groups["type"].Value;
            var ty = Type.GetType(type, true);
           return this.OnHandler(function, ty);
        }
        protected abstract bool OnHandler(FunctionInfo function, Type type);

    }
    public abstract class NestedTypeFunctionHandler : RegexFunctionHandler
    {
        protected sealed override Regex Regex
        {
            get
            {
                return new Regex(@"^(?<type>\w+(\.\w+)+\s*\+\s*\w+\s*,\s*\w+(\.\w+)*)$", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
            }
        }

        protected sealed override bool OnHandler(FunctionInfo function, Match match)
        {
            var type = match.Groups["type"].Value;
            var ty = Type.GetType(type, true);
            return this.OnHandler(function, ty,ty.DeclaringType);
        }

        protected abstract bool OnHandler(FunctionInfo function, Type type,Type declareType);
    }
    public abstract class MothodFunctionHandler : RegexFunctionHandler
    {
        protected override Regex Regex
        {
            get
            {
                return new Regex(@"^(?<type>\w+(\.\w+)+\s*,\s*\w+(\.\w+)*)\s*/(?<method>\w+)(\((?<arg>.*?)\))?$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }
        }
        protected sealed override bool OnHandler(FunctionInfo function, Match match)
        {
            var type = match.Groups["type"].Value;
            var method = match.Groups["method"].Value;
            var argString = match.Groups["arg"].Success ? match.Groups["arg"].Value : string.Empty;
            var typeInfo = Type.GetType(type, true);

            var methodInfo = typeInfo.GetMethod(method, BindingFlags.Static|BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (methodInfo == null)
            {
                throw new ApplicationException(string.Format("在类型{0}中找不到名称为{1}的实例方法或静态方法。", type, method));
            }
            var args = GetParameterArgs(methodInfo, methodInfo.GetParameters(), argString);
            return HandlerMethod(typeInfo, methodInfo, args);
        }

        protected abstract bool HandlerMethod(Type type, MethodInfo method, object[] args);
       
        private object[] GetParameterArgs(MethodInfo method, ParameterInfo[] paramInfos, string argString)
        {
            if (paramInfos.Length == 0)
            {
                if (string.IsNullOrWhiteSpace(argString))
                {
                    return null;
                }
                else
                {
                    throw new ApplicationException(string.Format("无效的参数:{0}", argString));
                }
            }
            else if (paramInfos.Length == 1)
            {
                var converter = System.ComponentModel.TypeDescriptor.GetConverter(paramInfos[0].ParameterType);
                var params1 = converter.ConvertFromString(argString);
                return new object[] { params1 };
            }
            else
            {
                var matchs = new Regex(@"(?<arg>.+?)((?<!\\),|$)", RegexOptions.IgnoreCase | RegexOptions.Multiline).Matches(argString);
                if (matchs.Count != paramInfos.Length)
                {
                    throw new ApplicationException(string.Format("参数的数目不匹配,方法名称:{0},参数:{1}", method.Name, argString));
                }
                List<object> res = new List<object>();
                for (int i = 0; i < paramInfos.Length; i++)
                {
                    var converter = System.ComponentModel.TypeDescriptor.GetConverter(paramInfos[i].ParameterType);
                    res.Add(converter.ConvertFromString(matchs[1].Groups["arg"].Value));
                }
                return res.ToArray();
            }
        }
    }


    public class StaticMethodFunctionHandler : MothodFunctionHandler
    {
        protected override bool HandlerMethod(Type type, MethodInfo method, object[] args)
        {
            if (method.IsStatic)
            {
                method.Invoke(null, args);
                return true;
            }
            return false;
        }
    }
    public class InstanceMethodFunctionHandler: MothodFunctionHandler
    {
        public object Source { get; set; }
        public InstanceMethodFunctionHandler(object source)
        {
            this.Source = source;
        }

        protected override bool HandlerMethod(Type type, MethodInfo method, object[] args)
        {
            if (!method.IsStatic)
            {
                
                method.Invoke(this.Source, args);
                return true;
            }
            return false;
        }
    }
}
