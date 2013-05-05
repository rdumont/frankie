using System.IO;

namespace RDumont.Frankie.Core.Handlers
{
    public class TransformableContentHandler : IAssetHandler
    {
        private readonly SiteConfiguration _configuration;
        private readonly Io _io;

        public TransformableContentHandler(SiteConfiguration configuration, Io io)
        {
            _configuration = configuration;
            _io = io;
        }

        public bool Matches(string path)
        {
            return path.EndsWith(".html");
        }

        public void Handle(string path)
        {
            var finalPath = GetFinalPath(path);
            _io.EnsureDirectoryExists(finalPath);

            var model = new Page(Path.Combine(_configuration.SourcePath, path));
            model.LoadFile(_configuration);

            var result = TemplateManager.Current.RenderPage(path, model);

            _io.WriteFile(finalPath, result);
            Logger.Current.Log(LoggingLevel.Debug, "HTML page: {0}", path);
        }

        public void HandleRemoval(string path)
        {
            var finalPath = GetFinalPath(path);
            _io.DeleteFile(finalPath);
            DependencyTracker.Current.Remove(path);
        }

        private string GetFinalPath(string path)
        {
            return Path.Combine(_configuration.SitePath, path);
        }
    }
}