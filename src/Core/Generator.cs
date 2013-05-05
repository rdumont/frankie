using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using RDumont.Frankie.Core.Handlers;
using Path = System.IO.Path;

namespace RDumont.Frankie.Core
{
    public class Generator
    {
        public string BasePath;
        public string SitePath;
        public string RelativeSitePath;
        private readonly SiteContext siteContext;
        private AssetHandlerManager _contentHandlers;

        public Io Io { get; set; }
        public SiteConfiguration Configuration { get; set; }

        protected Generator()
        {
        }

        public Generator(SiteContext siteContext)
        {
            this.Io = new Io();
            this.siteContext = siteContext;
        }
        
        public void Init(string locationPath, string outputPath)
        {
            this.BasePath = locationPath.TrimEnd(Path.DirectorySeparatorChar);
            this.SitePath = outputPath.TrimEnd(Path.DirectorySeparatorChar);
            this.RelativeSitePath = this.SitePath.Remove(0, this.BasePath.Length + 1);

            var configPath = Path.Combine(locationPath, "config.yaml");
            Configuration = SiteConfiguration.Load(configPath);
            Configuration.SitePath = this.SitePath;
            Configuration.SourcePath = this.BasePath;

            if (Configuration.Culture != null)
            {
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture
                    = CultureInfo.GetCultureInfo(Configuration.Culture);
            }

            TemplateManager.SetTemplateManager(new LiquidTemplateManager());

            TemplateManager.Current.Init(this.BasePath);

            _contentHandlers = new AssetHandlerManager(this);
        }

        public void CompileTemplates(string root)
        {
            var allFiles = Io.FindFilesRecursively(Path.Combine(this.BasePath, TemplateManager.TEMPLATES_FOLDER), "*.html");
            foreach (var file in allFiles)
            {
                CompileTemplate(GetRelativePath(file));
            }
        }

        public void LoadPosts(IEnumerable<string> files)
        {
            _contentHandlers.PostHandler.LoadAllPosts(files.Select(GetRelativePath), siteContext);
        }

        public void AddFile(string fullPath)
        {
            var path = GetRelativePath(fullPath);

            if (Configuration.IsExcluded(path))
                return;

            var handler = _contentHandlers.FindMatchingHandler(path);
            if (handler != null) handler.Handle(path);
        }

        public void RemoveFile(string fullPath)
        {
            var relativePath = GetRelativePath(fullPath);
            if (Configuration.IsExcluded(relativePath)) return;

            if (_contentHandlers.GeneratedContentHandler.Matches(fullPath)) return;

            var destination = GetFileDestinationPath(relativePath);
            try
            {
                Io.DeleteFile(destination);
                Logger.Current.Log(LoggingLevel.Debug, "Removed file: {0}", GetRelativePath(fullPath));
            }
            catch (FileNotFoundException)
            {
                // ok, probably a temp file
            }
        }

        public void ReAddDependentFile(string file)
        {
            var fullPath = Path.Combine(BasePath, file);
            if (file.StartsWith(TemplateManager.TEMPLATES_FOLDER))
            {
                CompileTemplate(file);
            }
            else if (_contentHandlers.PostHandler.Matches(file))
            {
                _contentHandlers.PostHandler.Handle(file);
            }
            else
            {
                AddFile(fullPath);
            }
        }

        public void CompileTemplate(string path)
        {
            var name = path.Remove(0, TemplateManager.TEMPLATES_FOLDER.Length + 1).Replace(".html", "");
            TemplateManager.Current.CompileTemplate(path);

            Logger.Current.Log(LoggingLevel.Debug, "Compiled template: {0}", name);
        }

        protected string GetFileDestinationPath(string relativePath)
        {
            if (relativePath.StartsWith(Post.POSTS_FOLDER))
            {
                var post = Post.FromFile(relativePath);
                var destinationPath = post.GetDestinationFilePath(this.Configuration);
                return Path.Combine(this.SitePath, destinationPath).Replace('\\', '/');
            }

            var destination = relativePath.Replace(this.BasePath, this.SitePath);

            if (destination.EndsWith(".md"))
                destination = destination.Replace(".md", ".html");

            return destination;
        }

        public void EnsureDirectoryExists(string fullPath)
        {
            var destinationFolder = Path.GetDirectoryName(fullPath);
            if (!Io.DirectoryExists(destinationFolder))
            {
                Io.CreateDirectory(destinationFolder);
            }
        }

        private string GetRelativePath(string fullPath)
        {
            return fullPath.Remove(0, BasePath.Length + 1);
        }

        public void UpdatePostsCollection()
        {
            _contentHandlers.PostHandler.UpdatePostsCollection(siteContext);
        }

        public void WriteAllPosts()
        {
            _contentHandlers.PostHandler.WriteAllPosts();
        }
    }
}