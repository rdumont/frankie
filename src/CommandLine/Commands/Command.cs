using System;
using System.Diagnostics;
using System.IO;
using CommandLine;
using RDumont.Frankie.Core;

namespace RDumont.Frankie.CommandLine.Commands
{
    public abstract class Command<TOptions> : ICommand
        where TOptions : BaseOptions
    {
        protected TOptions Options { get; private set; }

        public abstract string Name { get; }

        public abstract void ExecuteCommand(TOptions options);

        public void ExecuteCommand(string[] args)
        {
            Options = Activator.CreateInstance<TOptions>();
            var commandLineParser = new Parser(settings =>
                {
                    settings.HelpWriter = Console.Out;
                    settings.CaseSensitive = true;
                });

            if (!commandLineParser.ParseArguments(args, Options))
            {
                Environment.Exit(1);
            }

            var verbosity = ParseVerbosityLevel(Options.Verbosity);
            Logger.Start(verbosity);

            ExecuteCommand(Options);
        }

        private LoggingLevel ParseVerbosityLevel(string argument)
        {
            switch (argument)
            {
                case "d":
                case "debug":
                    return LoggingLevel.Debug;

                case "i":
                case "info":
                    return LoggingLevel.Info;

                case "m":
                case "minimal":
                    return LoggingLevel.Minimal;
                
                case "q":
                case "quiet":
                    return LoggingLevel.Quiet;

                default:
                    Logger.Current.LogError("'{0}' is not a valid verbosity level");
                    Environment.Exit(1);
                    return default(LoggingLevel);
            }
        }

        public string GetAbsolutePath(string relativePath)
        {
            var current = Directory.GetCurrentDirectory();
            var actual = Path.Combine(current, relativePath);
            return Path.GetFullPath(actual);
        }

        protected void Profile(string message, Action action)
        {
            if (Options != null && Options.Profile)
            {
                var stopWatch = Stopwatch.StartNew();
                action();
                stopWatch.Stop();
                Console.WriteLine("{0}: {1}ms", message, stopWatch.ElapsedMilliseconds);
            }
            else
            {
                action();
            }
        }
    }
}