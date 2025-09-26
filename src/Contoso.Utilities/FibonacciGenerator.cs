using System;
using System.Collections.Generic;

namespace Contoso.Utilities;

public static class FibonacciGenerator
{
    public static IReadOnlyList<int> Generate(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be negative.");
        }

        if (count == 0)
        {
            return Array.Empty<int>();
        }

        var sequence = new List<int>(capacity: count);
        for (var index = 0; index < count; index++)
        {
            switch (index)
            {
                case 0:
                    sequence.Add(0);
                    break;
                case 1:
                    sequence.Add(1);
                    break;
                default:
                    checked
                    {
                        sequence.Add(sequence[index - 1] + sequence[index - 2]);
                    }
                    break;
            }
        }

        return sequence;
    }
}
