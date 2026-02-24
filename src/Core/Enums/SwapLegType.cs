#nullable enable

namespace FixedIncomePricingLibrary.Core.Enums;

/// <summary>
/// Specifies the type of a swap leg.
/// </summary>
public enum SwapLegType
{
    /// <summary>
    /// The leg pays or receives a fixed interest rate.
    /// </summary>
    Fixed,

    /// <summary>
    /// The leg pays or receives a floating interest rate indexed to a benchmark.
    /// </summary>
    Floating
}
