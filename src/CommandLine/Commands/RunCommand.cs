using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RDumont.Frankie.Core;

namespace RDumont.Frankie.CommandLine.Commands
{
    public class RunCommand : Command<BaseOptions>
    {
        private readonly Generator generator;

        private readonly string[] exclude = new[]
            {
                Post.POSTS_FOLDER,
                "_site",
                "_templates",
                ".gitignore",
                ".hgignore",
                "config.yaml"
            };

        public override string Name
        {
            get { return "run"; }
        }

        public RunCommand() : this(new Generator(SiteContext.Current))
        {
        }

        public RunCommand(Generator generator)
        {
            this.generator = generator;
        }

        public override void ExecuteCommand(BaseOptions options)
        {
            Logger.Current.Log(LoggingLevel.Minimal, "Running Frankie...");
            Logger.Current.Log(LoggingLevel.Minimal, "Source: {0}", options.LocationPath);
            Logger.Current.Log(LoggingLevel.Minimal, "Target: {0}\n", options.OutputPath);

            var sw = System.Diagnostics.Stopwatch.StartNew();
            var root = options.LocationPath;
            var output = options.OutputPath;

            this.generator.Init(root, output);

            this.CleanDirectory(output);

            this.generator.CompileTemplates(root);

            var postFiles = FindPostPaths(root);
            this.generator.LoadPosts(postFiles);
            this.generator.WriteAllPosts();

            var allEntries = FindAllEntries(root);
            foreach (var file in allEntries)
            {
                this.generator.AddFile(file);
            }
            sw.Stop();

            Logger.Current.Log(LoggingLevel.Minimal, "\nFINISHED! Took {0}ms", sw.ElapsedMilliseconds);
        }

        private void CleanDirectory(string path)
        {
            var folders = Directory.GetDirectories(path);
            foreach (var folder in folders)
                Directory.Delete(folder, true);

            var files = Directory.GetFiles(path).Where(f => !f.EndsWith(".gitignore"));
            foreach (var file in files)
                File.Delete(file);
        }

        private IEnumerable<string> FindPostPaths(string root)
        {
            var folder = Path.Combine(root, Post.POSTS_FOLDER);
            return Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories);
        }

        private IEnumerable<string> FindAllEntries(string root)
        {
            var excludedEndings = this.exclude.Select(e => Path.DirectorySeparatorChar + e).ToArray();

            var topLevelEntries = Directory.EnumerateFileSystemEntries(root)
                .Where(entry => !excludedEndings.Any(entry.EndsWith));

            var allEntries = new List<String>();
            foreach (var entry in topLevelEntries)
            {
                if (Directory.Exists(entry))
                    allEntries.AddRange(FindEntries(entry));
                else
                    allEntries.Add(entry);
            }
            return allEntries;
        }

        private static IEnumerable<string> FindEntries(string root)
        {
            var result = new List<string>();
            result.AddRange(Directory.GetFiles(root));

            foreach (var folder in Directory.GetDirectories(root))
                result.AddRange(FindEntries(folder));

            return result;
        }
    }
}
