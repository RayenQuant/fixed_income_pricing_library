#nullable enable
using System.Collections.Generic;
using FixedIncomePricingLibrary.Core.Models;

namespace FixedIncomePricingLibrary.Core.Interfaces;

/// <summary>
/// Defines the contract for calibrating interest rate models to market data.
/// </summary>
public interface IModelCalibrator
{
    /// <summary>
    /// Calibrates a model using the provided market data instruments.
    /// </summary>
    CalibrationResult Calibrate(string modelType, IEnumerable<object> calibrationInstruments);
}
