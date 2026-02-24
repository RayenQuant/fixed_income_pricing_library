#nullable enable
using System;
using System.Collections.Generic;
using FixedIncomePricingLibrary.Core.Interfaces;

namespace FixedIncomePricingLibrary.Pricing.Calibration;

/// <summary>
/// Represents a model parameter that can be calibrated.
/// </summary>
public class ModelParameter
{
    public string Name { get; set; } = string.Empty;
    public double Value { get; set; }
    public bool IsFixed { get; set; }
    public double LowerBound { get; set; } = double.NegativeInfinity;
    public double UpperBound { get; set; } = double.PositiveInfinity;
}

/// <summary>
/// Interface for model calibrators.
/// </summary>
public interface IModelCalibrator
{
    void Calibrate(IEnumerable<IInstrument> marketInstruments, IEnumerable<double> marketPrices);
}
