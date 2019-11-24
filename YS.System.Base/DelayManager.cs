using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace System
{
    public partial class DelayManager : System.ComponentModel.Component, IDisposable
    {
        Hashtable table = new Hashtable();
        public void Run<A>(Action<A> action, A args, int delay)
        {
            Run(action.Method, action, args, delay);
        }
        public void Run(Action action, int delay)
        {
            Run(action.Method, (a) => { action(); }, 0, delay);
        }
        public void Run<A>(object key, Action<A> fire, A args, int delay)
        {
            if (table.ContainsKey(key))
            {
                var delayevent = table[fire.Method] as DelayEvent<A>;
                delayevent.Reset(args, delay);
            }
            else
            {
                var delayevent = new DelayEvent<A>(this, key, fire, args, delay);
                table.Add(fire.Method, delayevent);
            }
        }

        public void Continue<A>(object key, Action<A> action, A args, int sweep)
        {

        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            foreach (var v in this.table.Values)
            {
                if (v is IDisposable)
                {
                    (v as IDisposable).Dispose();
                }
            }
        }


        #region InnerClass
        class DelayEvent<Aagument> : IDisposable
        {
            SynchronizationContext synchronizationContext;
            System.Timers.Timer timer;
            DelayManager manager;
            object cacheKey;
            public DelayEvent(DelayManager manager, object cacheKey, Action<Aagument> action, Aagument args, int delay)
            {
                this.manager = manager;
                this.cacheKey = cacheKey;
                synchronizationContext = SynchronizationContext.Current;
                timer = new System.Timers.Timer(delay)
                {
                    AutoReset = false
                };
                timer.Elapsed += Timer_Elapsed;
                this.FireAction = action;
                this.Arguments = args;
                timer.Start();
            }

            #region Property
            public Action<Aagument> FireAction { get; private set; }
            public Aagument Arguments { get; private set; }
            #endregion

            #region Method
            public void Reset(Aagument args, int delay)
            {
                this.timer.Stop();
                this.timer.Interval = delay;
                this.Arguments = args;
                this.timer.Start();
            }
            public void Dispose()
            {
                this.timer.Dispose();
            }
            #endregion

            #region private
            private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
                manager.table.Remove(this.cacheKey);
                if (synchronizationContext != null)
                {
                    synchronizationContext.Send((a) =>
                    {
                        this.FireAction((Aagument)a);
                    }, this.Arguments);
                }
                else
                {
                    FireAction(this.Arguments);
                }
            }
            #endregion
        }
        #endregion
    }
}
