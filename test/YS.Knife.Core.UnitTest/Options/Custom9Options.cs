using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;

namespace YS.Knife.Options
{
    [Options]
    public class Custom9Options
    {
        public string Text { get; set; }
    }

    [OptionsPostHandler]
    public class Custom9Validator : IPostConfigureOptions<Custom9Options>
    {
        public void PostConfigure(string name, Custom9Options options)
        {
            options.Text = "__" + options.Text;
        }
    }
}
