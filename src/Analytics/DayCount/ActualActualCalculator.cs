#nullable enable
using System;
using FixedIncomePricingLibrary.Core.Interfaces;

namespace FixedIncomePricingLibrary.Analytics.DayCount;

/// <summary>
/// Implements the Actual/Actual (ICMA) day count convention.
/// Note: Simple version that assumes 365/366 based on leap year. 
/// More complex ISDA versions exist but this serves the primary requirement.
/// </summary>
public class ActualActualCalculator : IDayCountCalculator
{
    public int DayCount(DateTime start, DateTime end)
    {
        return (end - start).Days;
    }

    public double YearFraction(DateTime start, DateTime end)
    {
        if (start == end) return 0.0;

        int y1 = start.Year;
        int y2 = end.Year;

        if (y1 == y2)
        {
            return DayCount(start, end) / (DateTime.IsLeapYear(y1) ? 366.0 : 365.0);
        }

        double f1 = (new DateTime(y1 + 1, 1, 1) - start).Days / (DateTime.IsLeapYear(y1) ? 366.0 : 365.0);
        double f2 = (end - new DateTime(y2, 1, 1)).Days / (DateTime.IsLeapYear(y2) ? 366.0 : 365.0);

        return f1 + f2 + (y2 - y1 - 1);
    }
}
