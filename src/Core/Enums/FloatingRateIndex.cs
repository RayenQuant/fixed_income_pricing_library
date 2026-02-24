#nullable enable

namespace FixedIncomePricingLibrary.Core.Enums;

/// <summary>
/// Specifies the reference interest rate index.
/// </summary>
public enum FloatingRateIndex
{
    /// <summary>
    /// Secured Overnight Financing Rate.
    /// </summary>
    SOFR,

    /// <summary>
    /// 1-Month London Interbank Offered Rate.
    /// </summary>
    Libor1M,

    /// <summary>
    /// 3-Month London Interbank Offered Rate.
    /// </summary>
    Libor3M,

    /// <summary>
    /// 6-Month London Interbank Offered Rate.
    /// </summary>
    Libor6M,

    /// <summary>
    /// 12-Month London Interbank Offered Rate.
    /// </summary>
    Libor12M,

    /// <summary>
    /// Overnight Indexed Swap rate.
    /// </summary>
    OIS
}
