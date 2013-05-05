using System.IO;
using CommandLine;

namespace RDumont.Frankie.CommandLine.Commands
{
    public class BaseOptions
    {
        [Option('o', "output", DefaultValue = ".\\_site\\")]
        public string Output { get; set; }

        [Option('s', "source", DefaultValue = ".\\")]
        public string Source { get; set; }

        [Option('v', "verbosity", DefaultValue = "quiet")]
        public string Verbosity { get; set; }

        [Option("profile")]
        public bool Profile { get; set; }

        public string OutputPath
        {
            get { return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), this.Output)); }
        }

        public string SourcePath
        {
            get { return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), this.Source)); }
        }
    }
}