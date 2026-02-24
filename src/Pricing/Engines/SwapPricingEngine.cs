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
/// Pricing engine for Vanilla Interest Rate Swaps.
/// </summary>
public class SwapPricingEngine : DiscountingEngine
{
    private readonly IYieldCurve _forecastCurve;

    public SwapPricingEngine(IYieldCurve discountCurve, IYieldCurve? forecastCurve = null)
        : base(discountCurve)
    {
        _forecastCurve = forecastCurve ?? discountCurve;
    }

    public override PricingResult Price(IInstrument instrument)
    {
        if (instrument is not InterestRateSwap swap)
            throw new ArgumentException("Instrument must be a Swap.");

        var fixedGenerator = new FixedLegCashFlowGenerator();
        var fixedCfs = fixedGenerator.Generate(
            DateTime.Today, swap.MaturityDate, swap.FixedRate, swap.FixedFrequency,
            swap.Notional, swap.Currency, swap.FixedDayCount, new HolidayCalendar(new List<DateTime>()), BusinessDayConvention.Following);

        var floatGenerator = new FloatingLegCashFlowGenerator();
        var floatCfs = floatGenerator.Generate(
            DateTime.Today, swap.MaturityDate, swap.FloatFrequency, swap.Spread,
            swap.Notional, swap.Currency, swap.FloatDayCount, new HolidayCalendar(new List<DateTime>()), BusinessDayConvention.Following, _forecastCurve);

        double fixedPv = DiscountCashFlows(fixedCfs.Cast<ICashFlow>(), DateTime.Today);
        double floatPv = DiscountCashFlows(floatCfs.Cast<ICashFlow>(), DateTime.Today);

        // PV = PV(Float) - PV(Fixed) for receiver, PV(Fixed) - PV(Float) for payer?
        // Industry convention: NPV = PV(Receive Leg) - PV(Pay Leg)
        double npv = swap.PayFixed ? (floatPv - fixedPv) : (fixedPv - floatPv);

        return new PricingResult(swap)
        {
            DirtyPrice = npv,
            CleanPrice = npv, // Swaps don't traditionally have "clean price" in the bond sense
            Timestamp = DateTime.UtcNow
        };
    }
}
