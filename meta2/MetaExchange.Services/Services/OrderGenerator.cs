using System.Collections.Concurrent;
using MetaExchangeAPIv2.Models;

public class OrderGenerator
{
    // Create a static shared instance of Random
    private static readonly Random sharedRandom = new Random();

    public static async Task<ConcurrentBag<Order>> GenerateRandomOrdersAsync(int orderInstances)
    {
        var orders = new ConcurrentBag<Order>();

        await Task.Run(() =>
        {
            for (int i = 0; i < orderInstances; i++)
            {
                var ask = new Order
                {
                    ExchangeId = Guid.NewGuid(),
                    OrderId = Guid.NewGuid(),
                    OrderType = "sell",
                    Amount = sharedRandom.Next(1, 100),
                    Price = sharedRandom.Next(500, 1001) 
                };
                orders.Add(ask);
            }

            for (int i = 0; i < orderInstances; i++)
            {
                var bid = new Order
                {
                    ExchangeId = Guid.NewGuid(),
                    OrderId = Guid.NewGuid(),
                    OrderType = "buy",
                    Amount = sharedRandom.Next(1, 100),
                    Price = sharedRandom.Next(500, 1001)
                };
                orders.Add(bid);
            }
        });

        return orders;
    }
}
