using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Varriables
{
    public static class CommonVar
    {
        public const string TimeName="Time";
        public const string ThreadName="Thread";
        public const string MachineName = "Machine";
        public const string UserName = "User";
        public const string StackTraceName="Stack";
        public const string ProcessName = "Process";
        public const string ProcessIDName = "ProcessId";
        public const string NewLineName = "NewLine";
        public const string RunTimeName = "RunTime";
        public const string AppDomainName = "AppDomain";
        //public const string MethodName = "Method";

        public readonly static Varriable Time = new DynamicVar(TimeName,() => { return DateTime.Now; });
        public readonly static Varriable Thread = new DynamicVar(ThreadName,() => { return Threading.Thread.CurrentThread.ManagedThreadId; });
        public readonly static Varriable Machine = new EnvironmentVar(MachineName,"MachineName");
        public readonly static Varriable User = new EnvironmentVar(UserName,"UserName");
        public readonly static Varriable StackTrace = new EnvironmentVar(StackTraceName,"StackTrace");
        public readonly static Varriable Process = new DynamicVar(ProcessName,() => { return  System.Diagnostics.Process.GetCurrentProcess().ProcessName; });
        //public readonly static Varriable ProcessId = new DynamicVar(ProcessName,() => { return System.Diagnostics.Process.GetCurrentProcess().Id; });
        public readonly static Varriable NewLine = new EnvironmentVar(NewLineName,"NewLine");
        public readonly static Varriable RunTime = new DynamicVar(RunTimeName,() => { return DateTime.Now - System.Diagnostics.Process.GetCurrentProcess().StartTime; });
        public readonly static Varriable AppDomain = new DynamicVar(AppDomainName, () => { return System.AppDomain.CurrentDomain.FriendlyName; });
        //public readonly static Varriable Method = new DynamicVar(MethodName,() => { return string.Empty; });
    }
}
