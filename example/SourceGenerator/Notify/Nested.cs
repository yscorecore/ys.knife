using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YS.Knife;

namespace SourceGenerator.Notify
{
    partial class Nested
    {
        [AutoNotify]
        private int val;

        partial class Class1 : INotifyPropertyChanged
        {
            [AutoNotify]
            private int val;

            public event PropertyChangedEventHandler PropertyChanged;

            partial class Class3 : Class1
            {
                [AutoNotify]
                private int val34;

            }
        }

        partial class Class1
        {
            [AutoNotify(PropertyName = "IntValue")]
            private int value;
        }
    }
}
