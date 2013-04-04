using System;
using System.IO;
using CommandLine;

namespace RDumont.Frankie.CommandLine.Commands
{
    public abstract class Command<TOptions> : ICommand
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

            ExecuteCommand(options);
        }

        public string GetAbsolutePath(string relativePath)
        {
            var current = Directory.GetCurrentDirectory();
            var actual = Path.Combine(current, relativePath);
            return Path.GetFullPath(actual);
        }
    }
}