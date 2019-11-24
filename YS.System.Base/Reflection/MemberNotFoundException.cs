using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Reflection
{
    [Serializable]
    public class MemberNotFoundException:Exception
    {
        public MemberNotFoundException() { }
        public MemberNotFoundException(Type type,string memberName) : base(CreateMessage(memberName,type)) { }
        protected MemberNotFoundException(
          Runtime.Serialization.SerializationInfo info,
          Runtime.Serialization.StreamingContext context)
            : base(info,context) { }

        private static string CreateMessage(string membername,Type type)
        {
            return string.Format("Can not the member '{0}' in type {1}",membername, type);
        }
    }
}
