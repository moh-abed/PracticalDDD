using System;
using System.Diagnostics;

namespace Sample.Domain
{
    public static class Printer
    {
        public static void Print(string input, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(input);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void Print(ConsoleColor color)
        {
            var method = new StackTrace().GetFrame(1).GetMethod();
            var callInfo = method.DeclaringType.Name + "." + method.Name;

            Console.ForegroundColor = color;
            Console.WriteLine(callInfo);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
