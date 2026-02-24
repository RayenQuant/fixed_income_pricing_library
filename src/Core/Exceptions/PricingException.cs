#nullable enable
using System;
using FixedIncomePricingLibrary.Core.Interfaces;

namespace FixedIncomePricingLibrary.Core.Exceptions;

/// <summary>
/// Exception thrown when an error occurs during the pricing of an instrument.
/// </summary>
public class PricingException : Exception
{
    /// <summary>
    /// The instrument that was being priced when the exception occurred.
    /// </summary>
    public IInstrument? Instrument { get; }

    public PricingException(string message) : base(message) { }

    public PricingException(string message, Exception innerException) : base(message, innerException) { }

    public PricingException(string message, IInstrument instrument) : base(message)
    {
        Instrument = instrument;
    }
}
