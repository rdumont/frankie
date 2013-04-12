using System;

namespace RDumont.Frankie.Core
{
    public class Logger
    {
        public static Logger Current { get; private set; }

        public LoggingLevel Level { get; private set; }

        public static void Start(LoggingLevel level)
        {
            Current = new Logger {Level = level};
        }

        public void LogError(string message, params object[] args)
        {
            WriteColor(ConsoleColor.Red, message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            WriteColor(ConsoleColor.Yellow, message, args);
        }

        public void Log(LoggingLevel level, string message, params object[] args)
        {
            if(level >= this.Level)
                Console.WriteLine(message, args);
        }

        private static void WriteColor(ConsoleColor color, string message, params object[] args)
        {
            var previous = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message, args);
            Console.ForegroundColor = previous;
        }
    }

    public enum LoggingLevel
    {
        Debug = 1,
        Info = 2,
        Minimal = 3,
        Quiet = 4
    }
}
