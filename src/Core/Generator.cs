using System.IO;
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

        public void RemoveFile(string path)
        {
            if (Configuration.IsExcluded(path)) return;

            if (_handlers.GeneratedContentHandler.Matches(path)) return;

            var destination = GetFileDestinationPath(path);
            try
            {
                Io.DeleteFile(destination);
                Logger.Current.Log(LoggingLevel.Debug, "Removed file: {0}", path);
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
    }
}