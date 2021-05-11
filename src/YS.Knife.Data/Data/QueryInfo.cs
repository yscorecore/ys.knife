namespace YS.Knife.Data
{
    public class QueryInfo
    {
        public FilterInfo Filter { get; set; }
        public OrderInfo Order { get; set; }
        public LimitInfo Limit { get; set; }
        public SelectInfo Select { get; set; }
    }
}
