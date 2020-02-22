using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Rest.Api.Client.Generator
{
    public enum ArgumentSource
    {
        Unknown,
        FromQuery,
        FromBody,
        FromHeader,
        FromForm,
        FromRouter,
        FromService,
    }
}
