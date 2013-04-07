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

        public void Init(string path)
        {
            this.basePath = path;
            this.postsPath = Path.Combine(path, "_posts");
            this.templatesPath = Path.Combine(path, "_templates");
            this.sitePath = Path.Combine(path, "_site");

            var configPath = Path.Combine(path, "config.yaml");
            this.Configuration = SiteConfiguration.Load(configPath);
        }

        public void RemoveFile(string fullPath)
        {
        }

        public void AddFile(string fullPath)
        {
        }
    }
}