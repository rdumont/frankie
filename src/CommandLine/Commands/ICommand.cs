using System.Collections.Generic;
using System.IO;

namespace RDumont.Frankie.CommandLine.Commands
{
    public interface ICommand
    {
        string Name { get; }

        void ExecuteCommand(string[] args);
    }
}