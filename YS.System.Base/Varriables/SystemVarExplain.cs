using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Varriables
{
    public class SystemVarExplain:IVarriableExplain
    {
        public object GetValue(string variableName)
        {
            return Environment.GetEnvironmentVariable(variableName,EnvironmentVariableTarget.Process);
        }
        public object ExpandEnvironmentVariables(string variableStr)
        {
            return Environment.ExpandEnvironmentVariables(variableStr);
        }
    }
}
