using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;

namespace YS.Knife.Options
{
    [Options]
    public class DeepObjectOptions
    {
        public string Text { get; set; }
        public AddressInfo Level1 { get; set; }
    }

    public class AddressInfo
    {
        public string Street { get; set; }

        public string City{get;set;}

        public Province Province{get;set;}
    }

    public class Province
    {
        public string Country { get; set; }
        public string Code { get; set; }
    }

}
