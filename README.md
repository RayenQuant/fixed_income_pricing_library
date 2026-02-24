# Fixed Income Pricing Library (.NET 9)

A professional-grade C# SDK for Fixed Income Derivatives pricing, risk management, and market data analytics. Designed for integration into trading platforms, risk systems, or automated valuation pipelines.

---

## Architecture & Modules

The library is strictly modular, allowing you to use individual components (like Day Counters) or the full Pricing Pipeline.

### 1. `Core`
*   **Interfaces:** `IInstrument`, `IYieldCurve`, `IPricingEngine`, `IVolatilitySurface`.
*   **Enums:** Industry-standard conventions (`DayCountConvention`, `BusinessDayConvention`, `CouponFrequency`).

### 2. `MarketData`
*   **Yield Curves:** High-performance bootstrapping with Log-Linear, Linear, or Cubic Spline interpolation. 
*   **Volatility:** Support for 3D Swaption Cubes and 2D Cap/Floor Smile Surfaces.
*   **Providers:** Built-in CSV loaders for Treasury Curves and SOFR fixings.

### 3. `Instruments`
*   **Cash Products:** Fixed-Rate Bonds, Zero-Coupon Bonds, Floating-Rate Notes.
*   **Linear Derivatives:** Vanilla Interest Rate Swaps (IRS), Basis Swaps.
*   **Options:** Swaptions (Payer/Receiver), Caps, Floors, Collars.

### 4. `Pricing`
*   **Analytical:** Discounting Engine (Bonds/Swaps), Black-76 (European Options).
*   **Numerical:** Hull-White One-Factor model via **Trinomial Tree** (Backward Induction) or **Monte Carlo** simulation.
*   **Calibration:** Automated Hull-White parameter optimization (Mean Reversion $\alpha$, Volatility $\sigma$) using market quotes.

### 5. `Risk`
*   **Greeks:** DV01, Modified Duration, Convexity.
*   **Analytics:** Scenario Analysis (parallel shifts), Portfolio NPV aggregation.
*   **VaR:** 1-day Value-at-Risk using Monte Carlo simulation & Historical Simulation.

---

##  Developer Guide: Usage Examples

### 1. Pricing a Bond
```csharp
using FixedIncomePricingLibrary.Instruments;
using FixedIncomePricingLibrary.Pricing.Engines;
using FixedIncomePricingLibrary.MarketData.YieldCurves;

// Load a curve
var curve = YieldCurveBuilder.FlatCurve(DateTime.Today, 0.045);

// Create instrument
var bond = new Bond {
    Id = "UST-10Y",
    Notional = 1_000_000,
    CouponRate = 0.0425,
    MaturityDate = DateTime.Today.AddYears(10),
    Frequency = CouponFrequency.SemiAnnual,
    DayCount = DayCountConvention.Thirty360
};

// Price
var engine = new BondPricingEngine(curve);
var result = engine.Price(bond);

Console.WriteLine($"Dirty Price: {result.DirtyPrice}");
```

### 2. Pricing a Swaption with Black-76
```csharp
using FixedIncomePricingLibrary.Pricing.Engines;
using FixedIncomePricingLibrary.MarketData.Volatility;

// Setup market data
var volSurface = new SwaptionVolatilityCube(0.40); // 40% normal vol
var engine = new BlackEngine(curve, volSurface);

var swaption = new Swaption {
    Notional = 10_000_000,
    ExpiryDate = DateTime.Today.AddYears(1),
    OptionType = OptionType.Payer,
    UnderlyingSwap = mySwap
};

var price = engine.Price(swaption).DirtyPrice;
```

### 3. Calculating Portfolio Risk (DV01)
```csharp
using FixedIncomePricingLibrary.Risk;

// Automatic engine factor for Greeks
Func<IYieldCurve, IPricingEngine> factory = (c) => new BondPricingEngine(c);

double dv01 = GreeksCalculator.CalculateDV01(factory, bond, curve);
Console.WriteLine($"DV01: {dv01}"); // Sensitivity to 1bp move
```

---

## 🧪 Testing & Validation
The library includes a comprehensive test suite (32+ unit tests) covering:
*   **Mathematical correctness:** Comparison against analytical closed-form solutions.
*   **Conventions:** Day count accuracy (Actual/360, 30/360, etc.) and holiday adjustment logic.
*   **Bootstrapping:** Convergence of the yield curve builder.

Run tests via CLI:
```powershell
dotnet test
```

---

## Sample Data / Demo
While the library is an SDK, a reference implementation is provided in `samples/ConsoleApp`. It demonstrates:
*   How to load data from CSVs.
*   How to orchestrate multiple pricing engines.
*   How to run a high-performance Monte Carlo VaR on a multi-asset portfolio.

---

## 🛠️ Installation
1. Clone the repository.
2. Reference `FixedIncomePricingLibrary.csproj` in your solution.
3. Install Nuget dependencies: `MathNet.Numerics`, `CsvHelper`.

---

## ⚖️ License
MIT License
