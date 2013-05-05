using CommandLine;

namespace RDumont.Frankie.CommandLine.Commands
{
    public class WatchOptions : BaseOptions
    {
        public bool DontWaitForCommand { get; set; }
    }
}