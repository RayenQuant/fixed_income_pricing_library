#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using FixedIncomePricingLibrary.Core.Interfaces;
using FixedIncomePricingLibrary.MarketData.Interpolation;

namespace FixedIncomePricingLibrary.MarketData.YieldCurves;

/// <summary>
/// Implementation of a Yield Curve that stores discount factors and provides rate lookups.
/// </summary>
public class YieldCurve : IYieldCurve
{
    private readonly double[] _times;
    private readonly double[] _discountFactors;
    private readonly IInterpolator _interpolator;

    public DateTime ValuationDate { get; }

    public YieldCurve(DateTime valuationDate, double[] times, double[] discountFactors, IInterpolator interpolator)
    {
        ValuationDate = valuationDate;
        _times = times;
        _discountFactors = discountFactors;
        _interpolator = interpolator;
        _interpolator.Initialize(_times, _discountFactors);
    }

    public double GetDiscountFactor(DateTime date)
    {
        if (date <= ValuationDate) return 1.0;

        double t = (date - ValuationDate).TotalDays / 365.0;
        return _interpolator.Interpolate(t);
    }

    public double GetZeroRate(DateTime maturity)
    {
        if (maturity <= ValuationDate) return 0.0;

        double t = (maturity - ValuationDate).TotalDays / 365.0;
        double df = GetDiscountFactor(maturity);

        // Continuous compounding: DF = exp(-r * T) => r = -ln(DF) / T
        return -Math.Log(df) / t;
    }

    public double GetForwardRate(DateTime start, DateTime end)
    {
        if (start >= end) return 0.0;

        double dfStart = GetDiscountFactor(start);
        double dfEnd = GetDiscountFactor(end);
        double t = (end - start).TotalDays / 365.0;

        // f = -ln(dfEnd / dfStart) / t
        return -Math.Log(dfEnd / dfStart) / t;
    }

    public double GetParSwapRate(string tenor)
    {
        // Simplistic implementation for bootstrapping purposes
        // Real implementation would use the underlying swap frequency
        throw new NotImplementedException("Par swap rate calculation requires cash flow frequency details.");
    }
}
