#nullable enable
using System;
using FixedIncomePricingLibrary.Core.Interfaces;

namespace FixedIncomePricingLibrary.Analytics.DayCount;

/// <summary>
/// Implements the Actual/360 day count convention.
/// Year Fraction = Actual days / 360.
/// </summary>
public class Actual360Calculator : IDayCountCalculator
{
    public int DayCount(DateTime start, DateTime end)
    {
        return (end - start).Days;
    }

    public double YearFraction(DateTime start, DateTime end)
    {
        return DayCount(start, end) / 360.0;
    }
}
