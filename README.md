# Fixed Income Pricing Library (.NET 9)

A production-grade C# library for pricing fixed income derivatives, calculating risk metrics (Greeks), and performing portfolio-level analytics including Value-at-Risk (VaR).

## Key Features

### 1. Multi-Model Pricing Engine
The system automatically routes instruments to the most appropriate mathematical model:
*   **Bonds (Fixed/Zero) & Swaps:** Analytical Discounting Engine using multi-curve framework (SOFR for discounting).
*   **Swaptions:** Black-76 model calibrated to normal volatility cubes.
*   **Caps & Floors:** Black-76 model with support for volatility smiles and individual caplet/floorlet valuation.
*   **Exotics:** Hull-White One-Factor model using both Monte Carlo path generation and Trinomial Trees (Backward Induction).

### 2. Market Data & Calibration
*   **Yield Curve Construction:** Bootstrapped curves from Treasury quotes with Log-Linear interpolation.
*   **Automatic Calibration:** The Hull-White model auto-calibrates to market instruments (swaptions/bonds) using Nelder-Mead optimization.
*   **Volatility Surfaces:** Support for 3D swaption cubes and 2D cap/floor smile surfaces.

### 3. Risk & Portfolio Analytics
*   **Greeks:** Numerical calculation of DV01 and Modified Duration via parallel curve shifting.
*   **Scenario Analysis:** Portfolio P&L analysis across parallel yield curve shocks (-100bp to +100bp).
*   **Value-at-Risk (VaR):** 
    *   **Monte Carlo VaR:** 10,000+ simulations with historical rate volatility.
    *   **CVaR (Expected Shortfall):** Tail risk assessment for extreme events.
    *   **Parametric VaR:** Variance-covariance approach using normal distributions.

---

## Project Structure

```text
FixedIncomePricingLibrary/
├── src/
│   ├── Core/           (Interfaces, Enums, Models)
│   ├── Analytics/      (DayCount, Calendars, CashFlows)
│   ├── MarketData/     (YieldCurves, Volatility, CSV Providers)
│   ├── Instruments/    (Bonds, Swaps, Options, Caps/Floors)
│   ├── Pricing/        (Engines, Hull-White, Black-76, MC)
│   └── Risk/           (Greeks, VaR, Portfolio Management)
├── tests/
│   ├── UnitTests/      (30+ tests for math correctness)
│   └── IntegrationTests/
├── samples/
│   └── ConsoleApp/     (Interactive Trading Desk UI)
└── data/
    ├── market_data/    (Yield curves, Vol surfaces)
    ├── historical/     (Historical data for VaR)
    └── calendars/      (Holiday calendars)
```

---

## Usage

### Prerequisites
*   .NET 9.0 SDK

### Running the Interactive Console
The console app acts as a "mini-trading desk" that automatically loads market data and routes instruments to the correct models.

```powershell
dotnet run --project samples/ConsoleApp/ConsoleApp.csproj
```

**Recommended Flow:**
1.  **Option 1:** Load Market Data (Loads `usd_treasury_curve.csv` + `swaption_volatilities.csv` and auto-calibrates HW).
2.  **Option 2:** Add Instruments (Bonds, Swaps, Swaptions, or Caps).
3.  **Option 4:** Price Portfolio (Automatic routing to Black-76 or Discounting).
4.  **Option 7:** Run VaR (Uses historical volatility from the curve data).

### Running Tests
```powershell
dotnet test
```

---

## Market Data CSV Formats

The system expects CSV files in the `data/market_data` folder:
*   **usd_treasury_curve.csv:** Tenors from 1M to 30Y.
*   **swaption_volatilities.csv:** Option Tenor x Swap Tenor matrix.
*   **cap_floor_volatilities.csv:** Tenor x Strike smile surface.

---

## ⚖️ License
MIT License
