#nullable enable
using System;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Core.Interfaces;

namespace FixedIncomePricingLibrary.Instruments;

/// <summary>
/// Implementation of an Interest Rate Swaption.
/// </summary>
public class Swaption : InstrumentBase
{
    public InterestRateSwap UnderlyingSwap { get; set; } = new();
    public DateTime ExpiryDate { get; set; }
    public SettlementType SettlementType { get; set; }
    public OptionType OptionType { get; set; }

    public Swaption()
    {
        InstrumentType = InstrumentType.Swaption;
    }
}
