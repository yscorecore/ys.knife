﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceGenerator.ConvertTo
{
    public class From
    {
        public string StrProp { get; set; }
        public int IntProp { get; set; }

        public int? NullableIntProp { get; set; }
        public int NullableIntProp2 { get; set; }
        public OneEnum OneEnumProp { get; set; }
        public OneEnum? NullableOneEnumProp { get; set; }
        public OneEnum NullableOneEnumProp2 { get; set; }

        public OneStruct OneStructProp { get; set; }

        public OneStruct? NullableOneStructProp { get; set; }

        public OneStruct NullableOneStructProp2 { get; set; }


        public string[] ArrayStringToArray { get; set; }
        public string[] ArrayStringToList { get; set; }

        public string[] ArrayStringToIList { get; set; }
        public string[] ArrayStringToICollection { get; set; }
        public string[] ArrayStringToIEnumerable { get; set; }
        public string[] ArrayStringToIQueryable { get; set; }
    }

    public enum OneEnum
    {
        Ont,
    }
    public struct OneStruct
    {
        public string StrProp { get; set; }
    }
    public struct OtherStruct
    {
        public string StrProp { get; set; }
        public int IntProp { get; set; }
    }


    public class To
    {
        public string StrProp { get; set; }
        public int IntProp { get; set; }
        public int? NullableIntProp { get; set; }
        public int? NullableIntProp2 { get; set; }
        public OneEnum OneEnumProp { get; set; }
        public OneEnum? NullableOneEnumProp { get; set; }
        public OneEnum? NullableOneEnumProp2 { get; set; }

        public OneStruct OneStructProp { get; set; }

        public OneStruct? NullableOneStructProp { get; set; }

        public OneStruct? NullableOneStructProp2 { get; set; }

        public string[] ArrayStringToArray { get; set; }

        public List<string> ArrayStringToList { get; set; }
        public IList<string> ArrayStringToIList { get; set; }

        public ICollection<string> ArrayStringToICollection { get; set; }

        public IEnumerable<string> ArrayStringToIEnumerable { get; set; }

        public IQueryable<string> ArrayStringToIQueryable { get; set; }
    }

    [YS.Knife.ConvertTo(typeof(From),typeof(To))]
    public partial class Converts
    {
       
    }
}
