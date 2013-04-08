using System;
using System.Collections.Generic;
using System.IO;

namespace RDumont.Frankie.Core
{
    public class Generator
    {
        private string basePath;
        private string postsPath;
        private string templatesPath;
        private string sitePath;

        protected SiteConfiguration Configuration { get; set; }

        public void Init(string locationPath, string outputPath)
        {
            this.basePath = locationPath;
            this.postsPath = Path.Combine(locationPath, "_posts");
            this.templatesPath = Path.Combine(locationPath, "_templates");
            this.sitePath = outputPath;

            var configPath = Path.Combine(locationPath, "config.yaml");
            this.Configuration = SiteConfiguration.Load(configPath);
        }

        public void CompileTemplates(string root)
        {
            var templatesFolder = Path.Combine(root, "_templates");
            var allFiles = Directory.GetFiles(templatesFolder, "*.cshtml", SearchOption.AllDirectories);
            foreach (var file in allFiles)
            {
                var name = file.Remove(0, root.Length + 1).Replace(Path.DirectorySeparatorChar, '|');
                var contents = File.ReadAllText(file);
                RazorEngine.Razor.Compile(contents, name);

                Logger.Current.Log(LoggingLevel.Debug, "Compiled template: {0}", name);
            }
        }

        public void CompilePages(string root, IEnumerable<string> pages)
        {
            foreach (var file in pages)
            {
                var name = file.Remove(0, root.Length + 1).Replace(Path.DirectorySeparatorChar, '|'); ;
                var contents = File.ReadAllText(file);
                RazorEngine.Razor.Compile(contents, name);

                Logger.Current.Log(LoggingLevel.Debug, "Compiled page: {0}", name);
            }
        }

        public void RemoveFile(string fullPath)
        {
        }

        public void AddFile(string fullPath)
        {
        }
    }
}