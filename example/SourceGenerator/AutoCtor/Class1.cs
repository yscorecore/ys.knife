using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YS.Knife;

namespace SourceGenerator.AutoCtor
{
    [AutoConstructor]
    partial class Class1<T>
    {
        private T genericVal;
        private string strValue;
        public int IntProp { get; private set; }

        public int AbcDE { get; }

        public int MyProperty { get; set; }
        public int Value
        {
            get { return 1; }
        }

        public static void Test()
        {

        }


    }

    [AutoConstructor]
    partial class Class2
    {

        private int myVar;
        private IList data;
        private IList<Class2> logger;

        public int MyProperty
        {
            get { return 1; }
        }

    }
    [AutoConstructor]
    partial class C2
    {
        private string abc;
        private int bcd;
    }
}
