using System.IO;
using CommandLine;

namespace RDumont.Frankie.CommandLine.Commands
{
    public class BaseOptions
    {
        [Option('o', "output", DefaultValue = ".\\_site\\")]
        public string Output { get; set; }

        [Option('l', "location", DefaultValue = ".\\")]
        public string Location { get; set; }

        public string OutputPath
        {
            get { return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), this.Output)); }
        }

        public string LocationPath
        {
            get { return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), this.Location)); }
        }
    }
}