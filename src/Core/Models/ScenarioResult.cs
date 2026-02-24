#nullable enable

namespace FixedIncomePricingLibrary.Core.Models;

/// <summary>
/// Contains the results of a scenario analysis calculation.
/// </summary>
public class ScenarioResult
{
    /// <summary>
    /// The name of the scenario.
    /// </summary>
    public string ScenarioName { get; init; } = string.Empty;

    /// <summary>
    /// The original value (before applying scenario shock).
    /// </summary>
    public double BaseValue { get; init; }

    /// <summary>
    /// The new value after applying the scenario shock.
    /// </summary>
    public double ShockedValue { get; init; }

    /// <summary>
    /// The change in value (P&L).
    /// </summary>
    public double PnL => ShockedValue - BaseValue;

    /// <summary>
    /// The percentage change in value.
    /// </summary>
    public double PercentageChange => BaseValue != 0 ? PnL / Math.Abs(BaseValue) : 0;
}
