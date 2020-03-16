using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Rest.Client
{
    public class RestArgument
    {
        public RestArgument(string name, ArgumentSource source, object value)
        {
            this.Name = name;
            this.Source = source;
            this.Value = value;
        }
        public string Name { get; private set; }

        public ArgumentSource Source { get; private set; }

        public object Value { get; private set; }

    }
}
