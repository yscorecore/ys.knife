using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YS.Knife
{
    public enum SearchType:int
    {
        /// <summary>
        /// ==
        /// </summary>
        Equals = 0,
        /// <summary>
        /// !=
        /// </summary>
        NotEquals = ~Equals,
        /// <summary>
        /// 大于
        /// </summary>
        GreaterThan = 1,
        /// <summary>
        /// 小于或等于
        /// </summary>
        LessThanOrEqual = ~GreaterThan,
        /// <summary>
        /// 小于
        /// </summary>
        LessThan = 2,
        /// <summary>
        /// 大于或等于
        /// </summary>
        GreaterThanOrEqual = ~LessThan,
        /// <summary>
        /// Between
        /// </summary>
        Between = 3,
        /// <summary>
        /// Not Between
        /// </summary>
        NotBetween = ~Between,
        /// <summary>
        /// In
        /// </summary>
        In = 4,
        /// <summary>
        /// Not In
        /// </summary>
        NotIn = ~In,
        /// <summary>
        /// Like
        /// </summary>
        StartsWith = 5,
        /// <summary>
        /// not like
        /// </summary>
        NotStartsWith = ~StartsWith,
        /// <summary>
        /// like
        /// </summary>
        EndsWith = 6,
        /// <summary>
        /// not like
        /// </summary>
        NotEndsWith = ~EndsWith,
        /// <summary>
        /// like
        /// </summary>
        Contains = 7,
        /// <summary>
        /// not like
        /// </summary>
        NotContains = ~Contains
    }

}
