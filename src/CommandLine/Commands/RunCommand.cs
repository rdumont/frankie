using System;
using System.IO;

namespace RDumont.Frankie.CommandLine.Commands
{
    public class RunCommand : Command<RunOptions>
    {
        public override string Name
        {
            get { return "run"; }
        }

        public override void ExecuteCommand(RunOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
