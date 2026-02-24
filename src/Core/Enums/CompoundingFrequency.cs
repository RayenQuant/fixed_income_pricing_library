#nullable enable

namespace FixedIncomePricingLibrary.Core.Enums;

/// <summary>
/// Specifies the frequency of compounding for interest rates.
/// </summary>
public enum CompoundingFrequency
{
    /// <summary>
    /// Interest is compounded once per year.
    /// </summary>
    Annual,

    /// <summary>
    /// Interest is compounded twice per year.
    /// </summary>
    SemiAnnual,

    /// <summary>
    /// Interest is compounded four times per year.
    /// </summary>
    Quarterly,

    /// <summary>
    /// Interest is compounded twelve times per year.
    /// </summary>
    Monthly,

    /// <summary>
    /// Interest is compounded continuously.
    /// </summary>
    Continuous
}
