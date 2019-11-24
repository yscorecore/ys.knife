using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Permissions;
using System.ComponentModel;

namespace System.License
{
    //[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
    public class FileLicenseProvider : LicFileLicenseProvider
    {
        protected override string GetKey (Type type) {
            return base.GetKey(type);
        }
        protected override bool IsKeyValid (string key, Type type) {
            return base.IsKeyValid(key, type);
        }
        
    }
}
