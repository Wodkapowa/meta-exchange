namespace CryptoExecutionService.Models
{
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

    public class ExecutionRequest
    {
        public string OrderType { get; set; }
        public decimal BtcAmount { get; set; }
    }
}
