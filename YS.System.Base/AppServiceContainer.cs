using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public class AppServiceContainer : System.ComponentModel.Design.ServiceContainer
    {
        private static AppServiceContainer instance;

        private static object instancelock = new object();

        private AppServiceContainer()
        {
        }

        public static AppServiceContainer Instance
        {
            get
            {
                if (object.ReferenceEquals(instance, null))
                {
                    lock (instancelock)
                    {
                        if (object.ReferenceEquals(instance, null))
                        {
                            instance = new AppServiceContainer();
                        }
                    }
                }
                return instance;
            }
        }
        public static T GetService<T>()
        {
            return (T)Instance.GetService(typeof(T));
        }
        public static void AddService<T>(object value)
        {
            Instance.AddService(typeof(T), value);
        }
    }

}
