using System;
using System.Collections.Generic;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.MarketData.YieldCurves;
using Xunit;

namespace FixedIncomePricingLibrary.UnitTests.MarketData;

public class YieldCurveTests
{
    [Fact]
    public void FlatCurve_ReturnsExpectedDiscountFactors()
    {
        var valuationDate = new DateTime(2025, 1, 1);
        double rate = 0.05;
        var curve = YieldCurveBuilder.FlatCurve(valuationDate, rate);

        var df1Y = curve.GetDiscountFactor(valuationDate.AddYears(1));
        // DF = exp(-0.05 * 1)
        Assert.Equal(Math.Exp(-0.05), df1Y, 4);

        Assert.Equal(rate, curve.GetZeroRate(valuationDate.AddYears(2)), 4);
    }

    [Fact]
    public void Bootstrap_SimpleDeposit_IsCorrect()
    {
        var valuationDate = new DateTime(2025, 1, 1);
        var quotes = new List<(DateTime, double)>
        {
            (valuationDate.AddMonths(6), 0.04),
            (valuationDate.AddYears(1), 0.045)
        };

        var curve = YieldCurveBuilder.Build(valuationDate, quotes, InterpolationMethod.LogLinear, DayCountConvention.Actual360);

        // For 1Y: DF = 1 / (1 + rate * dcf)
        // Actual/360 for 365 days = 365/360
        double expectedDf = 1.0 / (1.0 + 0.045 * (365.0 / 360.0));
        var df1Y = curve.GetDiscountFactor(valuationDate.AddYears(1));

        Assert.Equal(expectedDf, df1Y, 5);
    }
}
