using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

class OrderBook
{
    public string Exchange { get; set; }
    public List<Order> Bids { get; set; }
    public List<Order> Asks { get; set; }
}

class Order
{
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
}

class Balance
{
    public string Exchange { get; set; }
    public decimal EUR { get; set; }
    public decimal BTC { get; set; }
}

class Trade
{
    public string Exchange { get; set; }
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        string orderBookPath = "../../../BuyData.json"; // Adjust file path
        string balancePath = "../../../Balance.json";

        var orderBooks = JsonConvert.DeserializeObject<List<OrderBook>>(File.ReadAllText(orderBookPath));
        var balances = JsonConvert.DeserializeObject<List<Balance>>(File.ReadAllText(balancePath));

        string orderType = "buy"; // Change to "sell" if needed
        decimal btcAmount = 55;

        var executionPlan = GetBestExecution(orderBooks, balances, orderType, btcAmount);

        Console.WriteLine("Execution Plan:");
        foreach (var trade in executionPlan)
        {
            Console.WriteLine($"Exchange: {trade.Exchange}, Price: {trade.Price}, Amount: {trade.Amount}");
        }
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
            if (btcAmount <= 0) break;
        }

        return trades;
    }
}

public class ExecutionRequest
{
    public string OrderType { get; set; }
    public decimal BtcAmount { get; set; }
}
