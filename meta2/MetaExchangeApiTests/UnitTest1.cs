using MetaExchangeAPIv2.Models;
public class MetaExchangeServiceTests
{
    [Fact]
    public void GetBestPriceOrders_EmptyOrderList_ReturnsEmptyList()
    {
        var orders = new List<Order>();
        var orderType = "buy";
        var amount = 10m;

        var actualOrders = MetaExchangeService.GetBestPriceOrders(orders, orderType, amount);

        Assert.Empty(actualOrders);
    }

    [Fact]
    public void GetBestPriceOrders_InvalidOrderType_ThrowsArgumentException()
    {
        var orders = new List<Order>() { new Order { OrderType = "sell", Price = 10, Amount = 1 } };
        var orderType = "invalid";
        var amount = 10m;

        Assert.Throws<ArgumentException>(() => MetaExchangeService.GetBestPriceOrders(orders, orderType, amount));
    }

    [Fact]
    public void GetBestPriceOrders_ZeroAmount_ThrowsArgumentException()
    {
        var orders = new List<Order>() { new Order { OrderType = "sell", Price = 10, Amount = 1 } };
        var orderType = "buy";
        var amount = 0m;

        Assert.Throws<ArgumentException>(() => MetaExchangeService.GetBestPriceOrders(orders, orderType, amount));
    }

    [Fact]
    public void GetBestPriceOrders_NegativeAmount_ThrowsArgumentException()
    {
        var orders = new List<Order>() { new Order { OrderType = "sell", Price = 10, Amount = 1 } };
        var orderType = "buy";
        var amount = -10m;

        Assert.Throws<ArgumentException>(() => MetaExchangeService.GetBestPriceOrders(orders, orderType, amount));
    }


    [Fact]
    public void GetBestPriceOrders_InsufficientOrderAmount_ReturnsAllOrdersWithLowestPriceFirst()
    {
        var orders = new List<Order>()
  {
      new Order { OrderType = "sell", Price = 10m, Amount = 0.1m },
      new Order { OrderType = "sell", Price = 11m, Amount = 0.2m },
      new Order { OrderType = "sell", Price = 9m, Amount = 0.3m }, // Added an order with the lowest price
  };
        var orderType = "buy";
        var amount = 0.4m;

        var actualOrders = MetaExchangeService.GetBestPriceOrders(orders, orderType, amount).ToList();

        // Assert the first item has the lowest price
        Assert.Equal(9m, actualOrders.First().Price);

        // Assert all remaining orders have a higher or equal price
        Assert.True(actualOrders.Skip(1).All(order => order.Price >= actualOrders.First().Price));
    }

    [Fact]
    public void GetBestPriceOrdersPriorityQueue_ReturnsBestPriceOrders_WhenValidArgs()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order { OrderType = "buy", Amount = 10, Price = 100 },
            new Order { OrderType = "sell", Amount = 15, Price = 105 },
            new Order { OrderType = "buy", Amount = 8, Price = 95 },
            new Order { OrderType = "sell", Amount = 20, Price = 110 }
        };
        string orderType = "buy";
        decimal amount = 50;

        // Act
        var bestPriceOrders = MetaExchangeService.GetBestPriceOrdersPriorityQueue(orders, orderType, amount);

        // Assert
        Assert.Equal(2, bestPriceOrders.Count);
        Assert.True(bestPriceOrders.All(order => order.OrderType == "sell"));
        Assert.Equal(105, bestPriceOrders[0].Price);
        Assert.Equal(110, bestPriceOrders[1].Price);

    }

    [Fact]
    public void GetBestPriceOrdersPriorityQueue_ReturnsBestSellOrders_WhenValidArgs()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order { OrderType = "buy", Amount = 10, Price = 100 },
            new Order { OrderType = "sell", Amount = 15, Price = 105 },
            new Order { OrderType = "buy", Amount = 8, Price = 95 },
            new Order { OrderType = "buy", Amount = 20, Price = 110 },
            new Order { OrderType = "sell", Amount = 20, Price = 110 },
            new Order { OrderType = "buy", Amount = 20, Price = 108 },
            new Order { OrderType = "buy", Amount = 20, Price = 116 }
        };
        string orderType = "sell";
        decimal amount = 50;

        // Act
        var bestPriceOrders = MetaExchangeService.GetBestPriceOrdersPriorityQueue(orders, orderType, amount);

        // Assert
        Assert.True(bestPriceOrders.All(order => order.OrderType == "buy"));
        Assert.Equal(116, bestPriceOrders[0].Price); //20 filled
        Assert.Equal(110, bestPriceOrders[1].Price); //20 filled, and 10 left, search for the best solution to fill 10 for the best price
        Assert.Equal(108, bestPriceOrders[2].Price); //fill left 10 @ 108 per btc
        Assert.Equal(3, bestPriceOrders.Count);
    }

    [Fact]
    public void GetBestPriceOrdersPriorityQueue_ThrowsArgumentException_WhenInvalidOrderType()
    {
        // Arrange
        var orders = new List<Order>();
        string invalidOrderType = "invalid";
        decimal amount = 50;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => MetaExchangeService.GetBestPriceOrdersPriorityQueue(orders, invalidOrderType, amount));
    }

    [Fact]
    public void GetBestPriceOrdersPriorityQueue_ThrowsArgumentException_WhenAmountIsZero()
    {
        // Arrange
        var orders = new List<Order>();
        string orderType = "buy";
        decimal zeroAmount = 0;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => MetaExchangeService.GetBestPriceOrdersPriorityQueue(orders, orderType, zeroAmount));
    }
}
