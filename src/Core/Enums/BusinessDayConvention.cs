#nullable enable

namespace FixedIncomePricingLibrary.Core.Enums;

/// <summary>
/// Specifies the convention for adjusting payment dates that fall on non-business days.
/// </summary>
public enum BusinessDayConvention
{
    /// <summary>
    /// The payment date is adjusted to the first following business day.
    /// </summary>
    Following,

    /// <summary>
    /// The payment date is adjusted to the first following business day unless that day crosses into the next calendar month, 
    /// in which case the date is adjusted to the first preceding business day.
    /// </summary>
    ModifiedFollowing,

    /// <summary>
    /// The payment date is adjusted to the first preceding business day.
    /// </summary>
    Preceding
}
