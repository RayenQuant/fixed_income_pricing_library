#nullable enable
using System;
using System.Collections.Generic;

namespace FixedIncomePricingLibrary.Risk;

public class PortfolioReport
{
    public string PortfolioName { get; set; } = string.Empty;
    public double TotalNpv { get; set; }
    public List<PositionDetail> Items { get; set; } = new();
}

public class PositionDetail
{
    public string InstrumentId { get; set; } = string.Empty;
    public string InstrumentType { get; set; } = string.Empty;
    public double Quantity { get; set; }
    public double Npv { get; set; }
}
