#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using FixedIncomePricingLibrary.Risk;

namespace FixedIncomePricingLibrary.Samples.ConsoleApp;

public static class ReportGenerator
{
    public static void PrintPortfolioReport(PortfolioReport report)
    {
        Console.WriteLine($"\nPORTFOLIO REPORT: {report.PortfolioName}");
        Console.WriteLine(new string('-', 60));
        Console.WriteLine($"{"ID",-15} | {"Type",-15} | {"Qty",-8} | {"NPV",-12}");
        Console.WriteLine(new string('-', 60));

        foreach (var item in report.Items)
        {
            Console.WriteLine($"{item.InstrumentId,-15} | {item.InstrumentType,-15} | {item.Quantity,8:N2} | {item.Npv,12:N2}");
        }

        Console.WriteLine(new string('-', 60));
        Console.WriteLine($"{"TOTAL NPV",-42} | {report.TotalNpv,12:N2}");
        Console.WriteLine(new string('-', 60));
    }

    public static void ExportToCsv(string filePath, IEnumerable<PositionDetail> data)
    {
        using var writer = new StreamWriter(filePath);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(data);
    }
}
