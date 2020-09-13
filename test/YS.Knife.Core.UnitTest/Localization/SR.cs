using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Localization;

namespace YS.Knife.Localization
{

    [Resource]
    public class SR
    {
        private readonly IStringLocalizer<SR> stringLocalizer;

        public SR(IStringLocalizer<SR> stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
        }

        public string NoResource => stringLocalizer[nameof(NoResource)];

        public string ValueInResource => stringLocalizer[nameof(ValueInResource)];

        public string EmptyValueInResource => stringLocalizer[nameof(EmptyValueInResource)];

        public string FormatNoResource(string name) => stringLocalizer[nameof(FormatNoResource), name];

        public string FormatValueInResource(string name) => stringLocalizer[nameof(FormatValueInResource), name];
    }
}
