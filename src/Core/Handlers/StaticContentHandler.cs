using System.IO;

namespace RDumont.Frankie.Core.Handlers
{
    public class StaticContentHandler : IAssetHandler
    {
        private readonly SiteConfiguration _configuration;
        private readonly Io _io;

        public StaticContentHandler(SiteConfiguration configuration, Io io)
        {
            _configuration = configuration;
            _io = io;
        }

        public bool Matches(string path)
        {
            return true;
        }

        public void Handle(string path)
        {
            var finalPath = GetFinalPath(path);
            try
            {
                _io.CopyFile(Path.Combine(_configuration.SourcePath, path), finalPath, true);
                Logger.Current.Log(LoggingLevel.Debug, "Asset: {0}", path);
            }
            catch (FileNotFoundException)
            {
                // ok, probably a temp file
            }
        }

        public void HandleRemoval(string path)
        {
            var finalPath = GetFinalPath(path);
            _io.DeleteFile(finalPath);
        }

        private string GetFinalPath(string path)
        {
            var destination = Path.Combine(_configuration.SitePath, path);
            return destination;
        }
    }
}