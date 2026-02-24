#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using FixedIncomePricingLibrary.Core.Interfaces;
using FixedIncomePricingLibrary.Core.Models;

namespace FixedIncomePricingLibrary.Pricing.Engines;

/// <summary>
/// Basic pricing engine that sums discounted cash flows.
/// </summary>
public class DiscountingEngine : IPricingEngine
{
    private readonly IYieldCurve _curve;

    public DiscountingEngine(IYieldCurve curve)
    {
        _curve = curve;
    }

    public virtual PricingResult Price(IInstrument instrument)
    {
        // This is a generic engine, specific logic should be in derived engines 
        // or handled via cash flow projections.
        throw new NotImplementedException("Generic DiscountingEngine requires cash flows. Use concrete engines like BondPricingEngine.");
    }

    protected double DiscountCashFlows(IEnumerable<ICashFlow> cashFlows, DateTime settlementDate)
    {
        double presentValue = 0;
        foreach (var cf in cashFlows)
        {
            if (cf.PaymentDate > settlementDate)
            {
                double df = _curve.GetDiscountFactor(cf.PaymentDate);
                presentValue += cf.Amount * df;
            }
        }
        return presentValue;
    }
}
