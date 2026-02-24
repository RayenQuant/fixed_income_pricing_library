#nullable enable

namespace FixedIncomePricingLibrary.Core.Enums;

/// <summary>
/// Specifies the day count convention used for calculating interest accrual and year fractions.
/// </summary>
public enum DayCountConvention
{
    /// <summary>
    /// Actual/360: Actual number of days / 360 days in a year.
    /// Common in money market instruments and floating legs.
    /// </summary>
    Actual360,

    /// <summary>
    /// Actual/365: Actual number of days / 365 days in a year.
    /// Common in UK conventions.
    /// </summary>
    Actual365,

    /// <summary>
    /// 30/360: Assumes 30 days in each month and 360 days in a year.
    /// Common in US corporate bonds and fixed legs.
    /// </summary>
    Thirty360,

    /// <summary>
    /// Actual/Actual (ICMA): Common with US Treasuries and government bonds.
    /// </summary>
    ActualActual
}
