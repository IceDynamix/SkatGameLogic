using System;

namespace SkatGameLogic
{
    public static class ConsoleLineUtils
    {
        public static bool GetBool(string prompt)
        {
            Console.Write($"{prompt} (Y/n): ");
            return Console.ReadLine() != "n";
        }

        public static int? GetNullableInt(string prompt = default, int? min = null, int? max = null)
        {
            string promptString;
            if (prompt == default)
            {
                promptString = "Enter a number";
                if (min != null || max != null)
                {
                    var minString = min == null ? "" : min.ToString();
                    var maxString = max == null ? "" : max.ToString();
                    promptString += $" [{minString}-{maxString}]";
                }

                promptString += ": ";
            }
            else
                promptString = prompt;

            Console.Write(promptString);

            while (true)
            {
                var input = Console.ReadLine();
                if (input == "")
                    return null;

                var success = int.TryParse(input, out int n);
                if (!success)
                    Console.WriteLine("Could not read number, please try again");
                else if ((min != default && n < min) || (max != default && n > max))
                    Console.WriteLine("Number outside range, please try again");
                else
                    return n;
            }
        }
    }
}