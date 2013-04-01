using System.Collections.Generic;
using System.IO;

namespace RDumont.Frankie.CommandLine.Commands
{
    public interface ICommand
    {
        void ExecuteCommand(IEnumerable<string> args, TextWriter output);
    }
}