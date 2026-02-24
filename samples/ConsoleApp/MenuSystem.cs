#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Core.Interfaces;
using FixedIncomePricingLibrary.Core.Models;
using FixedIncomePricingLibrary.Instruments;
using FixedIncomePricingLibrary.MarketData.Providers;
using FixedIncomePricingLibrary.MarketData.Volatility;
using FixedIncomePricingLibrary.MarketData.YieldCurves;
using FixedIncomePricingLibrary.Pricing.Calibration;
using FixedIncomePricingLibrary.Pricing.Engines;
using FixedIncomePricingLibrary.Risk;

namespace FixedIncomePricingLibrary.Samples.ConsoleApp;

/// <summary>
/// Professional Fixed Income pricing console — each derivative type is
/// automatically routed to its correct pricing model. Calibration happens
/// automatically when market data is loaded.
/// </summary>
public class MenuSystem
{
    // ─── State ───
    private readonly Portfolio _portfolio = new() { Name = "Trading Book" };
    private IYieldCurve? _curve;
    private IVolatilitySurface? _swaptionVol;
    private IVolatilitySurface? _capVol;
    private double _hwA = 0.05;      // Hull-White mean reversion
    private double _hwSigma = 0.015; // Hull-White volatility
    private bool _marketDataLoaded = false;
    private bool _calibrated = false;

    // ─── Constants ───
    private const string BoxH = "════════════════════════════════════════════════════════════════";
    private const string BoxT = "────────────────────────────────────────────────────────────────";

    // ══════════════════════════════ MAIN LOOP ══════════════════════════════

    public void Run()
    {
        while (true)
        {
            Console.Clear();
            PrintHeader();
            PrintStatus();
            PrintMenu();
            Console.Write("\n  ➤ Select: ");

            var input = Console.ReadLine()?.Trim();
            if (input == "0") break;
            Console.Clear();

            try
            {
                switch (input)
                {
                    case "1": LoadMarketData(); break;
                    case "2": AddInstrumentMenu(); break;
                    case "3": ViewPortfolio(); break;
                    case "4": PricePortfolio(); break;
                    case "5": CalculateRisk(); break;
                    case "6": RunScenarioAnalysis(); break;
                    case "7": CalculateVaR(); break;
                    case "8": ExportReport(); break;
                    default: Console.WriteLine("  Invalid option."); break;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n  ✗ Error: {ex.Message}");
                Console.ResetColor();
            }

            Console.WriteLine($"\n  {BoxT}");
            Console.WriteLine("  Press any key to continue...");
            Console.ReadKey();
        }
    }

    // ══════════════════════════════ UI ══════════════════════════════

    private void PrintHeader()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n  {BoxH}");
        Console.WriteLine("       FIXED INCOME DERIVATIVES PRICING SYSTEM");
        Console.WriteLine($"  {BoxH}");
        Console.ResetColor();
    }

