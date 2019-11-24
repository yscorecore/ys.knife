using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data
{
    public class EntityValidateResult : List<EntityValidateError>
    {
        public bool Success
        {
            get
            {
                return this.Count == 0;
            }
        }
    }
    public class EntityValidateError
    {
        public object Entity { get; set; }
        public string Error { get; set; }
    }
}
