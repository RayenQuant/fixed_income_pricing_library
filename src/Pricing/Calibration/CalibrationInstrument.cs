#nullable enable
using System;
using FixedIncomePricingLibrary.Core.Interfaces;

namespace FixedIncomePricingLibrary.Pricing.Calibration;

/// <summary>
/// Wraps an instrument with its market price for calibration purposes.
/// </summary>
public class CalibrationInstrument
{
    public IInstrument Instrument { get; }
    public double MarketPrice { get; }
    public double Weight { get; }

    public CalibrationInstrument(IInstrument instrument, double marketPrice, double weight = 1.0)
    {
        Instrument = instrument;
        MarketPrice = marketPrice;
        Weight = weight;
    }
}
