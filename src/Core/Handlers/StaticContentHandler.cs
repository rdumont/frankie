using System.IO;

namespace RDumont.Frankie.Core.Handlers
{
    public class StaticContentHandler : IAssetHandler
    {
        private readonly Generator _generator;

        public StaticContentHandler(Generator generator)
        {
            _generator = generator;
        }

        public bool Matches(string path)
        {
            return true;
        }

        public void Handle(string path)
        {
            var destination = Path.Combine(_generator.SitePath, path);
            try
            {
                _generator.Io.CopyFile(Path.Combine(_generator.BasePath, path), destination, true);
                Logger.Current.Log(LoggingLevel.Debug, "Asset: {0}", path);
            }
            catch (FileNotFoundException)
            {
                // ok, probably a temp file
            }
        }
    }
}