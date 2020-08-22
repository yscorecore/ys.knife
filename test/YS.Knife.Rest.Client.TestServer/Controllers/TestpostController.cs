using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace YS.Knife.Rest.Client.TestServer
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestpostController : ControllerBase
    {


        // POST api/<TestpostController>


        [HttpPost]
        [Route("body")]

        public Result PostFromJsonBody([FromBody] Person person)
        {
            return new Result
            {
                Message = $"{person.Id}-{person.Name}",
                Success = true
            };
        }


        // POST api/<TestpostController>
        [HttpPost]
        [Route("form")]
        public Result PostFromForm([FromForm] Person person)
        {
            return new Result
            {
                Message = $"{person.Id}-{person.Name}-{JoinTags(person)}",
                Success = true
            };
        }

        private string JoinTags(Person person)
        {
            return string.Join("-", (person.Tags ?? new List<string>()));
        }

    }
    //[TypeConverter(typeof(PersonConverter))]
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<string> Tags { get; set; }
    }
    public class PersonConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string strValue)
            {
                var index = strValue.IndexOf(',');
                if (index > 0 && int.TryParse(strValue.Substring(0, index), out int id))
                {
                    return new Person
                    {
                        Id = id,
                        Name = strValue.Substring(index + 1)
                    };
                }
                return new Person
                {
                    Id = 999,
                    Name = strValue
                };
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

    public class Result
    {
        public bool Success { get; set; }

        public string Message { get; set; }
    }
}
