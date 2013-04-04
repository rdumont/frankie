using System;
using System.IO;

namespace RDumont.Frankie.CommandLine.Commands
{
    public class ServeCommand : Command<ServeOptions>
    {
        public override string Name
        {
            get { return "serve"; }
        }

        public override void ExecuteCommand(ServeOptions options)
        {
            throw new NotImplementedException();
        }
    }
}