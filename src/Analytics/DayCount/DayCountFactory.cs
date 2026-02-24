#nullable enable
using System;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Core.Interfaces;

namespace FixedIncomePricingLibrary.Analytics.DayCount;

/// <summary>
/// Factory for retrieving day count calculators based on the convention.
/// </summary>
public static class DayCountFactory
{
    public static IDayCountCalculator Create(DayCountConvention convention)
    {
        return convention switch
        {
            DayCountConvention.Actual360 => new Actual360Calculator(),
            DayCountConvention.Actual365 => new Actual365Calculator(),
            DayCountConvention.Thirty360 => new Thirty360Calculator(),
            DayCountConvention.ActualActual => new ActualActualCalculator(),
            _ => throw new ArgumentOutOfRangeException(nameof(convention), convention, null)
        };
    }
}
