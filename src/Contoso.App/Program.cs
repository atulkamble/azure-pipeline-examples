using Contoso.Utilities;

var count = args.Length > 0 && int.TryParse(args[0], out var parsed) ? parsed : 10;
var sequence = FibonacciGenerator.Generate(count);

Console.WriteLine($"Fibonacci sequence for count={count}:");
Console.WriteLine(string.Join(", ", sequence));
