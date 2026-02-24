#nullable enable
using System;
using System.Collections.Generic;
using FixedIncomePricingLibrary.Core.Models;

namespace FixedIncomePricingLibrary.Risk;

public class RiskReport
{
    public string PortfolioName { get; set; } = string.Empty;
    public RiskResult PortfolioGreeks { get; set; } = new();
    public VaRResult? VaR { get; set; }
    public List<ScenarioResult> ScenarioResults { get; set; } = new();
}
