#nullable enable
using System;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Core.Exceptions;
using FixedIncomePricingLibrary.Core.Interfaces;

namespace FixedIncomePricingLibrary.Instruments;

public static class InstrumentFactory
{
    public static IInstrument Create(InstrumentType type)
    {
        return type switch
        {
            InstrumentType.FixedRateBond => new Bond(),
            InstrumentType.ZeroCouponBond => new ZeroCouponBond(),
            InstrumentType.VanillaSwap => new InterestRateSwap(),
            InstrumentType.Swaption => new Swaption(),
            InstrumentType.Cap => new CapFloor { Type = OptionType.Cap, InstrumentType = InstrumentType.Cap },
            InstrumentType.Floor => new CapFloor { Type = OptionType.Floor, InstrumentType = InstrumentType.Floor },
            _ => throw new ValidationException($"Unsupported instrument type: {type}")
        };
    }
}
