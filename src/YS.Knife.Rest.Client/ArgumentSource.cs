using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Rest.Client
{
    public enum ArgumentSource
    {
        Query,
        BodyJson,
        BodyRaw,
        Header,
        FormUrlEncoded,
        FormData,
        Router
    }
}
