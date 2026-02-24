#nullable enable
using System;

namespace FixedIncomePricingLibrary.Core.Exceptions;

/// <summary>
/// Exception thrown when a model calibration process fails to converge or encounters an error.
/// </summary>
public class CalibrationException : Exception
{
    public CalibrationException(string message) : base(message) { }

    public CalibrationException(string message, Exception innerException) : base(message, innerException) { }
}
