#nullable enable
using System;
using FixedIncomePricingLibrary.Core.Interfaces;

namespace FixedIncomePricingLibrary.Analytics.DayCount;

/// <summary>
/// Implements the Actual/365 day count convention.
/// Year Fraction = Actual days / 365.
/// </summary>
public class Actual365Calculator : IDayCountCalculator
{
    public int DayCount(DateTime start, DateTime end)
    {
        return (end - start).Days;
    }

    public double YearFraction(DateTime start, DateTime end)
    {
        return DayCount(start, end) / 365.0;
    }
}
