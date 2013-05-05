using System.IO;

namespace RDumont.Frankie.Core.Handlers
{
    public class TransformableContentHandler : IAssetHandler
    {
        private readonly Generator _generator;

        public TransformableContentHandler(Generator generator)
        {
            _generator = generator;
        }

        public bool Matches(string path)
        {
            return path.EndsWith(".html");
        }

        public void Handle(string path)
        {
            var destination = Path.Combine(_generator.SitePath, path);
            _generator.EnsureDirectoryExists(destination);

            var model = new Page(Path.Combine(_generator.BasePath, path));
            model.LoadFile(_generator.Configuration);

            var result = TemplateManager.Current.RenderPage(path, model);

            _generator.Io.WriteFile(destination, result);
            Logger.Current.Log(LoggingLevel.Debug, "HTML page: {0}", path);
        }
    }
}