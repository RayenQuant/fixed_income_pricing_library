#nullable enable

namespace FixedIncomePricingLibrary.Core.Enums;

/// <summary>
/// Specifies the type of an interest rate option or swaption.
/// </summary>
public enum OptionType
{
    /// <summary>
    /// Right to pay fixed (payer swaption).
    /// </summary>
    Payer,

    /// <summary>
    /// Right to receive fixed (receiver swaption).
    /// </summary>
    Receiver,

    /// <summary>
    /// Interest rate cap.
    /// </summary>
    Cap,

    /// <summary>
    /// Interest rate floor.
    /// </summary>
    Floor,

    /// <summary>
    /// Interest rate collar (Cap + Floor).
    /// </summary>
    Collar
}
