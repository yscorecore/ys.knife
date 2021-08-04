using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace YS.Knife
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public abstract class AutoConstructorExtensionFieldAttribute:Attribute
    {
        public AutoConstructorExtensionFieldAttribute(string name, string fieldType, string ctorArgType)
        {
            this.Name = name;
            this.FieldType = fieldType;
            this.CtorArgType = ctorArgType;
        }

        public string Name { get; }

        public string FieldType { get;  }

        public string CtorArgType { get; }

    }

    public class ListFieldAttribute : AutoConstructorExtensionFieldAttribute
    {
        public ListFieldAttribute() : base("children", "System.Collections.IList", "System.Collections.Generic.IList`1")
        {

        }

        //[ModuleInitializer]

        public static void Initializer()
        {
            AutoConstructorGenerator.AddExtensionField(new ListFieldAttribute());
        }
    }


}
