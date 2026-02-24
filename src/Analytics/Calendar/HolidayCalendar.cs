#nullable enable
using System;
using System.Collections.Generic;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Core.Interfaces;

namespace FixedIncomePricingLibrary.Analytics.Calendar;

/// <summary>
/// Implements a business day calendar with holiday awareness.
/// </summary>
public class HolidayCalendar : ICalendar
{
    private readonly HashSet<DateTime> _holidays;

    public HolidayCalendar(IEnumerable<DateTime> holidays)
    {
        _holidays = new HashSet<DateTime>(holidays);
    }

    public bool IsBusinessDay(DateTime date)
    {
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            return false;

        return !_holidays.Contains(date.Date);
    }

    public DateTime AdjustDate(DateTime date, BusinessDayConvention convention)
    {
        if (IsBusinessDay(date))
            return date;

        return convention switch
        {
            BusinessDayConvention.Following => GetNextBusinessDay(date),
            BusinessDayConvention.ModifiedFollowing => GetModifiedFollowing(date),
            BusinessDayConvention.Preceding => GetPreviousBusinessDay(date),
            _ => throw new ArgumentOutOfRangeException(nameof(convention), convention, null)
        };
    }

    public DateTime AddBusinessDays(DateTime date, int n)
    {
        DateTime result = date;
        int count = 0;
        int step = n > 0 ? 1 : -1;
        int target = Math.Abs(n);

        while (count < target)
        {
            result = result.AddDays(step);
            if (IsBusinessDay(result))
            {
                count++;
            }
        }

        return result;
    }

    private DateTime GetNextBusinessDay(DateTime date)
    {
        DateTime result = date.AddDays(1);
        while (!IsBusinessDay(result))
        {
            result = result.AddDays(1);
        }
        return result;
    }

    private DateTime GetPreviousBusinessDay(DateTime date)
    {
        DateTime result = date.AddDays(-1);
        while (!IsBusinessDay(result))
        {
            result = result.AddDays(-1);
        }
        return result;
    }

    private DateTime GetModifiedFollowing(DateTime date)
    {
        DateTime next = GetNextBusinessDay(date);
        if (next.Month != date.Month)
        {
            return GetPreviousBusinessDay(date);
        }
        return next;
    }
}
