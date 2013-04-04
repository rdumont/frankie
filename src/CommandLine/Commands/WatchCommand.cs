using System;
using System.IO;

namespace RDumont.Frankie.CommandLine.Commands
{
    public class WatchCommand : Command<WatchOptions>
    {
        public override string Name
        {
            get { return "watch"; }
        }

        public override void ExecuteCommand(WatchOptions options)
        {
            throw new NotImplementedException();
        }
    }
}