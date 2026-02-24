using System;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Instruments;
using FixedIncomePricingLibrary.MarketData.YieldCurves;
using FixedIncomePricingLibrary.Pricing.Engines;
using FixedIncomePricingLibrary.Risk;
using Xunit;

namespace FixedIncomePricingLibrary.UnitTests.Risk;

public class RiskTests
{
    [Fact]
    public void BondDV01_ShouldBePositive()
    {
        var valDate = DateTime.Today;
        var bond = new Bond
        {
            Id = "BOND",
            Notional = 1_000_000,
            CouponRate = 0.05,
            Frequency = CouponFrequency.Annual,
            IssueDate = valDate,
            MaturityDate = valDate.AddYears(10),
            DayCount = DayCountConvention.Actual360
        };

        var curve = YieldCurveBuilder.FlatCurve(valDate, 0.05);
        var engine = new BondPricingEngine(curve);

        double dv01 = GreeksCalculator.CalculateDV01(engine, bond, curve);

        // Price should drop as rates rise, so DV01 = -(V_up - V_0) should be positive
        Assert.True(dv01 > 0);
    }
}
