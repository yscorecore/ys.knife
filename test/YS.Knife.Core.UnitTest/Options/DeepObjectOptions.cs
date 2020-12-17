using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace YS.Knife.Options
{
    [Options]
    public class DeepObjectOptions
    {
        public string Text { get; set; }
        [Required, ValidateObject]
        public AddressInfo Address { get; set; }
    }

    [Options]
    public class DeepListOptions
    {
        public string Text { get; set; }

        [Required, ValidateObject]
        public List<AddressInfo> Addresses { get; set; }
    }


    [Options]
    public class DeepDicOptions
    {
        public string Text { get; set; }

        [Required, ValidateObject]
        public Dictionary<string, AddressInfo> Addresses { get; set; }
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
