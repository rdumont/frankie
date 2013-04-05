using System;
using System.IO;

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
            var path = GetAbsolutePath(options.Source);
            this.generator.Init(path);

            var watcher = new FileSystemWatcher(path);
            watcher.IncludeSubdirectories = true;
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size;

            watcher.Created += OnFileCreated;
            watcher.Changed += OnFileChanged;
            watcher.Deleted += OnFileDeleted;
            watcher.Renamed += OnFileRenamed;

            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Monitoring file changes in " + path);
            while (Console.Read() != 'q') continue;
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("DELETED: " + e.FullPath);
            this.generator.RemoveFile(e.FullPath);
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("CREATED: " + e.FullPath);
            this.generator.AddFile(e.FullPath);
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("CHANGED: " + e.FullPath);
            this.generator.RemoveFile(e.FullPath);
            this.generator.AddFile(e.FullPath);
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine("RENAMED: " + e.FullPath);
            this.generator.RemoveFile(e.OldFullPath);
            this.generator.AddFile(e.FullPath);
        }
    }
}