using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace YS.Knife.Data.UnitTest.Translaters
{
    [TestClass]
    public class FilterTranslatorTest
    {
        [TestMethod]
        public void Test()
        {
        }
        
        public class FromClass
        {
            public int FieldInt { get; set; }
            public string ModelC3Name { get; set; }
            public List<string> FieldList { get; set; }
            public DtoC1 Class1 { get; set; }
            public List<DtoC2> Class2List { get; set; }
        }
        public class ToClass
        {
            public int Fi { get; set; }
            public List<string> Fl { get; set; }
            public ModelC1 C1 { get; set; }
            public List<ModelC2> C2List { get; set; }

        }
        public class DtoC1
        {
            public string Type { get; set; }
        }
        public class ModelC1
        {
            public string Type { get; set; }
        }
        public class DtoC2
        {
            
        }
        public class ModelC2
        {
            
        }
        public class ModelC3
        {
            public string Name { get; set; }
        }
        
    }
}
