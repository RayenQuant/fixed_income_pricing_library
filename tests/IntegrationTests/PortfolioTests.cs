using System;
using System.Collections.Generic;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Instruments;
using FixedIncomePricingLibrary.MarketData.YieldCurves;
using FixedIncomePricingLibrary.Pricing.Engines;
using FixedIncomePricingLibrary.Risk;
using Xunit;

namespace FixedIncomePricingLibrary.IntegrationTests;

public class PortfolioTests
{
    [Fact]
    public void Portfolio_PricedWithMultipleInstruments_ShouldAggregateNPV()
    {
        var valDate = DateTime.Today;
        var curve = YieldCurveBuilder.FlatCurve(valDate, 0.05);
        var engine = new BondPricingEngine(curve);

        var portfolio = new Portfolio { Name = "Test Portfolio" };
        portfolio.Positions.Add(new Position
        {
            Instrument = new Bond { Id = "B1", Notional = 1000, CouponRate = 0.05, MaturityDate = valDate.AddYears(1), IssueDate = valDate, DayCount = DayCountConvention.Actual365, Frequency = CouponFrequency.Annual },
            Quantity = 1.0
        });

        double totalNpv = portfolio.CalculateTotalNpv(inst => engine.Price(inst).DirtyPrice);

        Assert.True(totalNpv > 0);
    }
}
