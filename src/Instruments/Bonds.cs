#nullable enable
using System;
using System.Collections.Generic;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Core.Interfaces;

namespace FixedIncomePricingLibrary.Instruments;

/// <summary>
/// Implementation of a fixed-rate bond.
/// </summary>
public class Bond : InstrumentBase
{
    public double CouponRate { get; set; }
    public CouponFrequency Frequency { get; set; }

    public Bond()
    {
        InstrumentType = InstrumentType.FixedRateBond;
    }
}

/// <summary>
/// Implementation of a zero-coupon bond.
/// </summary>
public class ZeroCouponBond : InstrumentBase
{
    public ZeroCouponBond()
    {
        InstrumentType = InstrumentType.ZeroCouponBond;
    }
}
