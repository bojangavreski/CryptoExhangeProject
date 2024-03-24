namespace MetaExchangeAPIv2.Models
{
    public class Order : IComparable<Order>
	{
        public Guid ExchangeId { get; set; }
        public Guid OrderId { get; set; }
        public string OrderType { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }

        public int CompareTo(Order other)
        {
            // Compare orders based on price
            return this.Price.CompareTo(other.Price);
        }

        public static Comparison<Order> CompareByPriceAsc = (x, y) => x.Price.CompareTo(y.Price);
        public static Comparison<Order> CompareByPriceDesc = (x, y) => y.Price.CompareTo(x.Price);
    }
}

