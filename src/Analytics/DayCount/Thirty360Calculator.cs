#nullable enable
using System;
using FixedIncomePricingLibrary.Core.Interfaces;

namespace FixedIncomePricingLibrary.Analytics.DayCount;

/// <summary>
/// Implements the 30/360 (US Corporate) day count convention.
/// </summary>
public class Thirty360Calculator : IDayCountCalculator
{
    public int DayCount(DateTime start, DateTime end)
    {
        int d1 = Math.Min(30, start.Day);
        int d2 = Math.Min(30, end.Day);

        if (d1 == 30 && end.Day == 31)
        {
            d2 = 30;
        }

        return 360 * (end.Year - start.Year) + 30 * (end.Month - start.Month) + (d2 - d1);
    }

    public double YearFraction(DateTime start, DateTime end)
    {
        return DayCount(start, end) / 360.0;
    }
}
