using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Varriables
{
    public sealed class SuperVarExplain : IVarriableExplain
    {
        /*********************************************
         * $[datetime]
         * $[datetime:yyyy年MM月]
         * $[(datetime).Year:d5]
         * $[(datetime).Year.ToString().Length:d5]
         * $[<System.DateTime>.Now.Month]
         * $[<System.DateTime,mscorlib>.Now.Month:d4]
         * *******************************************/
        //\$\[\s*<(?<type>.+)>(?<path>(\.\w+(\(\s*\))?)+)\s*(:(?<fmt>[^]\n]*))?]
        //\$\[\s*(?<var>\w+)\s*(:(?<fmt>[^]\n]*))?]
        //\$\[\s*\((?<var>\w+)\)(?<path>(\.\w+(\(\s*\))?)+)\s*(:(?<fmt>[^]\n]*))?]
        private const string MatchPattern = @"\$\[\s*(?<var>\w+)\s*(:(?<fmt>[^]\n]*))?]|\$\[\s*\((?<var>\w+)\)(?<path>(\.\w+(\(\s*\))?)+)\s*(:(?<fmt>[^]\n]*))?]|\$\[\s*<(?<type>.+)>(?<path>(\.\w+(\(\s*\))?)*)\s*(:(?<fmt>[^]\n]*))?]";
        private const string VarName = "var";
        private const string FmtName = "fmt";
        private const string TypeName = "type";
        private const string PathName = "path";
        private const string SingleMatchPattern = @"^\s*" + MatchPattern + @"\s*$";

        private static Regex MatchRegex = new Regex(MatchPattern);
        private static Regex SingleMatchRegex = new Regex(SingleMatchPattern);
        #region 事件
        public event EventHandler<SuperVarExplainErrorEventArgs> ErrorHandle;
        #endregion

        #region 属性
        private VarriableCollection varriables = new VarriableCollection();

        public VarriableCollection Varriables
        {
            get { return varriables; }
        }
        private ITypeService ty = SmartTypeService.Instance;
        public ITypeService TypeService
        {
            get { return ty; }
            set { ty = value; }
        }
        #endregion

        #region 方法
        public object ExpandEnvironmentVariables(string variableStr)
        {
            if (string.IsNullOrEmpty(variableStr))
            {
                return null;
            }
            else
            {
                var match = SingleMatchRegex.Match(variableStr);
                if (match != null && match.Success)
                {
                    throw new NotImplementedException();
                }
                else//不是单个的则为字符串
                {
                    return SuperVarExplain.MatchRegex.Replace(variableStr, this.MatchEvaluator1);
                }
            }
        }
        public object GetVariableValue(string variableName)
        {
            if (this.varriables.ContainsKey(variableName))
            {
                return this.varriables[variableName].GetValue();
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 私有方法
        private string GetVarValue(Match match, string variableName, string path, string format)
        {
            if (this.varriables.ContainsKey(variableName))
            {
                object obj = this.GetVariableValue(variableName);
                if (obj == null)
                {
                    return string.Empty;
                }
                try//获取对象
                {
                    obj = System.Reflection.PathDirection.GetObject(obj, path);
                }
                catch (Exception ex)
                {
                    var v = new SuperVarExplainErrorEventArgs(match, string.Empty, variableName, path, format, ExplainErrorKind.InvalidPathDirection, ex);
                    this.HandleErrorInternal(v);
                    return v.MatchResult;
                }
                //格式化
                try
                {
                    string fmt = string.Format("{{0:{0}}}", Text.StandardTextExplain.DecodeEscape(format));
                    return string.Format(fmt, obj);
                }
                catch (FormatException ex)
                {
                    var v = new SuperVarExplainErrorEventArgs(match, string.Empty, variableName, path, format, ExplainErrorKind.InvalidFormated, ex);
                    this.HandleErrorInternal(v);
                    return v.MatchResult;
                }
            }
            else
            {
                var v = new SuperVarExplainErrorEventArgs(match, string.Empty, variableName, path, format, ExplainErrorKind.VarriableNotFound, null);
                this.HandleErrorInternal(v);
                return v.MatchResult;
            }

        }
        private string GetTypeValue(Match match, string type, string path, string format)
        {
            Type ty = this.TypeService != null ? this.TypeService.GetTypeByName(type) : Type.GetType(type);
            if (ty != null)
            {
                object obj = null;
                if (string.IsNullOrEmpty(path) || path.Trim() == ".")//缺少路径
                {
                    var v = new SuperVarExplainErrorEventArgs(match, type, string.Empty, path, format, ExplainErrorKind.MissPath, null);
                    this.HandleErrorInternal(v);
                    return v.MatchResult;
                }

                try//获取对象
                {
                    obj = System.Reflection.PathDirection.GetObject(ty, path);
                }
                catch (Exception ex)
                {
                    var v = new SuperVarExplainErrorEventArgs(match, type, string.Empty, path, format, ExplainErrorKind.InvalidPathDirection, ex);
                    this.HandleErrorInternal(v);
                    return v.MatchResult;
                }
                //格式化
                try
                {
                    string fmt = string.Format("{{0:{0}}}", Text.StandardTextExplain.DecodeEscape(format));
                    return string.Format(fmt, obj);
                }
                catch (FormatException ex)
                {
                    var v = new SuperVarExplainErrorEventArgs(match, type, string.Empty, path, format, ExplainErrorKind.InvalidFormated, ex);
                    this.HandleErrorInternal(v);
                    return v.MatchResult;
                }
            }
            else
            {
                var v = new SuperVarExplainErrorEventArgs(match, type, string.Empty, path, format, ExplainErrorKind.UnKnowType, null);
                this.HandleErrorInternal(v);
                return v.MatchResult;
            }
        }
        private object GetTypeValue2(Match match, string type, string path, string format)
        {
            Type ty = this.TypeService != null ? this.TypeService.GetTypeByName(type) : Type.GetType(type);
            if (ty != null)
            {
                object obj = null;
                if (string.IsNullOrEmpty(path) || path.Trim() == ".")//缺少路径
                {
                    var v = new SuperVarExplainErrorEventArgs(match, type, string.Empty, path, format, ExplainErrorKind.MissPath, null);
                    this.HandleErrorInternal(v);
                    return v.MatchResult;
                }

                try//获取对象
                {
                    obj = System.Reflection.PathDirection.GetObject(ty, path);
                }
                catch (Exception ex)
                {
                    var v = new SuperVarExplainErrorEventArgs(match, type, string.Empty, path, format, ExplainErrorKind.InvalidPathDirection, ex);
                    this.HandleErrorInternal(v);
                    return v.MatchResult;
                }
                //格式化
                try
                {
                    string fmt = string.Format("{{0:{0}}}", Text.StandardTextExplain.DecodeEscape(format));
                    return string.Format(fmt, obj);
                }
                catch (FormatException ex)
                {
                    var v = new SuperVarExplainErrorEventArgs(match, type, string.Empty, path, format, ExplainErrorKind.InvalidFormated, ex);
                    this.HandleErrorInternal(v);
                    return v.MatchResult;
                }
            }
            else
            {
                var v = new SuperVarExplainErrorEventArgs(match, type, string.Empty, path, format, ExplainErrorKind.UnKnowType, null);
                this.HandleErrorInternal(v);
                return v.MatchResult;
            }
        }
        private string MatchEvaluator1(Match match)
        {
            Group vargroup = match.Groups[VarName];
            Group fmtgroup = match.Groups[FmtName];
            Group typegroup = match.Groups[TypeName];
            Group pathgroup = match.Groups[PathName];
            try
            {
                if (typegroup.Success)//静态
                {
                    return this.GetTypeValue(match, typegroup.Value,
                        pathgroup.Success ? pathgroup.Value : string.Empty,
                        fmtgroup.Success ? fmtgroup.Value : string.Empty);
                }
                else
                {
                    return this.GetVarValue(match, vargroup.Value,
                         pathgroup.Success ? pathgroup.Value : string.Empty,
                        fmtgroup.Success ? fmtgroup.Value : string.Empty);
                }
            }
            catch (Exception ex)
            {
                var v = new SuperVarExplainErrorEventArgs(match, typegroup.Value, vargroup.Value, pathgroup.Value, fmtgroup.Value, ExplainErrorKind.Other, ex);
                this.HandleErrorInternal(v);
                return v.MatchResult;
            }
        }
        private void HandleErrorInternal(SuperVarExplainErrorEventArgs a)
        {
            if (ErrorHandle != null)
            {
                ErrorHandle(this, a);
            }
        }
        private object MatchToObject(Match match)
        {
            Group vargroup = match.Groups[VarName];
            Group fmtgroup = match.Groups[FmtName];
            Group typegroup = match.Groups[TypeName];
            Group pathgroup = match.Groups[PathName];
            try
            {
                if (typegroup.Success)//静态
                {
                    return this.GetTypeValue2(match, typegroup.Value,
                        pathgroup.Success ? pathgroup.Value : string.Empty,
                        fmtgroup.Success ? fmtgroup.Value : string.Empty);
                }
                else
                {
                    return this.GetVarValue(match, vargroup.Value,
                         pathgroup.Success ? pathgroup.Value : string.Empty,
                        fmtgroup.Success ? fmtgroup.Value : string.Empty);
                }
            }
            catch (Exception ex)
            {
                var v = new SuperVarExplainErrorEventArgs(match, typegroup.Value, vargroup.Value, pathgroup.Value, fmtgroup.Value, ExplainErrorKind.Other, ex);
                this.HandleErrorInternal(v);
                return v.MatchResult;
            }
        }
        #endregion

    }
    [Serializable]
    public class SuperVarExplainErrorEventArgs : SampleVarExplainErrorEventArgs
    {
        public string Path { get; private set; }
        public string TypeName { get; private set; }

        public SuperVarExplainErrorEventArgs(Match match, string typeName, string varName, string path, string format, ExplainErrorKind kind, Exception exception)
            : base(match, varName, format, kind, exception)
        {
            this.TypeName = typeName;
            this.Path = path;
        }
    }




}
