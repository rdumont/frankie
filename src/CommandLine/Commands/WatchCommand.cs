using System;
using System.IO;
using RDumont.Frankie.Core;

namespace RDumont.Frankie.CommandLine.Commands
{
    public class WatchCommand : RunnableAbstractCommand<WatchOptions>
    {
        public override string Name
        {
            get { return "watch"; }
        }

        public WatchCommand() : this(new Generator(SiteContext.Current))
        {
        }

        public WatchCommand(Generator generator) : base(generator)
        {
        }

        public override void ExecuteCommand(WatchOptions options)
        {
            RunTransformation(options);

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
            this.Generator.RemoveFile(e.FullPath);
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            this.Generator.AddFile(e.FullPath);
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            this.Generator.AddFile(e.FullPath);
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            this.Generator.RemoveFile(e.OldFullPath);
            this.Generator.AddFile(e.FullPath);
        }
    }
}