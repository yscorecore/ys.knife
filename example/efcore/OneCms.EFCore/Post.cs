using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCms.EFCore
{
    public class Post : CmsBaseEntity
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(5000)]
        public virtual string Content { get; set; }
        
        [Required]
        public virtual Topic Topic { get; set; }
    }
}
