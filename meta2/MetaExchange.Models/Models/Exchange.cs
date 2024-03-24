using System;
namespace MetaExchangeAPIv2.Models
{
    public class Exchange
    {
        public decimal BTCReserve { get; set; }
        public decimal EURReserve { get; set; }
        public Guid ExchangeId { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}

