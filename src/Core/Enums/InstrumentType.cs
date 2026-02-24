#nullable enable

namespace FixedIncomePricingLibrary.Core.Enums;

/// <summary>
/// Specifies the type of fixed income instrument.
/// </summary>
public enum InstrumentType
{
    ZeroCouponBond,
    FixedRateBond,
    FloatingRateBond,
    CallableBond,
    VanillaSwap,
    BasisSwap,
    AmortizingSwap,
    Swaption,
    Cap,
    Floor,
    Collar
}
