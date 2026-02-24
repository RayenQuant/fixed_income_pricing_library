#nullable enable
using System;
using FixedIncomePricingLibrary.Core.Enums;

namespace FixedIncomePricingLibrary.Core.Interfaces;

/// <summary>
/// Defines the base contract for all fixed income instruments.
/// </summary>
public interface IInstrument
{
    /// <summary>
    /// Unique identifier for the instrument.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// The type of fixed income instrument.
    /// </summary>
    InstrumentType InstrumentType { get; }

    /// <summary>
    /// The currency in which the instrument is denominated.
    /// </summary>
    Currency Currency { get; }

    /// <summary>
    /// The principal or face value of the instrument.
    /// </summary>
    double Notional { get; }

    /// <summary>
    /// The date on which the instrument matures and principal is returned.
    /// </summary>
    DateTime MaturityDate { get; }
}
