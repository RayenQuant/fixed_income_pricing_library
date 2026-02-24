#nullable enable
using System;
using System.Collections.Generic;
using FixedIncomePricingLibrary.Core.Models;

namespace FixedIncomePricingLibrary.Analytics.CashFlows;

public class CashFlowReport
{
    public string Description { get; set; } = string.Empty;
    public List<CashFlow> CashFlows { get; set; } = new();
}
