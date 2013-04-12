using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RDumont.Frankie.Core
{
    public class Generator
    {
        private string basePath;
        private string postsPath;
        private string templatesPath;
        private string sitePath;
        private List<Post> posts;
        private readonly SiteContext siteContext;

        protected SiteConfiguration Configuration { get; set; }

        public Generator() : this(SiteContext.Current)
        {
            
        }

        private Generator(SiteContext siteContext)
        {
            this.siteContext = siteContext;
        }

        public void Init(string locationPath, string outputPath)
        {
            this.basePath = locationPath.TrimEnd(Path.DirectorySeparatorChar);
            this.postsPath = Path.Combine(locationPath, "_posts");
            this.templatesPath = Path.Combine(locationPath, "_templates");
            this.sitePath = outputPath.TrimEnd(Path.DirectorySeparatorChar);

            var configPath = Path.Combine(locationPath, "config.yaml");
            this.Configuration = SiteConfiguration.Load(configPath);

            TemplateManager.Init();

            this.posts = new List<Post>();
        }

        public void CompileTemplates(string root)
        {
            var allFiles = Directory.GetFiles(templatesPath, "*.cshtml", SearchOption.AllDirectories);
            foreach (var file in allFiles)
            {
                var name = file.Remove(0, templatesPath.Length + 1).Replace(".cshtml", "");
                var contents = File.ReadAllText(file);
                TemplateManager.CompileTemplate(file.Remove(0, basePath.Length + 1), contents);

                Logger.Current.Log(LoggingLevel.Debug, "Compiled template: {0}", name);
            }
        }

        public void RemoveFile(string fullPath)
        {
        }

        public void AddFile(string fullPath)
        {
            if (IsIgnored(fullPath)) return;

            if (!IsSiteContent(fullPath)) return;

            var destination = GetFileDestinationPath(fullPath);
            var relativeOrigin = fullPath.Remove(0, this.basePath.Length + 1);

            if (fullPath.EndsWith(".cshtml"))
            {
                HandleRazorPage(fullPath, destination);
                Logger.Current.Log(LoggingLevel.Debug, "Razor: {0}", relativeOrigin);
            }
            else if (fullPath.EndsWith(".markdown") || fullPath.EndsWith(".md"))
            {
                HandleMarkdownPage(fullPath, destination);
                Logger.Current.Log(LoggingLevel.Debug, "Markdown: {0}", relativeOrigin);
            }
            else
            {
                File.Copy(fullPath, destination, true);
                Logger.Current.Log(LoggingLevel.Debug, "Content: {0}", relativeOrigin);
            }
        }

        private void HandleMarkdownPage(string originPath, string destinationPath)
        {
            // TODO
        }

        private void HandleRazorPage(string originPath, string destinationPath)
        {
            var contents = File.ReadAllText(originPath, System.Text.Encoding.UTF8);

            var model = new Page();
            var result = TemplateManager.RenderPage(originPath.Remove(0, basePath.Length + 1), contents, model);

            var finalPath = destinationPath.Replace(".cshtml", ".html");
            File.WriteAllText(finalPath, result, System.Text.Encoding.UTF8);
        }

        private string GetFileDestinationPath(string fullPath)
        {
            var destination = fullPath.Replace(this.basePath, this.sitePath);
            var destinationFolder = Path.GetDirectoryName(destination);
            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }
            return destination;
        }

        private bool IsSiteContent(string fullPath)
        {
            if (fullPath.StartsWith(this.templatesPath)) return false;
            if (fullPath.StartsWith(this.sitePath)) return false;
            if (fullPath.StartsWith(this.postsPath)) return false;
            return true;
        }

        private bool IsIgnored(string fullPath)
        {
            return Configuration.Ignore.Any(fullPath.EndsWith);
        }

        public void LoadPosts(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                var post = Post.FromFile(file);
                if (post == null) continue;

                Logger.Current.Log(LoggingLevel.Debug, "Loading post: {0}", file);
                post.LoadFile(Configuration);
                post.ExecuteTransformationPipeline(this.basePath, Configuration);

                this.posts.Add(post);
            }

            this.siteContext.Posts = this.posts.OrderBy(p => p.Date).ToList().AsReadOnly();
        }

        public void WriteAllPosts(string root, string outputPath)
        {
            foreach (var post in posts)
            {
                var permalink = post.Permalink.Substring(1);
                var folderPath = Path.Combine(this.sitePath, permalink);
                var filePath = Path.Combine(folderPath, "index.html");

                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                File.WriteAllText(filePath, post.Body);
            }
        }
    }
}