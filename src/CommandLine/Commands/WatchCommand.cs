using System;
using System.IO;
using RDumont.Frankie.Core;

namespace RDumont.Frankie.CommandLine.Commands
{
    public class WatchCommand : RunnableAbstractCommand<WatchOptions>
    {
        private FileSystemWatcher _watcher;

        public override string Name
        {
            get { return "watch"; }
        }

        public bool IsIdle { get; private set; }

        public override void ExecuteCommand(WatchOptions options)
        {
            RunTransformation(options);

            var path = GetAbsolutePath(options.Source);

            _watcher = new FileSystemWatcher(path);
            _watcher.IncludeSubdirectories = true;
            _watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size;

            _watcher.Created += OnFileCreated;
            _watcher.Changed += OnFileChanged;
            _watcher.Deleted += OnFileDeleted;
            _watcher.Renamed += OnFileRenamed;

            _watcher.EnableRaisingEvents = true;

            Logger.Current.Log(LoggingLevel.Minimal, "\nNow monitoring file changes in {0}", path);

            if (options.DontWaitForCommand) return;
            while (Console.Read() != 'q') continue;
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            RemoveFile(e.FullPath);
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            AddFile(e.FullPath);
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            AddFile(e.FullPath);
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            RemoveFile(e.OldFullPath);
            AddFile(e.FullPath);
        }

        private void RemoveFile(string fullPath)
        {
            IsIdle = false;
            var path = Configuration.GetRelativePath(fullPath);
            try
            {
                Handlers.HandleRemoval(path);
            }
            catch (Exception exception)
            {
                Logger.Current.LogError("An exception has occured while removing \"{0}\".\n\n" + exception, path);
            }
            finally
            {
                IsIdle = true;
            }
        }

        private void AddFile(string fullPath)
        {
            IsIdle = false;
            var path = Configuration.GetRelativePath(fullPath);
            try
            {
                Handlers.Handle(path);
            }
            catch (Exception exception)
            {
                Logger.Current.LogError("An exception has occured while removing \"{0}\".\n\n" + exception, path);
            }
            finally
            {
                IsIdle = true;
            }
        }

        public void Stop()
        {
            _watcher.EnableRaisingEvents = false;
        }
    }
}