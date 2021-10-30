using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YS.Knife.Data;
namespace OneCms.EFCore
{
    public abstract class CmsBaseEntity : YS.Knife.Entity.Model.BaseEntity<Guid>
    {
        
        public override Guid Id { get; set; }
    }
}
