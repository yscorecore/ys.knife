using System;

namespace YS.Knife.Data.Query
{
    [AttributeUsage(AttributeTargets.Field,AllowMultiple =true)]
    internal class OperatorCodeAttribute:Attribute
    {
        public OperatorCodeAttribute(string code)
        {
            this.Code = code;
        }
        public string Code { get; set; }
    }
}
