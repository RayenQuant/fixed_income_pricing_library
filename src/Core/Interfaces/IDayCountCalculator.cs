#nullable enable
using System;

namespace FixedIncomePricingLibrary.Core.Interfaces;

/// <summary>
/// Defines the contract for calculating day counts and year fractions.
/// </summary>
public interface IDayCountCalculator
{
    /// <summary>
    /// Calculates the day count between two dates.
    /// </summary>
    int DayCount(DateTime start, DateTime end);

    /// <summary>
    /// Calculates the year fraction between two dates based on the convention.
    /// </summary>
    double YearFraction(DateTime start, DateTime end);
}
