#nullable enable
using System;

namespace FixedIncomePricingLibrary.Core.Exceptions;

/// <summary>
/// Exception thrown when an error occurs while retrieving or processing market data.
/// </summary>
public class MarketDataException : Exception
{
    public MarketDataException(string message) : base(message) { }

    public MarketDataException(string message, Exception innerException) : base(message, innerException) { }
}
