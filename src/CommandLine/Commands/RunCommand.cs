using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RDumont.Frankie.Core;

namespace RDumont.Frankie.CommandLine.Commands
{
    public class RunCommand : Command<RunOptions>
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

        public RunCommand(Generator generator)
        {
            this.generator = generator;
        }

        public override void ExecuteCommand(RunOptions options)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var root = options.LocationPath;
            var output = options.OutputPath;

            this.generator.Init(root, output);

            this.generator.CompileTemplates(root);

            // Transform posts

            var allEntries = FindAllEntries(root);
            foreach (var file in allEntries)
            {
                this.generator.AddFile(file);
            }
            sw.Stop();
            Logger.Current.Log(LoggingLevel.Debug, "Time: {0} ms", sw.ElapsedMilliseconds);
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
