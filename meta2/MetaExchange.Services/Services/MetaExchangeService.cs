    using System.Collections.Concurrent;
    using MetaExchangeAPIv2.Models;

    public static class MetaExchangeService
    {

        public static List<Order> GetBestPriceOrdersPriorityQueue(List<Order> orders, string orderType, decimal amount)
        {
            ValidateArgs(orderType, amount);

            List<Order> bestOrders = new List<Order>();

            // Create a priority queue based on the order type
            var priorityQueue = new PriorityQueue<Order>(orderType == "buy" ? Order.CompareByPriceAsc : Order.CompareByPriceDesc);

            // Populate the priority queue
            foreach (var order in orders)
            {
                // Enqueue orders only if their type matches the desired type
                if (order.OrderType != orderType)
                {
                    priorityQueue.Enqueue(order);
                }
            }

            // Process orders to fulfill the desired amount
            decimal remainingAmount = amount;
            while (remainingAmount > 0 && priorityQueue.Count > 0)
            {
                var order = priorityQueue.Dequeue();
                decimal amountToProcess = Math.Min(remainingAmount, order.Amount);
                remainingAmount -= amountToProcess;

                bestOrders.Add(new Order
                {
                    ExchangeId = order.ExchangeId,
                    OrderId = order.OrderId,
                    OrderType = order.OrderType,
                    Amount = amountToProcess,
                    Price = order.Price
                });
            }

            return bestOrders;
        }

        public static List<Order> GetBestPriceOrdersAVLTree(List<Order> orders, string orderType, decimal amount)
        {

            ValidateArgs(orderType, amount);

            // Create an AVL tree
            AVLTree tree = new AVLTree();

            // Insert orders into the AVL tree
            foreach (var order in orders)
            {
                // Filter orders based on order type
                if ((orderType == "buy" || order.OrderType == "sell"))
                {
                    tree.Insert(order);
                }
            }

            // Perform in-order traversal to get best orders
            List<Order> bestOrders = tree.InOrderTraversal();

            // Process orders to fulfill the desired amount
            decimal remainingAmount = amount;
            List<Order> selectedOrders = new List<Order>();
            foreach (var order in bestOrders)
            {
                if (remainingAmount <= 0)
                    break;

                decimal amountToProcess = Math.Min(remainingAmount, order.Amount);
                remainingAmount -= amountToProcess;

                selectedOrders.Add(new Order
                {
                    ExchangeId = order.ExchangeId,
                    OrderId = order.OrderId,
                    OrderType = order.OrderType,
                    Amount = amountToProcess,
                    Price = order.Price
                });
            }

            return selectedOrders;
        }

        public static List<Order> GetBestPriceOrders(List<Order> orders, string orderType, decimal amount)
        {
            ValidateArgs(orderType, amount);

            List<Order> bestOrders = new List<Order>();

            // Sort orders based on price (lowest for buy, highest for sell)
            IEnumerable<Order> sortedOrders = orderType == "buy" ?
                orders.Where(order => order.OrderType == "sell").OrderBy(order => order.Price) :
                orders.Where(order => order.OrderType == "buy").OrderByDescending(order => order.Price);

            // Process orders to fulfill the desired amount
            decimal remainingAmount = amount;
            foreach (var order in sortedOrders)
            {
                if (remainingAmount <= 0)
                    break;

                decimal amountToProcess = Math.Min(remainingAmount, order.Amount);
                remainingAmount -= amountToProcess;

                bestOrders.Add(new Order
                {
                    ExchangeId = order.ExchangeId,
                    OrderId = order.OrderId,
                    OrderType = order.OrderType,
                    Amount = amountToProcess,
                    Price = order.Price
                });
            }

            return bestOrders;
        }

        private static void ValidateArgs(string orderType, decimal amount)
        {
            if (orderType != "buy" && orderType != "sell")
            {
                throw new ArgumentException("Invalid argument for 'orderType'. Must be sell or buy");
            }

            if (amount <= 0)
            {
                throw new ArgumentException("Invalid argument for 'amount'. Must be greater than 0");
            }
        }

        public static List<Order> GetBestPriceOrdersSortedSet(List<Order> orders, string orderType, decimal amount)
        {

            ValidateArgs(orderType, amount);

            List<Order> bestOrders = new List<Order>();

            // Create a sorted dictionary to maintain the order book
            var orderBook = new SortedDictionary<decimal, List<Order>>();

            // Populate the order book
            foreach (var order in orders)
            {
                if (!orderBook.ContainsKey(order.Price))
                    orderBook[order.Price] = new List<Order>();

                orderBook[order.Price].Add(order);
            }

            // Process orders to fulfill the desired amount
            decimal remainingAmount = amount;
            var comparer = orderType == "buy" ? Comparer<decimal>.Default : Comparer<decimal>.Create((x, y) => -x.CompareTo(y));
            foreach (var priceOrders in orderBook)
            {
                foreach (var order in priceOrders.Value.OrderBy(o => o.Price, comparer))
                {
                    if (remainingAmount <= 0)
                        break;

                    if (orderType == "buy" && order.OrderType == "sell" ||
                        orderType == "sell" && order.OrderType == "buy")
                    {
                        decimal amountToProcess = Math.Min(remainingAmount, order.Amount);
                        remainingAmount -= amountToProcess;

                        bestOrders.Add(new Order
                        {
                            ExchangeId = order.ExchangeId,
                            OrderId = order.OrderId,
                            OrderType = order.OrderType,
                            Amount = amountToProcess,
                            Price = order.Price
                        });
                    }
                }
            }

            return bestOrders;
        }

        public static async Task<List<Order>> GetBestOrders(decimal amount, string orderType)
        {
            // Create some exchanges (otherwise external call to cex/dex)
            var exchanges = new List<Exchange>
        {
            new Exchange { BTCReserve = 400, EURReserve = 50000, ExchangeId = Guid.NewGuid() },
            new Exchange { BTCReserve = 500, EURReserve = 45000, ExchangeId = Guid.NewGuid() },
            new Exchange { BTCReserve = 700, EURReserve = 55000, ExchangeId = Guid.NewGuid() },
            new Exchange { BTCReserve = 800, EURReserve = 70000, ExchangeId = Guid.NewGuid() }
        };

            // Generate random orders asynchronously and push them to exchanges
            ConcurrentBag<Order> orders = await OrderGenerator.GenerateRandomOrdersAsync(100);
            Parallel.ForEach(exchanges, exchange =>
            {
         
                   exchange.Orders.AddRange(orders);
          
            });

            // Find the best orders in parallel
            ConcurrentDictionary<Guid, Order> processedOrders = new ConcurrentDictionary<Guid, Order>();

            Parallel.ForEach(exchanges, exchange =>
            {
                var exchangeOrders = GetBestPriceOrdersSortedSet(exchange.Orders, orderType, amount);
                foreach (var order in exchangeOrders)
                {
                    processedOrders.TryAdd(order.OrderId, order);
                }
            });

            // Sort and return best orders
            return orderType == "sell" ?
                processedOrders.Values.OrderByDescending(order => order.Price).ThenByDescending(order => order.Amount).ToList() :
                processedOrders.Values.OrderBy(order => order.Price).ThenByDescending(order => order.Amount).ToList();
        }

    }