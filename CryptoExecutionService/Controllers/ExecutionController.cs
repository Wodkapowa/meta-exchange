using CryptoExecutionService.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        try
        {
            // Validate input
            if (request.BtcAmount <= 0)
                return BadRequest("BTC amount must be greater than zero.");
            if (string.IsNullOrEmpty(request.OrderType) || !(request.OrderType.ToLower() == "buy" || request.OrderType.ToLower() == "sell"))
                return BadRequest("Invalid order type. Please specify 'buy' or 'sell'.");

            // Load order book and balances from JSON files
            var orderBooks = JsonConvert.DeserializeObject<List<OrderBook>>(System.IO.File.ReadAllText("BuyData.json"));
            var balances = JsonConvert.DeserializeObject<List<Balance>>(System.IO.File.ReadAllText("Balance.json"));

            // Call the service to get the best execution plan
            var trades = _executionService.GetBestExecution(orderBooks, balances, request.OrderType, request.BtcAmount);

            if (trades.Count == 0)
            {
                return NotFound("No trades could be executed.");
            }

            return Ok(trades);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

}
