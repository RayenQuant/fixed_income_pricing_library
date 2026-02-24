#nullable enable
using FixedIncomePricingLibrary.Core.Models;

namespace FixedIncomePricingLibrary.Core.Interfaces;

/// <summary>
/// Defines the contract for a pricing engine capable of valuing instruments.
/// </summary>
public interface IPricingEngine
{
    /// <summary>
    /// Prices the specified instrument and returns the results.
    /// </summary>
    PricingResult Price(IInstrument instrument);
}
