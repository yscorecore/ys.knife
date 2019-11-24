using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public class TokenManager
    {
        private static TokenManager instance;

        private static object instancelock = new object();

        private TokenManager()
        {
        }

        public static TokenManager Instance
        {
            get
            {
                if (object.ReferenceEquals(instance, null))
                {
                    lock (instancelock)
                    {
                        if (object.ReferenceEquals(instance, null))
                        {
                            instance = new TokenManager();
                        }
                    }
                }
                return instance;
            }
        }
        Dictionary<string, object> tokens = new Dictionary<string, object>();
        public object GetToken(string key)
        {
            lock (tokens)
            {
                if (tokens.ContainsKey(key))
                {
                    return null;
                }
                else
                {
                    return tokens[key] = new object();
                }
            }
        }
        public void BackToken(string key)
        {
            lock (tokens)
            {
                tokens.Remove(key);
            }
        }
    }
}
