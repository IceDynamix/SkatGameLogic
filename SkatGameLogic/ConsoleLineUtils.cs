using System;
using System.Collections.Generic;

namespace SkatGameLogic
{
    public static class ConsoleLineUtils
    {
        public static bool GetBool(string prompt)
        {
            Console.Write($"{prompt} (Y/n): ");
            return Console.ReadLine() != "n";
        }

        public static int? GetInt()
        {
            while (true)
            {
                var input = Console.ReadLine();
                if (input == "")
                    return null;

                var success = int.TryParse(input, out int n);
                if (!success)
                    Console.WriteLine("Could not read number, please try again");
                else
                    return n;
            }
        }
        

    }
}