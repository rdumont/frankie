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
            var path = GetAbsolutePath(options.Source);

            var process = new Process();
            var startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "tools\\onehttpd.exe",
                    Arguments = string.Format("{0} -p {1} -l", path, options.Port),
                };
            process.StartInfo = startInfo;
            process.Start();

            Console.WriteLine("Press return to stop server...");
            Console.ReadLine();

            if(!process.HasExited) process.Kill();
        }
    }
}