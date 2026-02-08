using BenchmarkDotNet.Running;
using System.Globalization;

namespace XenoAtom.Logging.Benchmark;

public static class Program
{
    private enum NormalizeArgumentsResult
    {
        Success,
        Exit,
        Error
    }

    private static readonly IReadOnlyDictionary<string, string[]> NamedSuites = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
    {
        ["comparison"] = ["Disabled", "EnabledSimple", "EnabledStructured", "EnabledException"],
        ["async"] = ["AsyncEnabledStructured", "AsyncEnabledException"],
    };

    private static readonly IReadOnlyDictionary<string, string> SuiteAliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["sync"] = "comparison",
    };

    public static int Main(string[] args)
    {
        if (args.Any(arg => string.Equals(arg, "--validate-format", StringComparison.OrdinalIgnoreCase)))
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
            return FormatValidation.Run(Console.Out, Console.Error);
        }

        var normalizeResult = TryNormalizeArguments(args, out var normalizedArgs);
        if (normalizeResult == NormalizeArgumentsResult.Exit)
        {
            return 0;
        }

        if (normalizeResult == NormalizeArgumentsResult.Error)
        {
            return 1;
        }

        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(normalizedArgs);
        return 0;
    }

    private static NormalizeArgumentsResult TryNormalizeArguments(string[] args, out string[] normalizedArgs)
    {
        var selectedCategories = new HashSet<string>(StringComparer.Ordinal);
        var result = new List<string>(args.Length + 16);

        for (var index = 0; index < args.Length; index++)
        {
            var arg = args[index];
            if (string.Equals(arg, "--suite", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(arg, "-s", StringComparison.OrdinalIgnoreCase))
            {
                if (index + 1 >= args.Length)
                {
                    Console.Error.WriteLine("Missing suite name after --suite.");
                    PrintSuites();
                    normalizedArgs = Array.Empty<string>();
                    return NormalizeArgumentsResult.Error;
                }

                var suiteName = args[++index];
                if (!TryResolveSuiteCategories(suiteName, out var categories))
                {
                    Console.Error.WriteLine($"Unknown suite '{suiteName}'.");
                    PrintSuites();
                    normalizedArgs = Array.Empty<string>();
                    return NormalizeArgumentsResult.Error;
                }

                foreach (var category in categories)
                {
                    selectedCategories.Add(category);
                }

                continue;
            }

            if (string.Equals(arg, "--list-suites", StringComparison.OrdinalIgnoreCase))
            {
                PrintSuites();
                normalizedArgs = Array.Empty<string>();
                return NormalizeArgumentsResult.Exit;
            }

            result.Add(arg);
        }

        if (selectedCategories.Count > 0 && !ContainsCategoryOption(result))
        {
            result.Add("--anyCategories");
            foreach (var category in selectedCategories)
            {
                result.Add(category);
            }
        }

        normalizedArgs = result.ToArray();
        return NormalizeArgumentsResult.Success;
    }

    private static bool ContainsCategoryOption(List<string> args)
    {
        foreach (var arg in args)
        {
            if (string.Equals(arg, "--anyCategories", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(arg, "--allCategories", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryResolveSuiteCategories(string suiteName, out string[] categories)
    {
        if (NamedSuites.TryGetValue(suiteName, out categories!))
        {
            return true;
        }

        if (SuiteAliases.TryGetValue(suiteName, out var canonicalName) &&
            NamedSuites.TryGetValue(canonicalName, out categories!))
        {
            return true;
        }

        categories = Array.Empty<string>();
        return false;
    }

    private static void PrintSuites()
    {
        Console.WriteLine("Available suites:");
        foreach (var suite in NamedSuites.Keys.OrderBy(key => key, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine($"  {suite}");
        }
        Console.WriteLine();
        Console.WriteLine("Aliases:");
        Console.WriteLine("  sync -> comparison");
        Console.WriteLine();
        Console.WriteLine("Usage: --suite <name> (repeatable), --list-suites");
    }
}
