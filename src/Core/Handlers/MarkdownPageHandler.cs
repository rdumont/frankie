using System;
using System.IO;

namespace RDumont.Frankie.Core.Handlers
{
    public class MarkdownPageHandler : IAssetHandler
    {
        private readonly SiteConfiguration _configuration;
        private readonly Io _io;

        public MarkdownPageHandler(SiteConfiguration configuration, Io io)
        {
            _configuration = configuration;
            _io = io;
        }

        public bool Matches(string path)
        {
            return (path.EndsWith(".md") || path.EndsWith(".markdown"))
                && !path.StartsWith(Post.POSTS_FOLDER);
        }

        public void Handle(string path)
        {
            var page = new Page(Path.Combine(_configuration.SourcePath, path));
            var destination = Path.Combine(_configuration.SitePath, path.Replace(".md", ".html"));
            _io.EnsureDirectoryExists(destination);
            page.LoadFile(_configuration);

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

            _io.WriteFile(destination, page.Body);
            Logger.Current.Log(LoggingLevel.Debug, "Markdown page: {0}", path);
        }
    }
}