using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace YS.Knife.Elasticsearch
{
    [Options]
    public class ElasticOptions
    {
        [Required]
        [MaxLength(1000)]
        [MinLength(1)]

        public List<string> Urls { get; set; } = new List<string> { "http://localhost:9200" };
        [RegularExpression("^\\w+$")]
        [Required(AllowEmptyStrings =false)]
        public string DefaultIndex { get; set; } = "defaultindex";
    }
}
