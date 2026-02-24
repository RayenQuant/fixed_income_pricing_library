#nullable enable
using System;

namespace FixedIncomePricingLibrary.Core.Exceptions;

/// <summary>
/// Exception thrown when data validation fails.
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }

    public ValidationException(string message, Exception innerException) : base(message, innerException) { }
}
