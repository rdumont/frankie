using CommandLine;
using CommandLine.Text;

namespace RDumont.Frankie.CommandLine.Commands
{
    public class ServeOptions
    {
        [Option('b', "baseUrl", DefaultValue = "/", HelpText = "A base url where to serve the site")]
        public string BaseUrl { get; set; }

        [Option('p', "port", DefaultValue = 5000, HelpText = "The port where to serve this site")]
        public int Port { get; set; }

        [Option('w', "watch", DefaultValue = false, HelpText = "Whether file changes should be automatically detected")]
        public bool Watch { get; set; }

        [HelpOption('h', "help")]
        public string GetUsage()
        {
            var help = new HelpText
                {

                };
            help.AddOptions(this);
            return help;
        }
    }
}