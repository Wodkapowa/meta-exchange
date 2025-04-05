using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class OrderBook
{
    public string Exchange { get; set; }
    public List<Order> Bids { get; set; }  // Buy orders
    public List<Order> Asks { get; set; }  // Sell orders
}

public class Order
{
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
}

public class Balance
{
    public string Exchange { get; set; }
    public decimal BTC { get; set; }
    public decimal EUR { get; set; }
}

public class Trade
{
    public string Exchange { get; set; }
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        string orderBookPath = "../../../BuyData.json";  // Path to your single order book JSON file
        string balancePath = "../../../Balance.json";   // Path to your balance JSON file

        // Deserialize single order book (not a list anymore)
        var orderBooks = JsonConvert.DeserializeObject<List<OrderBook>>(File.ReadAllText(orderBookPath));
        var balances = JsonConvert.DeserializeObject<List<Balance>>(File.ReadAllText(balancePath));
        
        string orderType = "buy"; // Change to "sell" if needed
        decimal btcAmount = 55;


        Console.WriteLine("Execution Plan:");
        var executionPlan = GetBestExecution(orderBooks, balances, orderType, btcAmount);

    }

    static List<Trade> GetBestExecution(List<OrderBook> orderBooks, List<Balance> balances, string orderType, decimal btcAmount)
    {
        var trades = new List<Trade>();

        var sortedOrders = orderType == "buy"
            ? orderBooks.SelectMany(ob => ob.Asks.Select(a => new { ob.Exchange, a.Price, a.Amount }))
                        .OrderBy(o => o.Price).ToList()
            : orderBooks.SelectMany(ob => ob.Bids.Select(b => new { ob.Exchange, b.Price, b.Amount }))
                        .OrderByDescending(o => o.Price).ToList();

        foreach (var order in sortedOrders)
        {
            var balance = balances.FirstOrDefault(b => b.Exchange == order.Exchange);
            if (balance == null) continue;

            decimal availableAmount = orderType == "buy"
                ? Math.Min(order.Amount, balance.EUR / order.Price)
                : Math.Min(order.Amount, balance.BTC);

            if (availableAmount <= 0) continue;

            decimal amountToTrade = Math.Min(btcAmount, availableAmount);
            trades.Add(new Trade { Exchange = order.Exchange, Price = order.Price, Amount = amountToTrade });

            if (orderType == "buy") balance.EUR -= amountToTrade * order.Price;
            else balance.BTC -= amountToTrade;

            btcAmount -= amountToTrade;

            Console.WriteLine($"Exchange: {order.Exchange}, Price: {order.Price}, Amount: {amountToTrade:F4}, Remaining BTC to {orderType}: {btcAmount:F4}, balance left eur {balance.EUR:F4}");

            if (btcAmount <= 0) break;
        }

        return trades;
    }
}
