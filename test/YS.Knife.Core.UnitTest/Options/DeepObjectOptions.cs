using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
namespace YS.Knife.Options
{
    [Options]
    public class DeepObjectOptions
    {
        //[Required]
        public string Text { get; set; }
        [Required, ValidateObject]
        public AddressInfo Address { get; set; }
    }

    public class AddressInfo
    {
        public string Street { get; set; }

        public string City { get; set; }
        [ValidateObject]
        public Province Province { get; set; }
    }

    public class Province
    {

        public string Country { get; set; }
        [RegularExpression("\\d{3,5}")]
        public string Code { get; set; }
    }

}
