using System;
using System.IO;
using RDumont.Frankie.Core;

namespace RDumont.Frankie.CommandLine.Commands
{
    public class WatchCommand : Command<WatchOptions>
    {
        private readonly Generator generator;

        public override string Name
        {
            get { return "watch"; }
        }

        public WatchCommand(Generator generator)
        {
            this.generator = generator;
        }

        public override void ExecuteCommand(WatchOptions options)
        {
            var runCommand = new RunCommand(this.generator);
            runCommand.ExecuteCommand(options);

            var path = GetAbsolutePath(options.Location);

            var watcher = new FileSystemWatcher(path);
            watcher.IncludeSubdirectories = true;
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size;

            watcher.Created += OnFileCreated;
            watcher.Changed += OnFileChanged;
            watcher.Deleted += OnFileDeleted;
            watcher.Renamed += OnFileRenamed;

            watcher.EnableRaisingEvents = true;

            Logger.Current.Log(LoggingLevel.Minimal, "\nNow monitoring file changes in {0}", path);
            while (Console.Read() != 'q') continue;
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            this.generator.RemoveFile(e.FullPath);
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            this.generator.AddFile(e.FullPath);
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            this.generator.AddFile(e.FullPath);
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            this.generator.RemoveFile(e.OldFullPath);
            this.generator.AddFile(e.FullPath);
        }
    }
}