using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Permissions;
using System.ComponentModel;

namespace System.ComponentModel
{
    //[HostProtection(SecurityAction.LinkDemand,SharedState = true)]
    public class DataTimeLimitLicenseProvider:LicenseProvider
    {
        public override System.ComponentModel.License GetLicense(LicenseContext context,Type type,object instance,bool allowExceptions)
        {
            if(context.UsageMode == LicenseUsageMode.Runtime)
            {
                string con = context.GetSavedLicenseKey(type,null);
                if(DateTime.Now < EndDateTime && DateTime.Now > BeginDateTime)
                {
                    System.ComponentModel.License license = new DateTimeLimitLicense();
                    context.SetSavedLicenseKey(type,license.LicenseKey);
                    return license;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return new DateTimeLimitLicense();
            }
        }

        public virtual DateTime EndDateTime
        {
            get
            {
                return new DateTime(2010,2,20);
            }
        }
        public virtual DateTime BeginDateTime
        {
            get
            {
                return new DateTime(2010,10,12);
            }
        }
        private class DateTimeLimitLicense:System.ComponentModel.License
        {
            public override void Dispose()
            {
            }
            public override string LicenseKey
            {
                get { return "Licensed"; }
            }
        }
    }
}
