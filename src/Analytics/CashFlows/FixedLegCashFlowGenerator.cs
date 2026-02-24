#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using FixedIncomePricingLibrary.Analytics.DayCount;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Core.Interfaces;
using FixedIncomePricingLibrary.Core.Models;

namespace FixedIncomePricingLibrary.Analytics.CashFlows;

/// <summary>
/// Generates cash flows for fixed-rate instruments.
/// </summary>
public class FixedLegCashFlowGenerator
{
    public IEnumerable<CashFlow> Generate(
        DateTime startDate,
        DateTime endDate,
        double couponRate,
        CouponFrequency frequency,
        double notional,
        Currency currency,
        DayCountConvention dayCount,
        ICalendar calendar,
        BusinessDayConvention convention)
    {
        var schedule = ScheduleGenerator.Generate(startDate, endDate, frequency, calendar, convention);
        var calculator = DayCountFactory.Create(dayCount);
        var cashFlows = new List<CashFlow>();

        DateTime prevDate = startDate;
        foreach (var payDate in schedule)
        {
            double dcf = calculator.YearFraction(prevDate, payDate);
            double amount = notional * couponRate * dcf;

            // If it's the last payment, add principal
            if (payDate.Date == schedule[^1].Date)
            {
                amount += notional;
            }

            cashFlows.Add(new CashFlow(payDate, amount, currency)
            {
                Notional = notional,
                DayCountFraction = dcf
            });

            prevDate = payDate;
        }

        return cashFlows;
    }
}
