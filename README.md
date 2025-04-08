# Meta Exchange Optimizer

A .NET Core console application that executes optimal BTC buy/sell orders across multiple crypto exchanges, considering individual exchange balances and market prices. 

## Features

- Optimally distributes BTC buy/sell orders across multiple exchanges.
- Considers current EUR/BTC balances for each exchange.
- Prevents fund transfers between exchanges.
- Outputs best execution plan with summary.
- Future API support via Kestrel (.NET Core Web API).

## Sample Input

```json
{
  "orderType": "buy",
  "btcAmount": 1.5,
  "balances": [
    { "exchange": "Exchange1", "EUR": 10000, "BTC": 0.5 },
    { "exchange": "Exchange2", "EUR": 8000, "BTC": 1.0 }
  ]
}