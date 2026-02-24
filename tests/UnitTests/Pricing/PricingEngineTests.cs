using System;
using System.Collections.Generic;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Instruments;
using FixedIncomePricingLibrary.MarketData.YieldCurves;
using FixedIncomePricingLibrary.Pricing.Engines;
using Xunit;

namespace FixedIncomePricingLibrary.UnitTests.Pricing;

public class PricingEngineTests
{
    [Fact]
    public void ZeroCouponBond_PricedAtItsYield_ShouldReturnPar()
    {
        var valuationDate = DateTime.Today;
        var maturity = valuationDate.AddYears(1);
        var yield = 0.05;

        var bond = new Bond
        {
            Id = "ZCB",
            Notional = 1_000_000,
            CouponRate = 0,
            Frequency = CouponFrequency.Zero,
            IssueDate = valuationDate,
            MaturityDate = maturity,
            DayCount = DayCountConvention.Actual360
        };

        var curve = YieldCurveBuilder.FlatCurve(valuationDate, yield);
        var engine = new BondPricingEngine(curve);

        var result = engine.Price(bond);

        Assert.Equal(1_000_000 * Math.Exp(-0.05), result.DirtyPrice, 0);
    }

    [Fact]
    public void InterestRateSwap_AtPar_ShouldHaveZeroNpv()
    {
        var valDate = DateTime.Today;
        var maturity = valDate.AddYears(5);
        var rate = 0.04;

        var swap = new InterestRateSwap
        {
            Id = "SWAP",
            Notional = 10_000_000,
            FixedRate = 0.04,
            FixedFrequency = CouponFrequency.Annual,
            FixedDayCount = DayCountConvention.Thirty360,
            FloatFrequency = CouponFrequency.Annual,
            FloatDayCount = DayCountConvention.Thirty360,
            PayFixed = true,
            MaturityDate = maturity
        };

        var curve = YieldCurveBuilder.FlatCurve(valDate, rate);
        var engine = new SwapPricingEngine(curve);

        var result = engine.Price(swap);

        Assert.Equal(0, result.DirtyPrice, 0);
    }
}
