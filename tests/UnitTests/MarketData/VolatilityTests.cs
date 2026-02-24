using System;
using FixedIncomePricingLibrary.MarketData.Volatility;
using Xunit;

namespace FixedIncomePricingLibrary.UnitTests.MarketData;

public class VolatilityTests
{
    [Fact]
    public void SwaptionVolatilityCube_ReturnsFlatVol()
    {
        var cube = new SwaptionVolatilityCube(0.45);
        var vol = cube.GetVolatility(DateTime.Today, "5Y", 0.05);
        Assert.Equal(0.45, vol);
    }
}
