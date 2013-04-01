using System;
using System.Collections.Generic;
using System.IO;

namespace RDumont.Frankie.CommandLine.Commands
{
    public abstract class Command<TOptions> : ICommand
    {
        public void ExecuteCommand(IEnumerable<string> args, TextWriter output)
        {
            var options = Activator.CreateInstance<TOptions>();

            ExecuteCommand(options, output);
        }

        public abstract void ExecuteCommand(TOptions options, TextWriter output);
    }
}