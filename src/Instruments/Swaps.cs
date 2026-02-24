#nullable enable
using System;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Core.Interfaces;

namespace FixedIncomePricingLibrary.Instruments;

/// <summary>
/// Implementation of a Vanilla Interest Rate Swap.
/// </summary>
public class InterestRateSwap : InstrumentBase
{
    // Fixed Leg
    public double FixedRate { get; set; }
    public CouponFrequency FixedFrequency { get; set; }
    public DayCountConvention FixedDayCount { get; set; }

    // Floating Leg
    public FloatingRateIndex FloatIndex { get; set; }
    public CouponFrequency FloatFrequency { get; set; }
    public DayCountConvention FloatDayCount { get; set; }
    public double Spread { get; set; }

    public bool PayFixed { get; set; }

    public InterestRateSwap()
    {
        InstrumentType = InstrumentType.VanillaSwap;
    }
}
