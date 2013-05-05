using System;
using System.IO;

namespace RDumont.Frankie.Core.Handlers
{
    public class MarkdownPageHandler : IAssetHandler
    {
        private readonly Generator _generator;

        public MarkdownPageHandler(Generator generator)
        {
            _generator = generator;
        }

        public bool Matches(string path)
        {
            return (path.EndsWith(".md") || path.EndsWith(".markdown"))
                && !path.StartsWith(Post.POSTS_FOLDER);
        }

        public void Handle(string path)
        {
            var page = new Page(Path.Combine(_generator.BasePath, path));
            var destination = Path.Combine(_generator.SitePath, path.Replace(".md", ".html"));
            _generator.EnsureDirectoryExists(destination);
            page.LoadFile(_generator.Configuration);

            var template = page.Metadata["template"] ?? "_page";

            try
            {
                page.Body = TemplateManager.Current.RenderMarkdownPage(path, template, page);
            }
            catch (InvalidOperationException exception)
            {
                if (!exception.Message.StartsWith("No template exists")) throw;

                Logger.Current.LogError("{0}\n  No template exists with name '{1}'",
                    path, template);
            }

            _generator.Io.WriteFile(destination, page.Body);
            Logger.Current.Log(LoggingLevel.Debug, "Markdown page: {0}", path);
        }
    }
}