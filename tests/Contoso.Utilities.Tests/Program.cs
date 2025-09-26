using System;
using System.Collections.Generic;
using System.Linq;
using Contoso.Utilities;

namespace Contoso.Utilities.Tests;

internal static class Program
{
    private static readonly List<string> _log = new();

    public static int Main()
    {
        try
        {
            RunAll();
            Console.WriteLine("All tests passed.");
            foreach (var entry in _log)
            {
                Console.WriteLine(entry);
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Test failure: {ex.Message}");
            return 1;
        }
    }

    private static void RunAll()
    {
        AssertSequence(Array.Empty<int>(), FibonacciGenerator.Generate(0), "Generate(0) returns empty sequence");
        AssertSequence(new[] { 0 }, FibonacciGenerator.Generate(1), "Generate(1) returns 0");
        AssertSequence(new[] { 0, 1 }, FibonacciGenerator.Generate(2), "Generate(2) returns 0,1");
        AssertSequence(new[] { 0, 1, 1, 2, 3, 5, 8 }, FibonacciGenerator.Generate(7), "Generate(7) returns expected sequence");
        AssertThrows<ArgumentOutOfRangeException>(() => FibonacciGenerator.Generate(-1), "Generate(-1) throws ArgumentOutOfRangeException");
    }

    private static void AssertSequence(IReadOnlyList<int> expected, IReadOnlyList<int> actual, string message)
    {
        if (!expected.SequenceEqual(actual))
        {
            throw new InvalidOperationException($"{message}. Expected [{string.Join(", ", expected)}] but found [{string.Join(", ", actual)}].");
        }

        _log.Add($"✔ {message}");
    }

    private static void AssertThrows<TException>(Action action, string message) where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException)
        {
            _log.Add($"✔ {message}");
            return;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"{message}. Expected {typeof(TException).Name} but received {ex.GetType().Name}.");
        }

        throw new InvalidOperationException($"{message}. Expected {typeof(TException).Name} but no exception was thrown.");
    }
}
