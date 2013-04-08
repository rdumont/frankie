using System.IO;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;

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


            Razor.SetTemplateService(new TemplateService(new TemplateServiceConfiguration
            {
                BaseTemplateType = typeof(PageTemplate<>)
            }));
        }

        public void CompileTemplates(string root)
        {
            var templatesFolder = Path.Combine(root, "_templates");
            var allFiles = Directory.GetFiles(templatesFolder, "*.cshtml", SearchOption.AllDirectories);
            foreach (var file in allFiles)
            {
                var name = file.Remove(0, templatesFolder.Length + 1).Replace(".cshtml", "");
                var contents = File.ReadAllText(file);
                Razor.Compile(contents, name);

                Logger.Current.Log(LoggingLevel.Debug, "Compiled template: {0}", name);
            }
        }

        public void RemoveFile(string fullPath)
        {
        }

        public void AddFile(string fullPath)
        {
            Logger.Current.Log(LoggingLevel.Debug, "Adding file: {0}", fullPath);
            if (fullPath.EndsWith(".cshtml"))
            {
                var contents = File.ReadAllText(fullPath, System.Text.Encoding.UTF8);

                var model = new Page {Foo = "bar"};
                var result = Razor.Parse(contents, model);

                var finalPath = fullPath.Replace(basePath, sitePath).Replace(".cshtml", ".html");
                File.WriteAllText(finalPath, result, System.Text.Encoding.UTF8);
            }
        }
    }
}