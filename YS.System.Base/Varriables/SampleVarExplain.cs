using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Varriables
{
    public sealed class SampleVarExplain:IVarriableExplain
    {
        private const string MatchPattern = @"\$\[\s*(?<var>\w+)\s*(:(?<fmt>[^]\n]+))?]";
        private const string VarName = "var";
        private const string FmtName = "fmt";

        public event EventHandler<SampleVarExplainErrorEventArgs> ErrorHandle;
        public object ExpandEnvironmentVariables(string variableStr)
        {
            if(string.IsNullOrEmpty(variableStr))
            {
                return string.Empty;
            }
            else
            {
                return Regex.Replace(variableStr,MatchPattern,MatchEvaluator1);
            }
        }

        public object GetVariableValue(string variableName)
        {
            if(this.varriables.ContainsKey(variableName))
            {
                return this.varriables[variableName].GetValue();
            }
            else
            {
                return null;
            }
        }

        private VarriableCollection varriables = new VarriableCollection();

        public VarriableCollection Varriables
        {
            get { return varriables; }
        }

        private string MatchEvaluator1(Match match)
        {
            Group vargroup = match.Groups[VarName];
            Group fmtgroup = match.Groups[FmtName];
            try
            {
                if(this.varriables.ContainsKey(vargroup.Value))
                {
                    object obj = this.GetVariableValue(vargroup.Value);
                    if(obj != null)
                    {
                        try
                        {
                            string fmt = string.Format("{{0:{0}}}",Text.StandardTextExplain.DecodeEscape(fmtgroup.Value));
                            return string.Format(fmt,obj);
                        }
                        catch(FormatException ex)
                        {
                            var v = new SampleVarExplainErrorEventArgs(match,vargroup.Value,fmtgroup.Value,ExplainErrorKind.InvalidFormated,ex);
                            this.HandleErrorInternal(v);
                            return v.MatchResult;
                        }
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    var v = new SampleVarExplainErrorEventArgs(match,vargroup.Value,fmtgroup.Value,ExplainErrorKind.VarriableNotFound,null);
                    this.HandleErrorInternal(v);
                    return v.MatchResult;
                }
            }
            catch(Exception ex)
            {
                SampleVarExplainErrorEventArgs arg = new SampleVarExplainErrorEventArgs(match,vargroup.Value,fmtgroup.Value,ExplainErrorKind.Other,ex);
                this.HandleErrorInternal(arg);
                return arg.MatchResult;
            }

        }
        private void HandleErrorInternal(SampleVarExplainErrorEventArgs a)
        {
            if(ErrorHandle != null)
            {
                ErrorHandle(this,a);
            }
        }
    }

    [Serializable]
    public class SampleVarExplainErrorEventArgs:EventArgs
    {
        public string Format { get; private set; }

        public string VarName { get; private set; }

        public ExplainErrorKind ErrorKind { get; private set; }
        public Exception Exception { get; private set; }
        public Match Match { get; private set; }
        public string MatchValue
        {
            get
            {
                if(this.Match != null)
                {
                    return this.Match.Value;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public SampleVarExplainErrorEventArgs(Match match,string varName,string format,ExplainErrorKind kind,Exception exception)
        {
            this.Match = match;
            this.VarName = varName;
            this.Format = format;
            this.ErrorKind = kind;
            this.MatchResult = match.Value;

        }
        public virtual string MatchResult { get; set; }
    }

}
