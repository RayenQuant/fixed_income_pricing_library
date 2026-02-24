#nullable enable
using System;
using FixedIncomePricingLibrary.Analytics.DayCount;
using FixedIncomePricingLibrary.Core.Enums;

namespace FixedIncomePricingLibrary.Analytics.CashFlows;

/// <summary>
/// Calculates accrued interest for an instrument between coupon dates.
/// </summary>
public static class AccruedInterestCalculator
{
    public static double Calculate(
        DateTime lastCouponDate,
        DateTime settlementDate,
        double couponRate,
        double notional,
        DayCountConvention convention)
    {
        if (settlementDate <= lastCouponDate)
            return 0;

        var calculator = DayCountFactory.Create(convention);
        double dcf = calculator.YearFraction(lastCouponDate, settlementDate);

        return notional * couponRate * dcf;
    }
}
