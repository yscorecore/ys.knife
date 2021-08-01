using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceGenerator.Others;
using YS.Knife;

namespace SourceGenerator.Notify
{
    partial class Class1
    {
        [AutoNotify]
        private int value;
        [AutoNotify]
        private Person person;
    }
}
