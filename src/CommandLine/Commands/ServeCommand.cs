using System;
using System.Diagnostics;
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
            var path = GetAbsolutePath(options.Source).TrimEnd('\\').Replace(@"\", @"\\");

            var frankieRoot = typeof (ServeCommand).Assembly.Location;
            frankieRoot = frankieRoot.Substring(0, frankieRoot.LastIndexOf(Path.DirectorySeparatorChar));
            var serverExe = Path.Combine(frankieRoot, "tools", "onehttpd.exe");

            var process = new Process();
            var startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Normal,
                    FileName = serverExe,
                    Arguments = string.Format(" \"{0}\" -p {1} -l", path, options.Port),
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };
            process.StartInfo = startInfo;
            process.Start();

            while (Console.Read() != 'q') continue;

            if(!process.HasExited) process.Kill();
        }
    }
}