namespace YS.Knife.Data
{
    public enum FilterType : int
    {
        /// <summary>
        /// == eq
        /// </summary>
        Equals = 0,
        /// <summary>
        /// != neq
        /// </summary>
        NotEquals = ~Equals,
        /// <summary>
        /// 大于 gt
        /// </summary>
        GreaterThan = 1,
        /// <summary>
        /// 小于或等于 ngt
        /// </summary>
        LessThanOrEqual = ~GreaterThan,
        /// <summary>
        /// 小于 lt
        /// </summary>
        LessThan = 2, 
        /// <summary>
        /// 大于或等于 nlt
        /// </summary>
        GreaterThanOrEqual = ~LessThan,
        /// <summary>
        /// Between bt
        /// </summary>
        Between = 3,
        /// <summary>
        /// Not Between nbt
        /// </summary>
        NotBetween = ~Between,
        /// <summary>
        /// In    in
        /// </summary>
        In = 4,
        /// <summary>
        /// Not In nin
        /// </summary>
        NotIn = ~In,
        /// <summary>
        /// Like sw
        /// </summary>
        StartsWith = 5,
        /// <summary>
        /// not like nsw
        /// </summary>
        NotStartsWith = ~StartsWith,
        /// <summary>
        /// like ew
        /// </summary>
        EndsWith = 6,
        /// <summary>
        /// not like new
        /// </summary>
        NotEndsWith = ~EndsWith,
        /// <summary>
        /// like 
        /// </summary>
        Contains = 7,
        /// <summary>
        /// not like
        /// </summary>
        NotContains = ~Contains,
        /// <summary>
        /// exists
        /// </summary>
        Exists = 8,
        /// <summary>
        /// not exists
        /// </summary>
        NotExists = ~Exists,
        /// <summary>
        /// all 
        /// </summary>
        All = 9,
        /// <summary>
        /// not all
        /// </summary>
        NotAll = ~All,

    }

}
