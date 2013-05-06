using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using RDumont.Frankie.Core;
using RDumont.Frankie.Core.Handlers;

namespace RDumont.Frankie.CommandLine.Commands
{
    public abstract class RunnableAbstractCommand<T> : Command<T>
        where T : BaseOptions
    {
        private readonly string[] exclude = new[]
            {
                Post.POSTS_FOLDER,
                "_site",
                "_templates",
                "config.yaml"
            };

        protected SiteConfiguration Configuration { get; private set; }
        protected Io Io { get; private set; }
        protected AssetHandlerManager Handlers { get; private set; }

        protected RunnableAbstractCommand()
        {
            this.Io = new Io();
        }

        protected void RunTransformation(BaseOptions options)
        {
            var root = options.SourcePath;
            var output = options.OutputPath;

            Profile("Initialization", () =>
                {
                    Configuration = LoadConfiguration(options);
                    Handlers = new AssetHandlerManager(Configuration, Io);

                    this.CleanDirectory(output);

                    TemplateManager.SetTemplateManager(new LiquidTemplateManager());
                    TemplateManager.Current.Init(Configuration);
                });

            Profile("Templates compilation", () =>
                Handlers.TemplateHandler.CompileAllTemplates());

            Profile("Posts transformation", () =>
                {
                    var postFiles = FindPostPaths(root).Select(Configuration.GetRelativePath);
                    Handlers.PostHandler.LoadAllPosts(postFiles, SiteContext.Current);
                    Handlers.PostHandler.WriteAllPosts();
                });

            Profile("Content handling", () =>
                FindAllEntries(root)
                    .Select(Configuration.GetRelativePath).AsParallel()
                    .ForAll(Handlers.Handle));
        }

        public static SiteConfiguration LoadConfiguration(BaseOptions options)
        {
            var configurationFilePath = Path.Combine(options.SourcePath, "config.yaml");
            var configuration = SiteConfiguration.Load(configurationFilePath);
            configuration.SourcePath = options.SourcePath.TrimEnd(Path.DirectorySeparatorChar);
            configuration.SitePath = options.OutputPath.TrimEnd(Path.DirectorySeparatorChar);

            if (configuration.Culture != null)
            {
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture
                    = CultureInfo.GetCultureInfo(configuration.Culture);
            }

            return configuration;
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
            return FindAllEntries(folder);
        }

        private IEnumerable<string> FindAllEntries(string root)
        {
            var excludedEndings = this.exclude.Select(e => Path.DirectorySeparatorChar + e).ToArray();

            var topLevelEntries = Directory.EnumerateFileSystemEntries(root)
                .Where(entry => !excludedEndings.Any(entry.EndsWith));

            var allEntries = new List<string>();
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