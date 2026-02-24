#nullable enable

namespace FixedIncomePricingLibrary.Core.Enums;

/// <summary>
/// Specifies the frequency of coupon payments.
/// </summary>
public enum CouponFrequency
{
    /// <summary>
    /// One coupon payment per year.
    /// </summary>
    Annual = 1,

    /// <summary>
    /// Two coupon payments per year.
    /// </summary>
    SemiAnnual = 2,

    /// <summary>
    /// Four coupon payments per year.
    /// </summary>
    Quarterly = 4,

    /// <summary>
    /// Twelve coupon payments per year.
    /// </summary>
    Monthly = 12,

    /// <summary>
    /// No coupons (Zero coupon instrument).
    /// </summary>
    Zero = 0
}
