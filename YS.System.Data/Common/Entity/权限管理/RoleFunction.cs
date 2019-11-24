using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace System.Data.Common
{
    /// <summary>
    /// 表示角色功能点关系
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class RoleFunction<TKey> : BaseEntity, ICreateTrack
    {
        public TKey RoleId { get; set; }
        public TKey FunctionId { get; set; }
        /// <summary>
        /// 表示是否是允许的权限，默认为true
        /// </summary>
        public bool Allow { get; set; }
    }
    public class RoleFunction : RoleFunction<Guid>
    {

    }
}
