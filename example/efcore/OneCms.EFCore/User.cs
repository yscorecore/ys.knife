using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OneCms.EFCore
{
    public class User : CmsBaseEntity
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(100)]
        public virtual string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(512)]
        public virtual string IconUrl { get; set; }
        public virtual List<Post> MyPosts { get; set; }
        public virtual List<Topic> MyTopics { get; set; }

        public virtual List<Post> LikePosts { get; set;}
        public virtual List<Topic> LikeTopics { get; set; }


    }
}
