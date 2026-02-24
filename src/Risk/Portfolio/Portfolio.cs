#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using FixedIncomePricingLibrary.Core.Interfaces;
using FixedIncomePricingLibrary.Core.Models;

namespace FixedIncomePricingLibrary.Risk;

/// <summary>
/// Represents a position in a specific instrument.
/// </summary>
public class Position
{
    public IInstrument Instrument { get; set; } = null!;
    public double Quantity { get; set; } = 1.0;
}

/// <summary>
/// Represents a collection of positions for risk aggregation.
/// </summary>
public class Portfolio
{
    public string Name { get; set; } = string.Empty;
    public List<Position> Positions { get; set; } = new();

    public double CalculateTotalNpv(Func<IInstrument, double> pricer)
    {
        return Positions.Sum(p => pricer(p.Instrument) * p.Quantity);
    }
}
