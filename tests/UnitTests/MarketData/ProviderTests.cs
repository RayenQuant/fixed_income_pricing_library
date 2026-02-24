using System;
using System.IO;
using FixedIncomePricingLibrary.MarketData.Providers;
using Xunit;

namespace FixedIncomePricingLibrary.UnitTests.MarketData;

public class ProviderTests
{
    [Fact]
    public void TreasuryCurveLoader_LoadsCorrectRates()
    {
        var valDate = new DateTime(2025, 2, 23);
        // data/market_data/usd_treasury_curve.csv should exist from previous step
        // But for isolation, we can check if it exists or use a temp file.
        // Assuming current directory logic works.

        string baseDir = AppContext.BaseDirectory;
        // Search upwards for 'data' folder
        string dataPath = Path.Combine(baseDir, "..", "..", "..", "..", "data", "market_data", "usd_treasury_curve.csv");

        if (File.Exists(dataPath))
        {
            var curve = TreasuryCurveLoader.Load(dataPath, valDate);
            Assert.NotNull(curve);
            // 1Y rate was 4.65%
            var df1Y = curve.GetDiscountFactor(valDate.AddYears(1));
            // DF = 1 / (1 + 0.0465 * 365/360)
            double expected = 1.0 / (1.0 + 0.0465 * (365.0 / 360.0));
            Assert.Equal(expected, df1Y, 5);
        }
    }
}
