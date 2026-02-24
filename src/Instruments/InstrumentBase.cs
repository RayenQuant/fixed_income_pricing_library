#nullable enable
using System;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Core.Interfaces;

namespace FixedIncomePricingLibrary.Instruments;

/// <summary>
/// Providing common properties for all financial instruments.
/// </summary>
public abstract class InstrumentBase : IInstrument
{
    public string Id { get; set; } = string.Empty;
    public InstrumentType InstrumentType { get; set; }
    public Currency Currency { get; set; } = Currency.USD;
    public double Notional { get; set; }
    public DateTime MaturityDate { get; set; }

    // Additional properties common to FI instruments
    public DateTime IssueDate { get; set; }
    public DayCountConvention DayCount { get; set; }
}
