#nullable enable

namespace FixedIncomePricingLibrary.Core.Enums;

/// <summary>
/// Specifies the settlement type for an option or trade.
/// </summary>
public enum SettlementType
{
    /// <summary>
    /// Physical delivery of the underlying instrument upon exercise.
    /// </summary>
    Physical,

    /// <summary>
    /// Cash settlement based on the difference between market price and strike.
    /// </summary>
    Cash
}
