using CryptoExecutionService.Models;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ExecutionController : ControllerBase
{
    private readonly ExecutionService _executionService;

    public ExecutionController(ExecutionService executionService)
    {
        _executionService = executionService;
    }

    [HttpPost("GetBestExecution")]
    public ActionResult<List<Trade>> GetBestExecution([FromBody] ExecutionRequest request)
    {
        // Example of order book data
        var orderBooks = new List<OrderBook>
        {
            new OrderBook
            {
                Exchange = "ExchangeA",
                Bids = new List<Order>
                {
                    new Order { Price = 2960.5m, Amount = 0.5m },
                    new Order { Price = 2959.7m, Amount = 1.0m }
                },
                Asks = new List<Order>
                {
                    new Order { Price = 3000m, Amount = 0.1m },
                    new Order { Price = 3050m, Amount = 0.5m }
                }
            }
        };

        // Example of balances
        var balances = new List<Balance>
        {
            new Balance
            {
                Exchange = "ExchangeA",
                BTC = 1.5m,
                EUR = 10000m
            }
        };

        // Call the service to get the best execution plan
        var trades = _executionService.GetBestExecution(orderBooks, balances, request.OrderType, request.BtcAmount);

        if (trades.Count == 0)
        {
            return NotFound("No trades could be executed.");
        }

        return Ok(trades);
    }
}
