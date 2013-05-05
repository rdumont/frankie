using System;
using System.IO;
using CommandLine;
using RDumont.Frankie.Core;

namespace RDumont.Frankie.CommandLine.Commands
{
    public abstract class Command<TOptions> : ICommand
        where TOptions : BaseOptions
    {
        public abstract string Name { get; }

        public abstract void ExecuteCommand(TOptions options);

        public void ExecuteCommand(string[] args)
        {
            var options = Activator.CreateInstance<TOptions>();
            var commandLineParser = new Parser(settings =>
                {
                    settings.HelpWriter = Console.Out;
                    settings.CaseSensitive = true;
                });

            if (!commandLineParser.ParseArguments(args, options))
            {
                Environment.Exit(1);
            }

            var verbosity = ParseVerbosityLevel(options.Verbosity);
            Logger.Start(verbosity);

            ExecuteCommand(options);
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
                    break;
            }
        }

        public string GetAbsolutePath(string relativePath)
        {
            var current = Directory.GetCurrentDirectory();
            var actual = Path.Combine(current, relativePath);
            return Path.GetFullPath(actual);
        }
    }
}