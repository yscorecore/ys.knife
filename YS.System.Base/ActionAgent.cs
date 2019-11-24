using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public class ActionAgent
    {
        const string keyformat = "yyyyMMdd_HHmmss";
        static ActionAgent()
        {
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            string key = e.SignalTime.ToString(keyformat);
            if (actions.ContainsKey(key))
            {
                lock (actions)
                {
                    var lst = actions[key];
                    actions.Remove(key);
                    foreach (var v in lst)
                    {
                        v.Action.BeginInvoke(v.CallBack, null);
                    }
                }
            }
        }
        private static System.Timers.Timer timer = new System.Timers.Timer() { Interval = 1000 };
        private static Dictionary<string, List<ActionBody>> actions = new Dictionary<string, List<ActionBody>>();
        /// <summary>
        /// 在指定的时间运行一次动作(精确到秒)
        /// </summary>
        /// <param name="dateTime">指定的时间</param>
        /// <param name="action">要执行的动作</param>
        /// <param name="callBack">回调函数</param>
        public static void RunActionAt(DateTime dateTime, Action action, AsyncCallback callBack)
        {
            if (action == null) throw new ArgumentNullException("action");
            string key = dateTime.ToString(keyformat);
            lock (actions)
            {
                if (actions.ContainsKey(key))
                {
                    actions[key].Add(new ActionBody(action, callBack));
                }
                else
                {
                    actions.Add(key, new List<ActionBody> { new ActionBody(action, callBack) });
                }
            }
        }
        /// <summary>
        /// 在指定的时间运行一次动作(精确到秒)
        /// </summary>
        /// <param name="dateTime">指定的时间</param>
        /// <param name="action">要执行的动作</param>
        public static void RunActionAt(DateTime dateTime, Action action)
        {
            RunActionAt(dateTime, action, null);
        }
        /// <summary>
        /// 清除无效的任务。（已经过时，但没有执行）
        /// </summary>
        public static void ClearInvalid()
        {
            string key = DateTime.Now.ToString(keyformat);
            List<string> lst=new List<string> ();
            lock (actions)
            {
                foreach (string v in actions.Keys)
                {
                    if (v.CompareTo(key)<0)
                    {
                        lst.Add(v);
                    }
                }
                foreach (string k in lst)
                {
                    actions.Remove(k);
                }
            }
        }
        /// <summary>
        /// 在指定的秒数之后执行一次操作
        /// </summary>
        /// <param name="action">要执行的动作</param>
        public static void RunActionAfterSeconds(int seconds, Action action, AsyncCallback callBack)
        {
            if (seconds < 0) throw new ArgumentOutOfRangeException("seconds", "seconds must be a positive integer.");
            RunActionAt(DateTime.Now.AddSeconds(seconds), action, callBack);
        }
        /// <summary>
        /// 在指定的秒数之后执行一次操作
        /// </summary>
        /// <param name="action">要执行的动作</param>
        public static void RunActionAfterSeconds(int seconds, Action action)
        {
            RunActionAfterSeconds(seconds, action, null);
        }
        public static bool RemoveAction(Guid actionId)
        {
            foreach (var v in actions.Values)
            {
                ActionBody body = null;
                foreach (var item in v)
                {
                    if (item.ID == actionId)
                    {
                        body = item;
                        break;
                    }
                }
                if (body != null)
                {
                    return v.Remove(body);
                }
            }
            return false;
        }
        class ActionBody
        {
            public ActionBody(Action action)
            {
                this.Action = action;
                this.ID = Guid.NewGuid();
            }
            public ActionBody(Action action, AsyncCallback callback)
            {
                this.Action = action;
                this.CallBack = callback;
                this.ID = Guid.NewGuid();
            }
            public Action Action;
            public AsyncCallback CallBack;
            public Guid ID;
        }

    }
}
