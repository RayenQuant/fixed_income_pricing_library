#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Core.Interfaces;
using FixedIncomePricingLibrary.Core.Models;
using FixedIncomePricingLibrary.Instruments;
using MathNet.Numerics.Distributions;

namespace FixedIncomePricingLibrary.Pricing.Engines;

/// <summary>
/// Implements Black-76 model for pricing options on forwards (Swaptions, Caps/Floors).
/// Forward rates and annuities are derived from the yield curve.
/// </summary>
public class BlackEngine : IPricingEngine
{
    private readonly IYieldCurve _curve;
    private readonly IVolatilitySurface _volSurface;

    public BlackEngine(IYieldCurve curve, IVolatilitySurface volSurface)
    {
        _curve = curve;
        _volSurface = volSurface;
    }

    public PricingResult Price(IInstrument instrument)
    {
        if (instrument is Swaption swaption)
            return PriceSwaption(swaption);

        if (instrument is CapFloor capFloor)
            return PriceCapFloor(capFloor);

        throw new NotImplementedException($"BlackEngine does not support instrument type: {instrument.InstrumentType}");
    }

    // ═══════════════════════ SWAPTION PRICING ═══════════════════════

    private PricingResult PriceSwaption(Swaption swaption)
    {
        var swap = swaption.UnderlyingSwap;
        double T = (swaption.ExpiryDate - _curve.ValuationDate).TotalDays / 365.0;
        if (T <= 0) return new PricingResult(swaption) { DirtyPrice = 0, CleanPrice = 0, Timestamp = DateTime.UtcNow };

        // Compute forward swap rate from the curve: F = (DF_start - DF_end) / Annuity
        double dfStart = _curve.GetDiscountFactor(swaption.ExpiryDate);
        double dfEnd = _curve.GetDiscountFactor(swap.MaturityDate);
        double annuity = ComputeAnnuity(swaption.ExpiryDate, swap.MaturityDate, swap.FixedFrequency);

        double F = annuity > 0 ? (dfStart - dfEnd) / annuity : swap.FixedRate;
        double K = swap.FixedRate;

        double tenorYears = (swap.MaturityDate - swaption.ExpiryDate).TotalDays / 365.0;
        string tenorStr = $"{(int)Math.Round(tenorYears)}Y";
        double sigma = _volSurface.GetVolatility(swaption.ExpiryDate, tenorStr, K);

        // Black-76: Value = Notional * Annuity * [F*N(d1) - K*N(d2)] for payer
        double d1 = (Math.Log(F / K) + 0.5 * sigma * sigma * T) / (sigma * Math.Sqrt(T));
        double d2 = d1 - sigma * Math.Sqrt(T);

        double pv;
        if (swaption.OptionType == OptionType.Payer)
            pv = swaption.Notional * annuity * (F * Normal.CDF(0, 1, d1) - K * Normal.CDF(0, 1, d2));
        else
            pv = swaption.Notional * annuity * (K * Normal.CDF(0, 1, -d2) - F * Normal.CDF(0, 1, -d1));

        return new PricingResult(swaption) { DirtyPrice = pv, CleanPrice = pv, Timestamp = DateTime.UtcNow };
    }

    // ═══════════════════════ CAP/FLOOR PRICING ═══════════════════════

    private PricingResult PriceCapFloor(CapFloor capFloor)
    {
        double totalPv = 0;

        // Price each caplet/floorlet individually using Black-76
        int periodsPerYear = capFloor.Frequency switch
        {
            CouponFrequency.Quarterly => 4,
            CouponFrequency.SemiAnnual => 2,
            CouponFrequency.Monthly => 12,
            CouponFrequency.Annual => 1,
            _ => 4
        };
        double tau = 1.0 / periodsPerYear;

        DateTime current = capFloor.IssueDate.AddMonths(12 / periodsPerYear); // skip first period
        while (current <= capFloor.MaturityDate)
        {
            DateTime periodStart = current.AddMonths(-12 / periodsPerYear);
            DateTime periodEnd = current;

            double T = (periodStart - _curve.ValuationDate).TotalDays / 365.0;
            if (T <= 0) { current = current.AddMonths(12 / periodsPerYear); continue; }

            double dfStart = _curve.GetDiscountFactor(periodStart);
            double dfEnd = _curve.GetDiscountFactor(periodEnd);

            // Forward rate for the period
            double F = (dfStart / dfEnd - 1.0) / tau;
            double K = capFloor.Strike;

            string tenorStr = $"{(int)Math.Ceiling((periodEnd - _curve.ValuationDate).TotalDays / 365.0)}Y";
            double sigma = _volSurface.GetVolatility(periodStart, tenorStr, K);

            double d1 = (Math.Log(F / K) + 0.5 * sigma * sigma * T) / (sigma * Math.Sqrt(T));
            double d2 = d1 - sigma * Math.Sqrt(T);

            double capletPv;
            if (capFloor.Type == OptionType.Cap)
                capletPv = dfEnd * tau * capFloor.Notional * (F * Normal.CDF(0, 1, d1) - K * Normal.CDF(0, 1, d2));
            else // Floor
                capletPv = dfEnd * tau * capFloor.Notional * (K * Normal.CDF(0, 1, -d2) - F * Normal.CDF(0, 1, -d1));

            totalPv += capletPv;
            current = current.AddMonths(12 / periodsPerYear);
        }

        return new PricingResult(capFloor) { DirtyPrice = totalPv, CleanPrice = totalPv, Timestamp = DateTime.UtcNow };
    }

    // ═══════════════════════ HELPERS ═══════════════════════

    /// <summary>
    /// Computes the swap annuity (PV of fixed leg basis points) from the yield curve.
    /// Annuity = Σ τ_i × DF(T_i) for each fixed payment date.
    /// </summary>
    private double ComputeAnnuity(DateTime startDate, DateTime endDate, CouponFrequency freq)
    {
        int periodsPerYear = freq switch
        {
            CouponFrequency.Annual => 1,
            CouponFrequency.SemiAnnual => 2,
            CouponFrequency.Quarterly => 4,
            CouponFrequency.Monthly => 12,
            _ => 2
        };
        double tau = 1.0 / periodsPerYear;
        double annuity = 0;

        DateTime current = startDate.AddMonths(12 / periodsPerYear);
        while (current <= endDate)
        {
            annuity += tau * _curve.GetDiscountFactor(current);
            current = current.AddMonths(12 / periodsPerYear);
        }
        return annuity;
    }
}
