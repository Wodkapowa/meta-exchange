using CryptoExecutionService.Models;

    public class ExecutionService
    {
        public List<Trade> GetBestExecution(List<OrderBook> orderBooks, List<Balance> balances, string orderType, decimal btcAmount)
        {
            var trades = new List<Trade>();

            var sortedOrders = orderType == "buy"
                ? orderBooks.SelectMany(ob => ob.Asks.Select(a => new { ob.AcqTime, a.Order.Price, a.Order.Amount }))
                            .OrderBy(o => o.Price).ToList()
                : orderBooks.SelectMany(ob => ob.Bids.Select(b => new { ob.AcqTime, b.Order.Price, b.Order.Amount }))
                            .OrderByDescending(o => o.Price).ToList();

            foreach (var order in sortedOrders)
            {
                var balance = balances.FirstOrDefault(b => b.Exchange == order.AcqTime);
                if (balance == null) continue;

                decimal availableAmount = orderType == "buy"
                    ? Math.Min(order.Amount, balance.EUR / order.Price)
                    : Math.Min(order.Amount, balance.BTC);

                if (availableAmount <= 0) continue;

                decimal amountToTrade = Math.Min(btcAmount, availableAmount);
                trades.Add(new Trade { Exchange = order.AcqTime, Price = order.Price, Amount = amountToTrade });

                if (orderType == "buy") balance.EUR -= amountToTrade * order.Price;
                else balance.BTC -= amountToTrade;

                btcAmount -= amountToTrade;

                if (btcAmount <= 0) break;
            }

            return trades;
        }
    }
