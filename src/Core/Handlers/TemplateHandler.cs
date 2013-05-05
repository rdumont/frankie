using System.IO;
using System.Linq;

namespace RDumont.Frankie.Core.Handlers
{
    public class TemplateHandler : IAssetHandler
    {
        private readonly SiteConfiguration _configuration;
        private readonly Io _io;
        private readonly AssetHandlerManager _handlers;

        public TemplateHandler(SiteConfiguration configuration, Io io, AssetHandlerManager handlers)
        {
            _configuration = configuration;
            _handlers = handlers;
            _io = io;
        }

        public bool Matches(string path)
        {
            return path.StartsWith(TemplateManager.TEMPLATES_FOLDER);
        }

        public void Handle(string path)
        {
            var dependentFiles = DependencyTracker.Current.FindAllDependentFiles(path);

            try
            {
                CompileTemplate(path);
            }
            catch (FileNotFoundException)
            {
                // ok, probably a temp file
            }

            foreach (var dependentFile in dependentFiles)
            {
                _handlers.Handle(dependentFile);
            }

            _handlers.PostHandler.UpdatePostsCollection();
        }

        public void HandleRemoval(string path)
        {
            TemplateManager.Current.RemoveTemplate(path);
        }

        public void CompileAllTemplates()
        {
            var templatesPath = Path.Combine(_configuration.SourcePath, TemplateManager.TEMPLATES_FOLDER);
            var allFiles = _io.FindFilesRecursively(templatesPath, "*.html");

            var paths = allFiles.Select(path => _configuration.GetRelativePath(path));
            foreach (var path in paths)
                CompileTemplate(path);
        }

        public void CompileTemplate(string path)
        {
            var name = GetTemplateName(path);
            TemplateManager.Current.CompileTemplate(path);

            Logger.Current.Log(LoggingLevel.Debug, "Compiled template: {0}", name);
        }

        private static string GetTemplateName(string path)
        {
            var name = path.Remove(0, TemplateManager.TEMPLATES_FOLDER.Length + 1).Replace(".html", "");
            return name;
        }
    }
}
