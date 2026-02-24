#nullable enable
using System;
using System.Collections.Generic;
using FixedIncomePricingLibrary.Analytics.DayCount;
using FixedIncomePricingLibrary.Core.Enums; // This line is already present, but the instruction asks to add it.
using FixedIncomePricingLibrary.Core.Interfaces;
using FixedIncomePricingLibrary.Core.Models;

namespace FixedIncomePricingLibrary.Analytics.CashFlows;

/// <summary>
/// Generates projected cash flows for floating-rate instruments.
/// </summary>
public class FloatingLegCashFlowGenerator
{
    public IEnumerable<CashFlow> Generate(
        DateTime startDate,
        DateTime endDate,
        CouponFrequency frequency,
        double spread,
        double notional,
        Currency currency,
        DayCountConvention dayCount,
        ICalendar calendar,
        BusinessDayConvention convention,
        IYieldCurve curve)
    {
        var schedule = ScheduleGenerator.Generate(startDate, endDate, frequency, calendar, convention);
        var calculator = DayCountFactory.Create(dayCount);
        var cashFlows = new List<CashFlow>();

        DateTime prevDate = startDate;
        foreach (var payDate in schedule)
        {
            double dcf = calculator.YearFraction(prevDate, payDate);

            // Project forward rate from curve
            double forwardRate = curve.GetForwardRate(prevDate, payDate);
            double amount = notional * (forwardRate + spread) * dcf;

            // principal at the end
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
