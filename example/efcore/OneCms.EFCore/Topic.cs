using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCms.EFCore
{
    public class Topic : CmsBaseEntity
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(256)]
        public virtual string Title { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(50000)]
        public virtual string Content { get; set; }

        public virtual List<Post> Posts { get; set; }
    }
}
