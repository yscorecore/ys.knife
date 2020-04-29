using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Api.Client.Generator.Model
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
