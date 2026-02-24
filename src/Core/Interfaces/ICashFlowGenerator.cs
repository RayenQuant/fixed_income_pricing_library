#nullable enable
using System.Collections.Generic;
using FixedIncomePricingLibrary.Core.Interfaces;

namespace FixedIncomePricingLibrary.Core.Interfaces;

/// <summary>
/// Defines the contract for generating cash flow schedules for instruments.
/// </summary>
public interface ICashFlowGenerator
{
    /// <summary>
    /// Generates a list of cash flows for the specified instrument.
    /// </summary>
    IEnumerable<ICashFlow> GenerateCashFlows(IInstrument instrument);
}
