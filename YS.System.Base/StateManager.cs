using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public sealed class StateManager
    {
        static StateManager _default = new StateManager();
        public static StateManager Default
        {
            get
            {
                return _default;
            }
        }
        StateList dic = new StateList();
        public void Reset()
        {
            this.dic.Clear();
        }
        public IDisposable CreateState(string stateName)
        {
            State st = new State(this, NormalName(stateName));
            dic.Add(st);
            return st;
        }
        public bool HasState(string stateName)
        {
            return this.dic.ContainsKey(NormalName(stateName));
        }
        public void Dispose(string flag)
        {
            flag = NormalName(flag);
            if (dic.ContainsKey(flag))
            {
                State s = dic[flag];
                dic.Remove(s);
                s.Dispose();
            }
        }
        private static string NormalName(string name)
        {
            return (name ?? string.Empty).ToLower();
        }
        private class State : IDisposable
        {
            StateManager manager;
            public State(StateManager manager, string name)
            {
                this.manager = manager;
                this.Name = name;
            }
            public string Name { get; set; }
            public void Dispose()
            {
                if (manager != null)
                {
                    manager.Dispose(this.Name);
                }
            }
        }
        private class StateList : KeyedList<State, string>
        {
            public override string GetItemKey(State item)
            {
                if (item != null && item.Name != null)
                {
                    return item.Name;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
