using System;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Instruments;
using Xunit;

namespace FixedIncomePricingLibrary.UnitTests.Instruments;

public class InstrumentTests
{
    [Fact]
    public void Bond_Creation_SetsCorrectProperties()
    {
        var bond = new Bond
        {
            Id = "BOND001",
            Notional = 1000,
            CouponRate = 0.05,
            Frequency = CouponFrequency.SemiAnnual,
            MaturityDate = new DateTime(2030, 1, 1)
        };

        Assert.Equal(InstrumentType.FixedRateBond, bond.InstrumentType);
        Assert.Equal(1000, bond.Notional);
        Assert.Equal(0.05, bond.CouponRate);
    }

    [Fact]
    public void InterestRateSwap_Creation_SetsCorrectProperties()
    {
        var swap = new InterestRateSwap
        {
            Id = "SWAP001",
            Notional = 10_000_000,
            FixedRate = 0.0425,
            PayFixed = true,
            FloatIndex = FloatingRateIndex.SOFR,
            MaturityDate = new DateTime(2030, 3, 1)
        };

        Assert.Equal(InstrumentType.VanillaSwap, swap.InstrumentType);
        Assert.True(swap.PayFixed);
        Assert.Equal(FloatingRateIndex.SOFR, swap.FloatIndex);
    }

    [Fact]
    public void InstrumentFactory_CreatesCorrectTypes()
    {
        var bond = InstrumentFactory.Create(InstrumentType.FixedRateBond);
        var swap = InstrumentFactory.Create(InstrumentType.VanillaSwap);

        Assert.IsType<Bond>(bond);
        Assert.IsType<InterestRateSwap>(swap);
    }
}
