namespace CryptoExecutionService.Models
{
    public class OrderBook
    {
        public string Exchange { get; set; }
        public List<Order> Bids { get; set; }
        public List<Order> Asks { get; set; }
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
