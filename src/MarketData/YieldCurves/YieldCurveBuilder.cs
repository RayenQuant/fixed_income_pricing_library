#nullable enable
using System;
using System.Collections.Generic;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Core.Interfaces;
using FixedIncomePricingLibrary.MarketData.Interpolation;

namespace FixedIncomePricingLibrary.MarketData.YieldCurves;

/// <summary>
/// Orchestrates the creation of Yield Curves.
/// </summary>
public static class YieldCurveBuilder
{
    public static IYieldCurve Build(
        DateTime valuationDate,
        IEnumerable<(DateTime maturity, double rate)> quotes,
        InterpolationMethod interpMethod = InterpolationMethod.LogLinear,
        DayCountConvention dayCount = DayCountConvention.Actual360)
    {
        var engine = new BootstrapEngine();
        var (times, dfs) = engine.Bootstrap(valuationDate, quotes, dayCount);

        var interpolator = InterpolatorFactory.Create(interpMethod);
        return new YieldCurve(valuationDate, times, dfs, interpolator);
    }

    public static IYieldCurve FlatCurve(DateTime valuationDate, double rate)
    {
        // A flat curve has zero rates constant at 'rate'
        // r = -ln(DF) / T => DF = exp(-r * T)
        var times = new[] { 0.0, 50.0 };
        var dfs = new[] { 1.0, Math.Exp(-rate * 50.0) };
        return new YieldCurve(valuationDate, times, dfs, new LogLinearInterpolator());
    }
}
