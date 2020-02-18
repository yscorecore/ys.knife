using System;
using System.Collections.Generic;
using System.Text;

namespace Knife.Rest.Client
{
    public enum ArgumentSource
    {
        FromQuery,
        FromBody,
        FromHeader,
        FromForm,
        FromRouter
    }
}
