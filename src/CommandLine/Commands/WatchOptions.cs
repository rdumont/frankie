using CommandLine;

namespace RDumont.Frankie.CommandLine.Commands
{
    public class WatchOptions
    {
        [Option('s', "source", DefaultValue = @".\", HelpText = "The path from where to serve files")]
        public string Source { get; set; }
    }
}