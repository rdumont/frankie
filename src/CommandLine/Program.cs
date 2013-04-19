using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using RDumont.Frankie.CommandLine.Commands;
using RDumont.Frankie.Core;

namespace RDumont.Frankie.CommandLine
{
    class Program
    {
        public const string Art = @"
   __________
   |  :  :  /
   |/¨´'`´¨|     Frankie v{version}
   |- () ()|     http://frankie.org
   | _____ <
  () \___/ |
    \___,_/
";

        private static readonly IList<ICommand> Commands = new List<ICommand>
            {
                new RunCommand(),
                new ServeCommand(),
                new WatchCommand(),
                new BootstrapCommand()
            };

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                WriteError("Please provide a command");
                Environment.Exit(1);
            }
                
            var command = Commands.FirstOrDefault(c => c.Name == args[0]);

            if (command == null)
            {
                WriteError("Command '{0}' is not recognized", args[0]);
                Environment.Exit(1);
            }

            var assembly = Assembly.GetExecutingAssembly();
            var version = FileVersionInfo.GetVersionInfo(assembly.Location);
            Console.WriteLine(Art.Replace("{version}", version.FileVersion));

            command.ExecuteCommand(args.Skip(1).ToArray());
        }

        private static void WriteError(string message, params object[] args)
        {
            WriteColoredLine(ConsoleColor.Red, message, args);
        }

        private static void WriteWarning(string message, params object[] args)
        {
            WriteColoredLine(ConsoleColor.Yellow, message, args);
        }

        private static void WriteColoredLine(ConsoleColor foregroundColor, string message, params object[] args)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = foregroundColor;
            Console.WriteLine(message, args);
            Console.ForegroundColor = oldColor;
        }
    }
}
