using System;
using FixedIncomePricingLibrary.Core.Enums;

namespace FixedIncomePricingLibrary.Core.Interfaces;

/// <summary>
/// Defines the contract for business holiday calendars and date adjustments.
/// </summary>
public interface ICalendar
{
    /// <summary>
    /// Determines if the specified date is a business day (not a weekend or holiday).
    /// </summary>
    bool IsBusinessDay(DateTime date);

    /// <summary>
    /// Adjusts the date based on the specified business day convention.
    /// </summary>
    DateTime AdjustDate(DateTime date, BusinessDayConvention convention);

    /// <summary>
    /// Adds a number of business days to a date.
    /// </summary>
    DateTime AddBusinessDays(DateTime date, int n);
}
