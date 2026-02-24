#nullable enable
using System;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Core.Interfaces;

namespace FixedIncomePricingLibrary.Instruments;

/// <summary>
/// Implementation of an Interest Rate Cap, Floor, or Collar.
/// </summary>
public class CapFloor : InstrumentBase
{
    public double Strike { get; set; }
    public FloatingRateIndex Index { get; set; }
    public CouponFrequency Frequency { get; set; }
    public OptionType Type { get; set; } // Cap, Floor, or Collar

    public CapFloor()
    {
        InstrumentType = InstrumentType.Cap; // Updated later if floor
    }
}