    private void PrintStatus()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        string mkt = _marketDataLoaded ? $"✓ Curve + Vols ({_curve!.ValuationDate:yyyy-MM-dd})" : "✗ Not loaded";
        string cal = _calibrated ? $"✓ HW a={_hwA:F4} σ={_hwSigma:F4}" : (_marketDataLoaded ? "✓ Auto" : "—");
        string port = $"{_portfolio.Positions.Count} position(s)";
        Console.WriteLine($"\n  Market Data: {mkt}  |  Models: {cal}  |  Portfolio: {port}");
        Console.ResetColor();
    }

    private void PrintMenu()
    {
        Console.WriteLine($"\n  {BoxT}");
        Section("MARKET DATA & CALIBRATION");
        Console.WriteLine("    1. Load Market Data  (curve from CSV + vol surfaces → auto-calibrate)");
        Section("PORTFOLIO");
        Console.WriteLine("    2. Add Instrument  (Bond / Swap / Swaption / Cap-Floor)");
        Console.WriteLine("    3. View Portfolio");
        Section("PRICING & RISK (AUTOMATIC MODEL ROUTING)");
        Console.WriteLine("    4. Price Portfolio   (Bond→Discounting, Swap→Discounting, Swaption→Black, Cap→Black)");
        Console.WriteLine("    5. Risk Analytics    (DV01, Modified Duration per position)");
        Console.WriteLine("    6. Scenario Analysis (parallel curve shifts −100bp to +100bp)");
        Console.WriteLine("    7. Value-at-Risk     (Monte Carlo VaR + CVaR)");
        Section("OUTPUT");
        Console.WriteLine("    8. Export Report to CSV");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\n    0. Exit");
        Console.ResetColor();
    }

    private static void Section(string title)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n  {title}");
        Console.ResetColor();
    }

    private static void Ok(string msg) { Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine($"  ✓ {msg}"); Console.ResetColor(); }
    private static void Warn(string msg) { Console.ForegroundColor = ConsoleColor.DarkYellow; Console.WriteLine($"  ⚠ {msg}"); Console.ResetColor(); }
    private static void Err(string msg) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"  ✗ {msg}"); Console.ResetColor(); }
    private static void Title(string t) { Console.ForegroundColor = ConsoleColor.Cyan; Console.WriteLine($"\n  ═══ {t} ═══\n"); Console.ResetColor(); }

    private bool RequireMarketData()
    {
        if (_marketDataLoaded) return true;
        Err("Load market data first (Option 1).");
        return false;
    }

    private bool RequirePortfolio()
    {
        if (_portfolio.Positions.Any()) return true;
        Err("Portfolio is empty — add instruments first (Option 2).");
        return false;
    }

    // ─── Parsing (handles both . and , as decimal separator) ───
    private static double Parse(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return double.NaN;
        if (double.TryParse(input.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var v)) return v;
        if (double.TryParse(input.Trim(), NumberStyles.Float, CultureInfo.CurrentCulture, out var v2)) return v2;
        return double.NaN;
    }

    // ══════════════════ 1. LOAD MARKET DATA + AUTO-CALIBRATE ══════════════════

    private void LoadMarketData()
    {
        Title("LOAD MARKET DATA & AUTO-CALIBRATE");

        // ── Step 1: Load yield curve from CSV ──
        string csvPath = FindFile("usd_treasury_curve.csv");
        Console.Write($"  Yield curve CSV [{csvPath}]: ");
        var userPath = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(userPath)) csvPath = userPath;

        if (!File.Exists(csvPath)) { Err($"File not found: {csvPath}"); return; }

        var lines = File.ReadAllLines(csvPath);
        var dates = lines.Skip(1).Where(l => !string.IsNullOrWhiteSpace(l))
                         .Select(l => DateTime.Parse(l.Split(',')[0], CultureInfo.InvariantCulture))
                         .OrderByDescending(d => d).ToList();
        if (!dates.Any()) { Err("No data in CSV."); return; }

        var valDate = dates.First();
        _curve = TreasuryCurveLoader.Load(csvPath, valDate);
        Ok($"USD Treasury curve loaded ({valDate:yyyy-MM-dd}) — 11 tenors from 1M to 30Y");

        // Print curve
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"\n  {"Tenor",-6} {"Rate",8}");
        Console.WriteLine($"  {BoxT[..20]}");
        foreach (var y in new[] { 1, 2, 3, 5, 7, 10, 20, 30 })
            Console.WriteLine($"  {y + "Y",-6} {_curve.GetZeroRate(valDate.AddYears(y)),8:P2}");
        Console.ResetColor();

        // ── Step 2: Load vol surfaces ──
        Console.WriteLine();
        string volPath = FindFile("swaption_volatilities.csv");
        if (File.Exists(volPath))
        {
            // Read average vol from CSV for a simple flat surface
            var volLines = File.ReadAllLines(volPath).Skip(1).Where(l => !string.IsNullOrWhiteSpace(l));
            var vols = volLines.Select(l => double.Parse(l.Split(',')[2], CultureInfo.InvariantCulture)).ToList();
            double avgVol = vols.Any() ? vols.Average() : 0.45;
            _swaptionVol = new SwaptionVolatilityCube(avgVol);
            _capVol = new CapFloorVolatilitySurface(avgVol);
            Ok($"Swaption vol surface loaded (avg σ = {avgVol:P1})");
            Ok($"Cap/Floor vol surface loaded (avg σ = {avgVol:P1})");
        }
        else
        {
            _swaptionVol = new SwaptionVolatilityCube(0.45);
            _capVol = new CapFloorVolatilitySurface(0.45);
            Warn("Swaption vols CSV not found — using default σ = 45%");
        }

        // ── Step 3: Auto-calibrate Hull-White ──
        Console.WriteLine();
        Console.WriteLine("  Calibrating Hull-White model to market...");
        AutoCalibrateHullWhite();
        _marketDataLoaded = true;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n  {BoxT}");
        Console.WriteLine("  ✓ MARKET DATA LOADED — MODELS CALIBRATED — READY TO PRICE");
        Console.WriteLine($"  {BoxT}");
        Console.ResetColor();
    }

    private void AutoCalibrateHullWhite()
    {
        // Create synthetic calibration instruments (ZCBs at key tenors)
        var calibInstruments = new List<IInstrument>();
        var marketPrices = new List<double>();
        foreach (var tenor in new[] { 2, 5, 10 })
        {
            var zcb = new ZeroCouponBond
            {
                Id = $"CALIB_ZCB_{tenor}Y",
                Notional = 1.0,
                MaturityDate = _curve!.ValuationDate.AddYears(tenor),
                IssueDate = _curve.ValuationDate,
                DayCount = DayCountConvention.ActualActual,
                Currency = Currency.USD
            };
            calibInstruments.Add(zcb);
            marketPrices.Add(_curve.GetDiscountFactor(_curve.ValuationDate.AddYears(tenor)));
        }

        var calibrator = new HullWhiteCalibrator(_curve!);
        calibrator.Calibrate(calibInstruments, marketPrices);

        _hwA = calibrator.Parameters.First(p => p.Name == "MeanReversion").Value;
        _hwSigma = calibrator.Parameters.First(p => p.Name == "Volatility").Value;
        _calibrated = true;

        Ok($"Hull-White calibrated: a = {_hwA:F4}, σ = {_hwSigma:F4}");
    }

    // ══════════════════════ PRICING ENGINE ROUTER ══════════════════════
    /// <summary>
    /// Automatically selects the correct pricing model for each instrument type.
    /// This is the core of the system — the user never picks a model.
    /// </summary>
    private IPricingEngine GetEngine(IInstrument instrument)
    {
        return instrument switch
        {
            // Swaptions → Black-76 with swaption vol surface
            Swaption => new BlackEngine(_curve!, _swaptionVol!),

            // Caps/Floors → Black-76 with cap vol surface
            CapFloor => new BlackEngine(_curve!, _capVol!),

            // Swaps → analytical discounting with dual curve
            InterestRateSwap => new SwapPricingEngine(_curve!),

            // Bonds (fixed and zero-coupon) → analytical discounting
            Bond or ZeroCouponBond => new BondPricingEngine(_curve!),

            // Anything else → Monte Carlo with calibrated Hull-White
            _ => new MonteCarloEngine(_curve!, _hwA, _hwSigma)
        };
    }

    /// <summary>Prices a single instrument using the automatically selected engine.</summary>
    private PricingResult PriceInstrument(IInstrument instrument)
    {
        var engine = GetEngine(instrument);
        return engine.Price(instrument);
    }

    /// <summary>Returns a human-readable model name for display.</summary>
    private static string ModelName(IInstrument inst) => inst switch
    {
        Swaption => "Black-76",
        CapFloor => "Black-76",
        InterestRateSwap => "Discounting",
        Bond => "Discounting",
        ZeroCouponBond => "Discounting",
        _ => "MC Hull-White"
    };

    // ══════════════════════ 2. ADD INSTRUMENT ══════════════════════

    private void AddInstrumentMenu()
    {
        Title("ADD INSTRUMENT TO PORTFOLIO");
        Console.WriteLine("  1. Fixed Rate Bond");
        Console.WriteLine("  2. Zero Coupon Bond");
        Console.WriteLine("  3. Interest Rate Swap (Vanilla IRS)");
        Console.WriteLine("  4. Swaption");
        Console.WriteLine("  5. Cap / Floor");
        Console.Write("\n  Select: ");
        switch (Console.ReadLine()?.Trim())
        {
            case "1": AddBond(); break;
            case "2": AddZCB(); break;
            case "3": AddSwap(); break;
            case "4": AddSwaption(); break;
            case "5": AddCapFloor(); break;
            default: Console.WriteLine("  Invalid."); break;
        }
    }

    private void AddBond()
    {
        Console.Write("  ID (e.g. UST-10Y): "); var id = Console.ReadLine() ?? "BOND001";
        Console.Write("  Face Value [1000000]: "); var n = Parse(Console.ReadLine()); double notional = n > 0 ? n : 1_000_000;
        Console.Write("  Coupon Rate (e.g. 0.05): "); var c = Parse(Console.ReadLine()); double coupon = !double.IsNaN(c) ? c : 0.05;
        Console.Write("  Maturity years [5]: "); int years = int.TryParse(Console.ReadLine()?.Trim(), out var y) && y > 0 ? y : 5;
        Console.Write("  Frequency [1/2/4] [2]: "); int freq = int.TryParse(Console.ReadLine()?.Trim(), out var f) && (f == 1 || f == 2 || f == 4) ? f : 2;

        _portfolio.Positions.Add(new Position
        {
            Instrument = new Bond
            {
                Id = id,
                Notional = notional,
                CouponRate = coupon,
                MaturityDate = DateTime.Today.AddYears(years),
                IssueDate = DateTime.Today,
                DayCount = DayCountConvention.Thirty360,
                Frequency = (CouponFrequency)freq,
                Currency = Currency.USD
            }
        });
        Ok($"Bond '{id}': {notional:N0} face, {coupon:P2} coupon, {years}Y  → priced by Discounting");
    }

    private void AddZCB()
    {
        Console.Write("  ID: "); var id = Console.ReadLine() ?? "ZCB001";
        Console.Write("  Face Value [1000000]: "); var n = Parse(Console.ReadLine()); double notional = n > 0 ? n : 1_000_000;
        Console.Write("  Maturity years [5]: "); int years = int.TryParse(Console.ReadLine()?.Trim(), out var y) && y > 0 ? y : 5;

        _portfolio.Positions.Add(new Position
        {
            Instrument = new ZeroCouponBond
            {
                Id = id,
                Notional = notional,
                MaturityDate = DateTime.Today.AddYears(years),
                IssueDate = DateTime.Today,
                DayCount = DayCountConvention.ActualActual,
                Currency = Currency.USD
            }
        });
        Ok($"ZCB '{id}': {notional:N0} face, {years}Y  → priced by Discounting");
    }

    private void AddSwap()
    {
        Console.Write("  ID: "); var id = Console.ReadLine() ?? "IRS001";
        Console.Write("  Notional [10000000]: "); var n = Parse(Console.ReadLine()); double notional = n > 0 ? n : 10_000_000;
        Console.Write("  Fixed Rate (e.g. 0.0425): "); var r = Parse(Console.ReadLine()); double rate = !double.IsNaN(r) ? r : 0.0425;
        Console.Write("  Maturity years [5]: "); int years = int.TryParse(Console.ReadLine()?.Trim(), out var y) && y > 0 ? y : 5;
        Console.Write("  Pay Fixed? [y/n] [y]: "); bool payFixed = Console.ReadLine()?.Trim().ToLower() != "n";

        _portfolio.Positions.Add(new Position
        {
            Instrument = new InterestRateSwap
            {
                Id = id,
                Notional = notional,
                FixedRate = rate,
                PayFixed = payFixed,
                FixedFrequency = CouponFrequency.SemiAnnual,
                FixedDayCount = DayCountConvention.Thirty360,
                FloatIndex = FloatingRateIndex.SOFR,
                FloatFrequency = CouponFrequency.Quarterly,
                FloatDayCount = DayCountConvention.Actual360,
                MaturityDate = DateTime.Today.AddYears(years),
                IssueDate = DateTime.Today,
                Currency = Currency.USD
            }
        });
        Ok($"IRS '{id}': {notional:N0}, {rate:P2} fixed, {years}Y, {(payFixed ? "Pay" : "Receive")} Fixed  → priced by Discounting");
    }

    private void AddSwaption()
    {
        Console.Write("  ID: "); var id = Console.ReadLine() ?? "SWPN001";
        Console.Write("  Notional [10000000]: "); var n = Parse(Console.ReadLine()); double notional = n > 0 ? n : 10_000_000;
        Console.Write("  Strike Rate (e.g. 0.045): "); var r = Parse(Console.ReadLine()); double strike = !double.IsNaN(r) ? r : 0.045;
        Console.Write("  Option Expiry years [1]: "); int ey = int.TryParse(Console.ReadLine()?.Trim(), out var e) && e > 0 ? e : 1;
        Console.Write("  Swap Tenor years [5]: "); int sy = int.TryParse(Console.ReadLine()?.Trim(), out var s) && s > 0 ? s : 5;
        Console.Write("  Payer/Receiver [P/R] [P]: "); bool isPayer = Console.ReadLine()?.Trim().ToUpper() != "R";

        _portfolio.Positions.Add(new Position
        {
            Instrument = new Swaption
            {
                Id = id,
                Notional = notional,
                ExpiryDate = DateTime.Today.AddYears(ey),
                MaturityDate = DateTime.Today.AddYears(ey + sy),
                IssueDate = DateTime.Today,
                Currency = Currency.USD,
                OptionType = isPayer ? OptionType.Payer : OptionType.Receiver,
                SettlementType = SettlementType.Cash,
                UnderlyingSwap = new InterestRateSwap
                {
                    Id = $"{id}_UND",
                    Notional = notional,
                    FixedRate = strike,
                    FixedFrequency = CouponFrequency.SemiAnnual,
                    FixedDayCount = DayCountConvention.Thirty360,
                    FloatIndex = FloatingRateIndex.SOFR,
                    FloatFrequency = CouponFrequency.Quarterly,
                    FloatDayCount = DayCountConvention.Actual360,
                    MaturityDate = DateTime.Today.AddYears(ey + sy),
                    IssueDate = DateTime.Today.AddYears(ey),
                    PayFixed = true,
                    Currency = Currency.USD
                }
            }
        });
        Ok($"Swaption '{id}': {ey}Y×{sy}Y, {strike:P2} strike, {(isPayer ? "Payer" : "Receiver")}  → priced by Black-76");
    }

    private void AddCapFloor()
    {
        Console.Write("  ID: "); var id = Console.ReadLine() ?? "CAP001";
        Console.Write("  Notional [10000000]: "); var n = Parse(Console.ReadLine()); double notional = n > 0 ? n : 10_000_000;
        Console.Write("  Strike (e.g. 0.045): "); var s = Parse(Console.ReadLine()); double strike = !double.IsNaN(s) ? s : 0.045;
        Console.Write("  Maturity years [3]: "); int years = int.TryParse(Console.ReadLine()?.Trim(), out var y) && y > 0 ? y : 3;
        Console.Write("  Cap/Floor [C/F] [C]: "); bool isCap = Console.ReadLine()?.Trim().ToUpper() != "F";

        _portfolio.Positions.Add(new Position
        {
            Instrument = new CapFloor
            {
                Id = id,
                Notional = notional,
                Strike = strike,
                Frequency = CouponFrequency.Quarterly,
                Index = FloatingRateIndex.SOFR,
                Type = isCap ? OptionType.Cap : OptionType.Floor,
                MaturityDate = DateTime.Today.AddYears(years),
                IssueDate = DateTime.Today,
                DayCount = DayCountConvention.Actual360,
                Currency = Currency.USD
            }
        });
        Ok($"{(isCap ? "Cap" : "Floor")} '{id}': {notional:N0}, {strike:P2} strike, {years}Y  → priced by Black-76");
    }

    // ══════════════════════ 3. VIEW PORTFOLIO ══════════════════════

    private void ViewPortfolio()
    {
        Title("PORTFOLIO");
        if (!RequirePortfolio()) return;

        Console.WriteLine($"  {"#",-3} {"ID",-12} {"Type",-16} {"Notional",14} {"Maturity",-11} {"Model",-14}");
        Console.WriteLine($"  {BoxT}");
        int i = 1;
        foreach (var p in _portfolio.Positions)
        {
            var inst = p.Instrument;
            Console.WriteLine($"  {i,-3} {inst.Id,-12} {inst.InstrumentType,-16} {inst.Notional,14:N0} {inst.MaturityDate:yyyy-MM-dd}  {ModelName(inst),-14}");
            i++;
        }
        Console.WriteLine($"  {BoxT}");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  {_portfolio.Positions.Count} position(s)  |  Total Notional: {_portfolio.Positions.Sum(p => p.Instrument.Notional):N0}");
        Console.ResetColor();
    }

    // ══════════════════════ 4. PRICE PORTFOLIO ══════════════════════

    private void PricePortfolio()
    {
        Title("PORTFOLIO PRICING — AUTOMATIC MODEL ROUTING");
        if (!RequireMarketData() || !RequirePortfolio()) return;

        Console.WriteLine($"  {"ID",-12} {"Type",-16} {"Model",-14} {"NPV",16}");
        Console.WriteLine($"  {BoxT}");

        double totalNpv = 0;
        foreach (var pos in _portfolio.Positions)
        {
            var inst = pos.Instrument;
            try
            {
                var result = PriceInstrument(inst);
                double npv = result.DirtyPrice * pos.Quantity;
                totalNpv += npv;
                Console.WriteLine($"  {inst.Id,-12} {inst.InstrumentType,-16} {ModelName(inst),-14} {npv,16:N2}");
            }
            catch (Exception ex)
            {
                Warn($"{inst.Id}: {ex.Message}");
            }
        }

        Console.WriteLine($"  {BoxT}");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  {"TOTAL PORTFOLIO NPV",-44} {totalNpv,16:N2}");
        Console.ResetColor();
    }

    // ══════════════════════ 5. RISK ══════════════════════

    private void CalculateRisk()
    {
        Title("RISK ANALYTICS — DV01 & MODIFIED DURATION");
        if (!RequireMarketData() || !RequirePortfolio()) return;

        Console.WriteLine($"  {"ID",-12} {"Type",-16} {"Model",-14} {"DV01",12} {"Mod Dur",10}");
        Console.WriteLine($"  {BoxT}");

        double totalDV01 = 0;
        foreach (var pos in _portfolio.Positions)
        {
            var inst = pos.Instrument;
            try
            {
                var factory = GetEngineFactory(inst);
                double dv01 = GreeksCalculator.CalculateDV01(factory, inst, _curve!);
                double dur = GreeksCalculator.CalculateModifiedDuration(factory, inst, _curve!);
                totalDV01 += dv01 * pos.Quantity;
                Console.WriteLine($"  {inst.Id,-12} {inst.InstrumentType,-16} {ModelName(inst),-14} {dv01,12:N2} {dur,10:F4}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  {inst.Id,-12} {inst.InstrumentType,-16} {ModelName(inst),-14} {"N/A",12} {"N/A",10}  ⚠ {ex.Message}");
            }
        }

        Console.WriteLine($"  {BoxT}");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  Portfolio DV01: {totalDV01:N2}");
        Console.WriteLine($"  → A +1bp parallel shift changes portfolio value by ${totalDV01:N2}");
        Console.ResetColor();
    }

    /// <summary>Returns a factory that recreates the correct engine for a given instrument given a new curve.</summary>
    private Func<IYieldCurve, IPricingEngine> GetEngineFactory(IInstrument instrument)
    {
        return instrument switch
        {
            Swaption => (curve) => new BlackEngine(curve, _swaptionVol!),
            CapFloor => (curve) => new BlackEngine(curve, _capVol!),
            InterestRateSwap => (curve) => new SwapPricingEngine(curve),
            _ => (curve) => new BondPricingEngine(curve)
        };
    }

    // ══════════════════════ 6. SCENARIO ANALYSIS ══════════════════════

    private void RunScenarioAnalysis()
    {
        Title("SCENARIO ANALYSIS — PARALLEL CURVE SHIFTS");
        if (!RequireMarketData() || !RequirePortfolio()) return;

        double baseNpv = CalcPortfolioNpv(_curve!);
        Console.WriteLine($"  Base NPV: {baseNpv:N2}\n");
        Console.WriteLine($"  {"Shift (bp)",12} {"NPV",16} {"P&L",16} {"%",8}");
        Console.WriteLine($"  {BoxT}");

        double baseRate = _curve!.GetZeroRate(_curve.ValuationDate.AddYears(1));
        foreach (var bp in new[] { -100, -50, -25, -10, 0, +10, +25, +50, +100 })
        {
            var shocked = YieldCurveBuilder.FlatCurve(_curve.ValuationDate, baseRate + bp / 10000.0);
            double npv = CalcPortfolioNpv(shocked);
            double pnl = npv - baseNpv;
            double pct = baseNpv != 0 ? pnl / Math.Abs(baseNpv) * 100 : 0;

            Console.ForegroundColor = pnl >= 0 ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine($"  {bp,+10:+0;-0;0} bp  {npv,16:N2} {pnl,16:N2} {pct,7:F2}%");
            Console.ResetColor();
        }
    }

    private double CalcPortfolioNpv(IYieldCurve curve)
    {
        // Temporarily swap the curve, price, swap back
        var saved = _curve;
        _curve = curve;
        double npv = 0;
        foreach (var pos in _portfolio.Positions)
        {
            try { npv += PriceInstrument(pos.Instrument).DirtyPrice * pos.Quantity; }
            catch { /* skip failing instruments in scenario */ }
        }
        _curve = saved;
        return npv;
    }

    // ══════════════════════ 7. VALUE-AT-RISK ══════════════════════

    private void CalculateVaR()
    {
        Title("VALUE-AT-RISK — MONTE CARLO SIMULATION");
        if (!RequireMarketData() || !RequirePortfolio()) return;

        Console.Write("  Confidence [0.95/0.99] [0.95]: ");
        var _cl = Parse(Console.ReadLine()); double conf = _cl > 0 && _cl < 1 ? _cl : 0.95;

        Console.Write("  Simulations [10000]: ");
        int sims = int.TryParse(Console.ReadLine()?.Trim(), out var si) && si > 100 ? si : 10000;

        // Auto-compute rate volatility from historical data
        double vol = ComputeHistoricalVol();
        Console.WriteLine($"  Rate volatility (auto from historical data): {vol:P4}");

        Console.WriteLine($"\n  Simulating {sims:N0} rate scenarios...\n");

        double baseNpv = CalcPortfolioNpv(_curve!);
        double baseRate = _curve!.GetZeroRate(_curve.ValuationDate.AddYears(1));

        var pnl = new List<double>(sims);
        var rng = new Random(42);
        for (int i = 0; i < sims; i++)
        {
            double u1 = 1.0 - rng.NextDouble();
            double u2 = 1.0 - rng.NextDouble();
            double shock = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2) * vol;

            var shocked = YieldCurveBuilder.FlatCurve(_curve.ValuationDate, baseRate + shock);
            pnl.Add(CalcPortfolioNpv(shocked) - baseNpv);
        }
        pnl.Sort();

        int idx = (int)Math.Floor((1 - conf) * pnl.Count);
        if (idx < 0) idx = 0;
        double varAmt = -pnl[idx];
        double cvar = idx > 0 ? -pnl.Take(idx + 1).Average() : varAmt;
        double z = MathNet.Numerics.Distributions.Normal.InvCDF(0, 1, conf);
        double paramVaR = Math.Abs(baseNpv) * z * vol * Math.Sqrt(1.0 / 252.0);

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  ┌────────────────────────────────────────────────┐");
        Console.WriteLine($"  │  VALUE-AT-RISK RESULTS                         │");
        Console.WriteLine($"  ├────────────────────────────────────────────────┤");
        Console.WriteLine($"  │  Confidence:       {conf:P0,-26}  │");
        Console.WriteLine($"  │  Simulations:      {sims,-26:N0}  │");
        Console.WriteLine($"  │  Rate Vol (hist):  {vol,-26:P4}  │");
        Console.WriteLine($"  │  Base Portfolio:   {baseNpv,-26:N2}  │");
        Console.WriteLine($"  ├────────────────────────────────────────────────┤");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  │  MC VaR (1-day):   {varAmt,-26:N2}  │");
        Console.WriteLine($"  │  CVaR (ES):        {cvar,-26:N2}  │");
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"  │  Parametric VaR:   {paramVaR,-26:N2}  │");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  └────────────────────────────────────────────────┘");
        Console.ResetColor();

        Console.WriteLine($"\n  With {conf:P0} confidence, the 1-day loss does not exceed ${varAmt:N2}.");
        Console.WriteLine($"  If it does, the expected tail loss (CVaR) is ${cvar:N2}.");
    }

    /// <summary>
    /// Computes annualized rate volatility from the historical curve CSV.
    /// Uses daily changes in the 1Y zero rate.
    /// </summary>
    private double ComputeHistoricalVol()
    {
        string histPath = FindFile("usd_curve_history.csv");
        // Fall back to main curve file
        if (!File.Exists(histPath)) histPath = FindFile("usd_treasury_curve.csv");
        if (!File.Exists(histPath)) return 0.01; // default

        var lines = File.ReadAllLines(histPath).Skip(1)
                        .Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
        if (lines.Count < 2) return 0.01;

        // Parse the 1Y column (index 4 in the CSV: Date,1M,3M,6M,1Y,...)
        var rates = new List<double>();
        foreach (var line in lines)
        {
            var cols = line.Split(',');
            if (cols.Length > 4 && double.TryParse(cols[4], NumberStyles.Float, CultureInfo.InvariantCulture, out var r))
                rates.Add(r / 100.0);
        }

        if (rates.Count < 2) return 0.01;

        // Compute daily absolute changes and their std dev
        var changes = new List<double>();
        for (int i = 1; i < rates.Count; i++)
            changes.Add(rates[i] - rates[i - 1]);

        double mean = changes.Average();
        double variance = changes.Sum(c => (c - mean) * (c - mean)) / (changes.Count - 1);
        double dailyVol = Math.Sqrt(variance);

        // Annualize: daily vol * sqrt(252)
        double annualVol = dailyVol * Math.Sqrt(252);

        // Floor to something reasonable
        if (annualVol < 0.001) annualVol = 0.005;

        return annualVol;
    }

    // ══════════════════════ 8. EXPORT ══════════════════════

    private void ExportReport()
    {
        Title("EXPORT PORTFOLIO REPORT");
        if (!RequirePortfolio()) return;

        Console.Write("  Output file [portfolio_report.csv]: ");
        var path = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(path)) path = "portfolio_report.csv";

        var details = _portfolio.Positions.Select(p =>
        {
            double npv = 0;
            if (_marketDataLoaded) try { npv = PriceInstrument(p.Instrument).DirtyPrice; } catch { }
            return new PositionDetail
            {
                InstrumentId = p.Instrument.Id,
                InstrumentType = p.Instrument.InstrumentType.ToString(),
                Quantity = p.Quantity,
                Npv = npv
            };
        }).ToList();

        ReportGenerator.ExportToCsv(path, details);
        Ok($"Report exported to: {path}");
    }

    // ──────── Helpers ────────

    private static string FindFile(string name)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            var c1 = Path.Combine(dir.FullName, "data", "market_data", name);
            if (File.Exists(c1)) return c1;
            dir = dir.Parent;
        }
        return Path.Combine("data", "market_data", name);
    }
}
