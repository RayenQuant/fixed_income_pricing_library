#nullable enable
using System.IO;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Core.Interfaces;
using FixedIncomePricingLibrary.Core.Models;
using FixedIncomePricingLibrary.MarketData.Volatility;

namespace FixedIncomePricingLibrary.MarketData.Providers;

public class CsvMarketDataProvider : IMarketDataProvider
{
    private readonly string _dataDir;

    public CsvMarketDataProvider(string dataDir)
    {
        _dataDir = dataDir;
    }

    public IYieldCurve GetYieldCurve(Currency currency, DateTime date)
    {
        string curveName = currency switch
        {
            Currency.USD => "usd_treasury_curve",
            _ => throw new ArgumentException($"No curve mapping for currency {currency}")
        };
        string path = Path.Combine(_dataDir, "market_data", $"{curveName}.csv");
        return TreasuryCurveLoader.Load(path, date);
    }

    public IVolatilitySurface GetVolSurface(string type, DateTime date)
    {
        // Simple fallback
        return new SwaptionVolatilityCube(0.40);
    }

    public double GetFixingRate(FloatingRateIndex index, DateTime date)
    {
        return 0.045; // Placeholder
    }
}
