using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            this.posts = new List<Post>();
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
            var destination = fullPath.Replace(this.basePath, this.sitePath);
            var destinationFolder = Path.GetDirectoryName(destination);
            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            if (fullPath.EndsWith(".cshtml"))
            {
                var contents = File.ReadAllText(fullPath, System.Text.Encoding.UTF8);

                var model = new Page {Foo = "bar"};
                var result = Razor.Parse(contents, model);

                var finalPath = destination.Replace(".cshtml", ".html");
                File.WriteAllText(finalPath, result, System.Text.Encoding.UTF8);
            }
            else
            {
                var finalPath = fullPath.Replace(this.basePath, this.sitePath);
                File.Copy(fullPath, finalPath, true);
            }
        }

        public void LoadPosts(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                var post = Post.FromFile(file);
                if (post == null) continue;

                Logger.Current.Log(LoggingLevel.Debug, "Loading post: {0}", file);
                post.LoadFile(Configuration);
                post.ExecuteTransformationPipeline(Configuration);

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