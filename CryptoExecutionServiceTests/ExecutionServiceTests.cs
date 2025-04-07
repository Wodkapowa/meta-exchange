using CryptoExecutionService.Models;

namespace Tests
{
    [TestClass()]
    public class ExecutionServiceTests
    {
        private readonly ExecutionService _executionService;

        public ExecutionServiceTests()
        {
            _executionService = new ExecutionService();
        }

        [TestMethod]
        public void Test_BuyOrder_FulfillsRequestCorrectly()
        {
            // Arrange
            var orderBooks = new List<OrderBook>
            {
                new OrderBook
                {
                    AcqTime = "Exchange1",
                    Asks = new List<OrderWrapper>
                    {
                        new OrderWrapper { Order = new Order { Price = 50m, Amount = 2m } },
                        new OrderWrapper { Order = new Order { Price = 55m, Amount = 1m } }
                    }
                }
            };

            var balances = new List<Balance>
            {
                new Balance { Exchange = "Exchange1", BTC = 0m, EUR = 150m }
            };

            var request = new ExecutionRequest { OrderType = "buy", BtcAmount = 1m };

            // Act
            var trades = _executionService.GetBestExecution(orderBooks, balances, request.OrderType, request.BtcAmount);

            // Assert
            Assert.AreEqual("Exchange1", trades.First().Exchange);
            Assert.AreEqual(50m, trades.First().Price);
            Assert.AreEqual(1m, trades.First().Amount);
            Assert.AreEqual(100m, balances.First().EUR); // EUR balance should decrease by 50 * 1
        }

        [TestMethod]
        public void Test_SellOrder_FulfillsRequestCorrectly()
        {
            // Arrange
            var orderBooks = new List<OrderBook>
            {
                new OrderBook
                {
                    AcqTime = "Exchange1",
                    Bids = new List<OrderWrapper>
                    {
                        new OrderWrapper { Order = new Order { Price = 50m, Amount = 2m } },
                        new OrderWrapper { Order = new Order { Price = 55m, Amount = 1m } }
                    }
                }
            };

            var balances = new List<Balance>
            {
                new Balance { Exchange = "Exchange1", BTC = 2m, EUR = 0m }
            };

            var request = new ExecutionRequest { OrderType = "sell", BtcAmount = 1m };

            // Act
            var trades = _executionService.GetBestExecution(orderBooks, balances, request.OrderType, request.BtcAmount);

            // Assert
            Assert.AreEqual("Exchange1", trades.First().Exchange);
            Assert.AreEqual(55m, trades.First().Price);
            Assert.AreEqual(1m, trades.First().Amount);
            Assert.AreEqual(1m, balances.First().BTC); // BTC balance should decrease by 1m
        }

        [TestMethod]
        public void Test_PartialOrderRequestWithInsufficientFunds()
        {
            // Arrange
            var orderBooks = new List<OrderBook>
            {
                new OrderBook
                {
                    AcqTime = "Exchange1",
                    Asks = new List<OrderWrapper>
                    {
                        new OrderWrapper { Order = new Order { Price = 50m, Amount = 2m } },
                        new OrderWrapper { Order = new Order { Price = 55m, Amount = 1m } }
                    }
                }
            };

            var balances = new List<Balance>
            {
                new Balance { Exchange = "Exchange1", BTC = 0m, EUR = 100m }
            };

            var request = new ExecutionRequest { OrderType = "buy", BtcAmount = 2m };

            // Act
            var trades = _executionService.GetBestExecution(orderBooks, balances, request.OrderType, request.BtcAmount);

            // Assert
            Assert.AreEqual("Exchange1", trades.First().Exchange);
            Assert.AreEqual(50m, trades.First().Price);
            Assert.AreEqual(2m, trades.First().Amount); // Should only buy 2m BTC
            Assert.AreEqual(0m, balances.First().EUR); // EUR balance should be fully spent
        }

        [TestMethod]
        public void Test_ZeroBtcAmountRequest()
        {
            // Arrange
            var orderBooks = new List<OrderBook>
            {
                new OrderBook
                {
                    AcqTime = "Exchange1",
                    Asks = new List<OrderWrapper>
                    {
                        new OrderWrapper { Order = new Order { Price = 50m, Amount = 2m } }
                    }
                }
            };

            var balances = new List<Balance>
            {
                new Balance { Exchange = "Exchange1", BTC = 0m, EUR = 100m }
            };

            var request = new ExecutionRequest { OrderType = "buy", BtcAmount = 0m };

            // Act
            var trades = _executionService.GetBestExecution(orderBooks, balances, request.OrderType, request.BtcAmount);

            // Assert
            Assert.AreEqual(0, trades.First().Amount);
        }
    }
}
