using Newtonsoft.Json;

public class OrderBook
{
    public string AcqTime { get; set; }  // Timestamp from JSON
    public List<OrderWrapper> Bids { get; set; }  // Buy orders
    public List<OrderWrapper> Asks { get; set; }  // Sell orders
}

public class OrderWrapper
{
    public Order Order { get; set; }  // Order containing price and amount
}

public class Order
{
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
}

public class Balance
{
    public string Exchange { get; set; }  // Exchange name, can be matched with AcqTime
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

        // Deserialize single order book (list of order books)
        var orderBooks = JsonConvert.DeserializeObject<List<OrderBook>>(File.ReadAllText(orderBookPath));
        var balances = JsonConvert.DeserializeObject<List<Balance>>(File.ReadAllText(balancePath));

        string orderType = "sell";  // Change to "sell" if needed
        decimal btcAmount = 9.2m;  // Amount of BTC to buy or sell

        Console.WriteLine("Execution Plan:");
        var executionPlan = GetBestExecution(orderBooks, balances, orderType, btcAmount);

    }

    static List<Trade> GetBestExecution(List<OrderBook> orderBooks, List<Balance> balances, string orderType, decimal btcAmount)
    {
        var trades = new List<Trade>();

        var sortedOrders = orderType == "buy"
            ? orderBooks.SelectMany(ob => ob.Asks.Select(a => new { ob.AcqTime, a.Order.Price, a.Order.Amount }))
                        .OrderBy(o => o.Price).ToList()  // For buy, we need the lowest price
            : orderBooks.SelectMany(ob => ob.Bids.Select(b => new { ob.AcqTime, b.Order.Price, b.Order.Amount }))
                        .OrderByDescending(o => o.Price).ToList();  // For sell, we need the highest price

        foreach (var order in sortedOrders)
        {
            var balance = balances.FirstOrDefault(b => b.Exchange == order.AcqTime);  // Match balance based on AcqTime

            if (balance == null) continue;

            decimal availableAmount = orderType == "buy"
                ? Math.Min(order.Amount, balance.EUR / order.Price)  // For buy, use EUR balance
                : Math.Min(order.Amount, balance.BTC);  // For sell, use BTC balance

            if (availableAmount <= 0) continue;

            decimal amountToTrade = Math.Min(btcAmount, availableAmount);
            trades.Add(new Trade { Exchange = order.AcqTime, Price = order.Price, Amount = amountToTrade });

            // Update balance based on the order type
            if (orderType == "buy") balance.EUR -= amountToTrade * order.Price;
            else balance.BTC -= amountToTrade;

            btcAmount -= amountToTrade;

            // Print trade details
            Console.WriteLine($"Exchange: {order.AcqTime}, Price: {order.Price}, Amount: {amountToTrade:F4}, Remaining BTC to {orderType}: {btcAmount:F4}");

            if (btcAmount <= 0) break;  // Exit if no more BTC to trade
        }

        return trades;
    }
}
