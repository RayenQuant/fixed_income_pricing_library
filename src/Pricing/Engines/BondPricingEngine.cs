using System;
using System.Collections.Generic;
using System.Linq;
using FixedIncomePricingLibrary.Analytics.Calendar;
using FixedIncomePricingLibrary.Analytics.CashFlows;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Core.Interfaces;
using FixedIncomePricingLibrary.Core.Models;
using FixedIncomePricingLibrary.Instruments;

namespace FixedIncomePricingLibrary.Pricing.Engines;

/// <summary>
/// Pricing engine specific to Fixed Rate Bonds.
/// </summary>
public class BondPricingEngine : DiscountingEngine
{
    public BondPricingEngine(IYieldCurve curve) : base(curve) { }

    public override PricingResult Price(IInstrument instrument)
    {
        if (instrument is not Bond bond)
            throw new ArgumentException("Instrument must be a Bond.");

        var generator = new FixedLegCashFlowGenerator();
        var cashFlows = generator.Generate(
            bond.IssueDate, bond.MaturityDate, bond.CouponRate, bond.Frequency,
            bond.Notional, bond.Currency, bond.DayCount, new HolidayCalendar(new List<DateTime>()), BusinessDayConvention.Following);

        double dirtyPrice = DiscountCashFlows(cashFlows.Cast<ICashFlow>(), DateTime.Today);

        // Accrued Interest (assuming settlement = today)
        double accrued = 0;

        return new PricingResult(bond)
        {
            DirtyPrice = dirtyPrice,
            CleanPrice = dirtyPrice - accrued,
            AccruedInterest = accrued,
            Timestamp = DateTime.UtcNow
        };
    }
}
