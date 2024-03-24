using System;

namespace CLITool
{
    class Program
    {
        static async Task Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Enter order type (buy/sell) or press 'x' to exit:");
                string orderTypeInput = Console.ReadLine().ToLower();

                if (orderTypeInput == "x")
                    break;

                if (orderTypeInput != "buy" && orderTypeInput != "sell")
                {
                    Console.WriteLine("Invalid order type. Please enter 'buy' or 'sell'.");
                    continue;
                }

                Console.WriteLine("Enter amount of bitcoins to buy/sell:");
                if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
                {
                    Console.WriteLine("Invalid amount. Please enter a valid decimal number.");
                    continue;
                }

                var bestOrders = await MetaExchangeService.GetBestOrders(amount, orderTypeInput);

                Console.WriteLine($"ExchangeId | OrderType | Amount | Price");
                foreach (var order in bestOrders)
                {
                    Console.WriteLine($"{order.ExchangeId} | {order.OrderType} | {order.Amount} | {order.Price}");
                }
            }
        }
    }
}
