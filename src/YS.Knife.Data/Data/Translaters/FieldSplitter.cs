using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace YS.Knife.Data.Translaters
{
    public class FieldSplitter
    {
        private static Regex path = new Regex(@"^\.[\w+]+$");
        public FieldPath Split(string filedName)
        {
            if (string.IsNullOrWhiteSpace(filedName))
            {
                throw new ArgumentException($"Field name to split should not be blank.");
            }

            bool inFunction = false;
            
           
            return null;
        }
    }

    public class FieldPath
    {
        public string Field { get; set; }

        public string FuncName { get; set; }
        
        public List<FieldPath> SubPaths { get; set; }
    }
}
