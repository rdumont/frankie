using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDumont.Frankie.Core;

namespace RDumont.Frankie.Specs.Testables
{
    public class TestLogger : Logger
    {
        public List<Log> Logs { get; set; }
        public List<Log> Errors { get; set; }
        public List<Log> Warnings { get; set; }

        public TestLogger()
        {
            Logs = new List<Log>();
            Errors = new List<Log>();
            Warnings = new List<Log>();
        }

        public static void Start(TestLogger logger)
        {
            Current = logger;
        }

        public override void Log(LoggingLevel level, string message, params object[] args)
        {
            Logs.Add(new Log(level, message, args));
        }

        public override void LogError(string message, params object[] args)
        {
            Errors.Add(new Log(LoggingLevel.Quiet, message, args));
        }

        public override void LogWarning(string message, params object[] args)
        {
            Warnings.Add(new Log(LoggingLevel.Quiet, message, args));
        }
    }

    public class Log
    {
        public LoggingLevel Level { get; set; }
        public string Message { get; set; }

        public Log(LoggingLevel level, string message, params object[] args)
        {
            Level = level;
            Message = string.Format(message, args);
        }
    }
}
