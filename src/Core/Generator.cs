using System.Collections.Generic;
using System.IO;
using System.Linq;
using RDumont.Frankie.Core.Handlers;
using Path = System.IO.Path;

namespace RDumont.Frankie.Core
{
    public class Generator
    {
        private readonly SiteContext siteContext;
        private AssetHandlerManager _handlers;

        public Io Io { get; set; }
        public SiteConfiguration Configuration { get; set; }

        protected Generator()
        {
        }

        public Generator(SiteContext siteContext)
        {
            this.siteContext = siteContext;
        }
        
        public void Init(AssetHandlerManager handlers, SiteConfiguration configuration, Io io)
        {
            _handlers = handlers;
            Configuration = configuration;
            Io = io;

            TemplateManager.SetTemplateManager(new LiquidTemplateManager());
            TemplateManager.Current.Init(configuration.SourcePath);
        }

        public void CompileTemplates()
        {
            _handlers.TemplateHandler.CompileAllTemplates();
        }

        public void LoadPosts(IEnumerable<string> files)
        {
            _handlers.PostHandler.LoadAllPosts(files.Select(GetRelativePath), siteContext);
        }

        public void AddFile(string fullPath)
        {
            var path = GetRelativePath(fullPath);

            _handlers.Handle(path);
        }

        public void RemoveFile(string fullPath)
        {
            var relativePath = GetRelativePath(fullPath);
            if (Configuration.IsExcluded(relativePath)) return;

            if (_handlers.GeneratedContentHandler.Matches(fullPath)) return;

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

        protected string GetFileDestinationPath(string relativePath)
        {
            if (relativePath.StartsWith(Post.POSTS_FOLDER))
            {
                var post = Post.FromFile(relativePath);
                var destinationPath = post.GetDestinationFilePath(this.Configuration);
                return Path.Combine(Configuration.SitePath, destinationPath).Replace('\\', '/');
            }

            var destination = relativePath.Replace(Configuration.SourcePath, Configuration.SitePath);

            if (destination.EndsWith(".md"))
                destination = destination.Replace(".md", ".html");

            return destination;
        }

        private string GetRelativePath(string fullPath)
        {
            return fullPath.Remove(0, Configuration.SourcePath.Length + 1);
        }

        public void UpdatePostsCollection()
        {
            _handlers.PostHandler.UpdatePostsCollection(siteContext);
        }

        public void WriteAllPosts()
        {
            _handlers.PostHandler.WriteAllPosts();
        }
    }
}