
namespace YS.Knife.Data.Query
{
    public enum Operator : int
    {
        /// <summary>
        /// == eq
        /// </summary>
        [OperatorCode("==")]
        [OperatorCode("=")]
        Equals = 0,
        /// <summary>
        /// != neq
        /// </summary>
        [OperatorCode("!=")]
        [OperatorCode("<>")]
        NotEquals = ~Equals,
        /// <summary>
        /// 大于 gt
        /// </summary>
        [OperatorCode(">")]
        GreaterThan = 1,
        /// <summary>
        /// 小于或等于 ngt
        /// </summary>
        [OperatorCode("<=")]
        LessThanOrEqual = ~GreaterThan,
        /// <summary>
        /// 小于 lt
        /// </summary>
        [OperatorCode("<")]
        LessThan = 2,
        /// <summary>
        /// 大于或等于 nlt
        /// </summary>
        [OperatorCode(">=")]
        GreaterThanOrEqual = ~LessThan,
        /// <summary>
        /// Between bt
        /// </summary>
        [OperatorCode("bt")]
        [OperatorCode("between")]
        Between = 3,
        /// <summary>
        /// Not Between nbt
        /// </summary>
        [OperatorCode("nbt")]
        [OperatorCode("not_between")]
        NotBetween = ~Between,
        /// <summary>
        /// In    in
        /// </summary>
        [OperatorCode("in")]
        In = 4,
        /// <summary>
        /// Not In nin
        /// </summary>
        [OperatorCode("nin")]
        [OperatorCode("not_in")]
        NotIn = ~In,
        /// <summary>
        /// Like sw
        /// </summary>
        [OperatorCode("sw")]
        [OperatorCode("startswith")]
        StartsWith = 5,
        /// <summary>
        /// not like nsw
        /// </summary>
        [OperatorCode("nsw")]
        [OperatorCode("not_startswith")]
        NotStartsWith = ~StartsWith,
        /// <summary>
        /// like ew
        /// </summary>
        [OperatorCode("ew")]
        [OperatorCode("endswith")]
        EndsWith = 6,
        /// <summary>
        /// not like new
        /// </summary>
        [OperatorCode("new")]
        [OperatorCode("not_endswith")]
        NotEndsWith = ~EndsWith,
        /// <summary>
        /// like 
        /// </summary>
        [OperatorCode("ct")]
        [OperatorCode("contains")]
        Contains = 7,
        /// <summary>
        /// not like
        /// </summary>
        [OperatorCode("nct")]
        [OperatorCode("not_contains")]
        NotContains = ~Contains,
      

    }

}
