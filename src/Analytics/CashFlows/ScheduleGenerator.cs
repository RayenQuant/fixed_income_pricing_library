#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Core.Interfaces;

namespace FixedIncomePricingLibrary.Analytics.CashFlows;

/// <summary>
/// Generates sequences of dates for payment schedules.
/// </summary>
public static class ScheduleGenerator
{
    public static List<DateTime> Generate(
        DateTime start,
        DateTime end,
        CouponFrequency frequency,
        ICalendar calendar,
        BusinessDayConvention convention)
    {
        var dates = new List<DateTime>();
        if (frequency == CouponFrequency.Zero)
        {
            dates.Add(calendar.AdjustDate(end, convention));
            return dates;
        }

        int monthsBetween = 12 / (int)frequency;
        DateTime current = end;

        // Generate backwards from end to preserve maturity date
        while (current > start)
        {
            dates.Add(calendar.AdjustDate(current, convention));
            current = current.AddMonths(-monthsBetween);
        }

        dates.Reverse();
        return dates;
    }
}
