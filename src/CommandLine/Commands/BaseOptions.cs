using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using CommandLine;
using CommandLine.Text;

namespace RDumont.Frankie.CommandLine.Commands
{
    public class BaseOptions
    {
        [Option('o', "output", DefaultValue = ".\\_site\\", HelpText = "Where the output should be placed")]
        public string Output { get; set; }

        [Option('s', "source", DefaultValue = ".\\", HelpText = "Where the site's source resides")]
        public string Source { get; set; }

        [Option('v', "verbosity", DefaultValue = "quiet", HelpText = "Verbosity level (debug, info, minimal or quiet)")]
        public string Verbosity { get; set; }

        [Option("profile", HelpText = "Set to enable profiling times")]
        public bool Profile { get; set; }

        public string OutputPath
        {
            get { return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), this.Output)); }
        }

        public string SourcePath
        {
            get { return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), this.Source)); }
        }

        [HelpOption('h', "help")]
        public virtual string GetUsage()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = FileVersionInfo.GetVersionInfo(assembly.Location);

            var help = new HelpText
                {
                    Heading = @"
   __________
   |  :  :  /
   |/¨´'`´¨|     Frankie v{version}
   |- () ()|     http://frankie.org
   | _____ <
  () \___/ |
    \___,_/".Replace("{version}", version.FileVersion),
                    AddDashesToOption = true
                };
            help.AddOptions(this);
            return help;
        }
    }
}