using System;
using System.Net;
using System.Threading;

namespace System.Threading
{
    /// <summary>
    /// 提供将异步转为同步执行的方法
    /// </summary>
    public static class AsyncToSync
    {
        public static VResult GetResult<TArg, VResult>(TArg arg, Action<FunContext<TArg, VResult>> action)
        {
            FunContext<TArg, VResult> context = new FunContext<TArg, VResult>(arg);
            action(context);
            context.Wait();
            return context.Result;
        }
        public static VResult GetResult<TArg1, TArg2, VResult>(TArg1 arg1, TArg2 arg2, Action<FunContext<TArg1, TArg2, VResult>> action)
        {
            FunContext<TArg1, TArg2, VResult> context = new FunContext<TArg1, TArg2, VResult>(arg1, arg2);
            action(context);
            context.Wait();
            return context.Result;
        }
        public static VResult GetResult<TArg1, TArg2, TArg3, VResult>(TArg1 arg1, TArg2 arg2, TArg3 arg3, Action<FunContext<TArg1, TArg2, TArg3, VResult>> action)
        {
            FunContext<TArg1, TArg2, TArg3, VResult> context = new FunContext<TArg1, TArg2, TArg3, VResult>(arg1, arg2, arg3);
            action(context);
            context.Wait();
            return context.Result;
        }
        public static VResult GetResult<TArg1, TArg2, TArg3, TArg4, VResult>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, Action<FunContext<TArg1, TArg2, TArg3, TArg4, VResult>> action)
        {
            FunContext<TArg1, TArg2, TArg3, TArg4, VResult> context = new FunContext<TArg1, TArg2, TArg3, TArg4, VResult>(arg1, arg2, arg3, arg4);
            action(context);
            context.Wait();
            return context.Result;
        }
        public static void RunAction(Action<ActionContext> action)
        {
            ActionContext context = new ActionContext();
            action(context);
            context.Wait();
        }
        public static void RunAction<TArg>(TArg arg, Action<ActionContext<TArg>> action)
        {
            ActionContext<TArg> context = new ActionContext<TArg>(arg);
            action(context);
            context.Wait();
        }
        public static void RunAction<TArg1, TArg2>(TArg1 arg1, TArg2 arg2, Action<ActionContext<TArg1, TArg2>> action)
        {
            ActionContext<TArg1, TArg2> context = new ActionContext<TArg1, TArg2>(arg1, arg2);
            action(context);
            context.Wait();
        }
        public static void RunAction<TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3, Action<ActionContext<TArg1, TArg2, TArg3>> action)
        {
            ActionContext<TArg1, TArg2, TArg3> context = new ActionContext<TArg1, TArg2, TArg3>(arg1, arg2, arg3);
            action(context);
            context.Wait();
        }
        public static void RunAction<TArg1, TArg2, TArg3, TArg4>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, Action<ActionContext<TArg1, TArg2, TArg3, TArg4>> action)
        {
            ActionContext<TArg1, TArg2, TArg3, TArg4> context = new ActionContext<TArg1, TArg2, TArg3, TArg4>(arg1, arg2, arg3, arg4);
            action(context);
            context.Wait();
        }
    }

    public class ActionContext
    {
        ManualResetEvent manualReset = new ManualResetEvent(false);
        public void Go()
        {
            this.manualReset.Set();
        }
        public void Wait()
        {
            this.manualReset.WaitOne();
        }
        public void Wait(int millisecondsTimeout)
        {
            this.manualReset.WaitOne(millisecondsTimeout);
        }
    }

    public class ActionContext<TArg> : ActionContext
    {
        public ActionContext(TArg arg)
        {
            this.Argument = arg;
        }
        public TArg Argument { get; private set; }
    }

    public class ActionContext<TArg1, TArg2> : ActionContext
    {
        public ActionContext(TArg1 arg1, TArg2 arg2)
        {
            this.Argument1 = arg1;
            this.Argument2 = arg2;
        }
        public TArg1 Argument1 { get; private set; }
        public TArg2 Argument2 { get; private set; }
    }

    public class ActionContext<TArg1, TArg2, TArg3> : ActionContext
    {
        public ActionContext(TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            this.Argument1 = arg1;
            this.Argument2 = arg2;
            this.Argument3 = arg3;
        }
        public TArg1 Argument1 { get; private set; }
        public TArg2 Argument2 { get; private set; }
        public TArg3 Argument3 { get; private set; }
    }

    public class ActionContext<TArg1, TArg2, TArg3, TArg4> : ActionContext
    {
        public ActionContext(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            this.Argument1 = arg1;
            this.Argument2 = arg2;
            this.Argument3 = arg3;
            this.Argument4 = arg4;
        }
        public TArg1 Argument1 { get; private set; }
        public TArg2 Argument2 { get; private set; }
        public TArg3 Argument3 { get; private set; }
        public TArg4 Argument4 { get; private set; }
    }

    public class FunContext<TResult> : ActionContext
    {
        public TResult Result { get; set; }
    }

    public class FunContext<TArg, TResult> : FunContext<TResult>
    {
        public FunContext(TArg arg)
        {
            this.Argument = arg;
        }
        public TArg Argument { get; private set; }
    }

    public class FunContext<TArg1, TArg2, TResult> : FunContext<TResult>
    {
        public FunContext(TArg1 arg1, TArg2 arg2)
        {
            this.Argument1 = arg1;
            this.Argument2 = arg2;
        }
        public TArg1 Argument1 { get; private set; }
        public TArg2 Argument2 { get; private set; }
    }

    public class FunContext<TArg1, TArg2, TArg3, TResult> : FunContext<TResult>
    {
        public FunContext(TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            this.Argument1 = arg1;
            this.Argument2 = arg2;
            this.Argument3 = arg3;
        }
        public TArg1 Argument1 { get; private set; }
        public TArg2 Argument2 { get; private set; }
        public TArg3 Argument3 { get; private set; }
    }

    public class FunContext<TArg1, TArg2, TArg3, TArg4, TResult> : FunContext<TResult>
    {
        public FunContext(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            this.Argument1 = arg1;
            this.Argument2 = arg2;
            this.Argument3 = arg3;
            this.Argument4 = arg4;
        }
        public TArg1 Argument1 { get; private set; }
        public TArg2 Argument2 { get; private set; }
        public TArg3 Argument3 { get; private set; }
        public TArg4 Argument4 { get; private set; }
    }
}
