using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Rest.Client
{
    public enum ArgumentSource
    {
        Router,
        Query,
        Header,
        BodyJson,
        FormUrlEncoded,
        FormData,

    }
}
