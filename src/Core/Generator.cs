using System;
using System.Collections.Generic;
using Path = System.IO.Path;
using System.Linq;

namespace RDumont.Frankie.Core
{
    public class Generator
    {
        protected string BasePath;
        protected string PostsPath;
        protected string TemplatesPath;
        protected string SitePath;
        private List<Post> posts;
        private readonly SiteContext siteContext;

        protected Io Io { get; set; }
        protected SiteConfiguration Configuration { get; set; }

        protected Generator()
        {
        }

        public Generator(SiteContext siteContext)
        {
            this.Io = new Io();
            this.siteContext = siteContext;
        }
        
        public void Init(string locationPath, string outputPath)
        {
            this.BasePath = locationPath.TrimEnd(Path.DirectorySeparatorChar);
            this.PostsPath = Path.Combine(locationPath, "_posts");
            this.TemplatesPath = Path.Combine(locationPath, "_templates");
            this.SitePath = outputPath.TrimEnd(Path.DirectorySeparatorChar);

            var configPath = Path.Combine(locationPath, "config.yaml");
            this.Configuration = SiteConfiguration.Load(configPath);

            TemplateManager.SetTemplateManager(new LiquidTemplateManager());

            TemplateManager.Current.Init(this.BasePath);

            this.posts = new List<Post>();
        }

        public void CompileTemplates(string root)
        {
            var allFiles = Io.FindFilesRecursively(TemplatesPath, "*.html");
            foreach (var file in allFiles)
            {
                CompileTemplate(file);
            }
        }

        private void HandleTemplateChange(string file)
        {
            var path = file.Remove(0, BasePath.Length + 1);
            var dependentFiles = DependencyTracker.Current.FindAllDependentFiles(path);

            foreach (var dependentFile in dependentFiles)
            {
                ReAddDependentFile(dependentFile);
            }

            CompileTemplate(file);
        }

        private void ReAddDependentFile(string file)
        {
            var fullPath = Path.Combine(BasePath, file);
            if (file.StartsWith(TemplateManager.TEMPLATES_FOLDER))
            {
                CompileTemplate(fullPath);
            }
            else if (file.StartsWith(Post.POSTS_FOLDER))
            {
                // handle post
            }
            else
            {
                AddFile(fullPath);
            }
        }

        private void CompileTemplate(string file)
        {
            var name = file.Remove(0, TemplatesPath.Length + 1).Replace(".html", "");
            TemplateManager.Current.CompileTemplate(file.Remove(0, BasePath.Length + 1));

            Logger.Current.Log(LoggingLevel.Debug, "Compiled template: {0}", name);
        }

        public void RemoveFile(string fullPath)
        {
            var destination = GetFileDestinationPath(fullPath);
            Io.DeleteFile(destination);
            Logger.Current.Log(LoggingLevel.Debug, "Removed file: {0}", GetRelativePath(fullPath));
        }

        private string GetRelativePath(string fullPath)
        {
            return fullPath.Remove(0, BasePath.Length + 1);
        }

        public void AddFile(string fullPath)
        {
            if (IsIgnored(fullPath)) return;

            if (IsTemplate(fullPath))
            {
                HandleTemplateChange(fullPath);
                return;
            }

            if (!IsSiteContent(fullPath)) return;

            var destination = GetFileDestinationPath(fullPath);
            EnsureDirectoryExists(destination);
            var relativeOrigin = fullPath.Remove(0, this.BasePath.Length + 1);

            if (fullPath.EndsWith(".html"))
            {
                HandleHtmlPage(fullPath, destination);
                Logger.Current.Log(LoggingLevel.Debug, "HTML page: {0}", relativeOrigin);
            }
            else if (fullPath.EndsWith(".md"))
            {
                HandleMarkdownPage(fullPath, destination);
                Logger.Current.Log(LoggingLevel.Debug, "Markdown page: {0}", relativeOrigin);
            }
            else
            {
                Io.CopyFile(fullPath, destination, true);
                Logger.Current.Log(LoggingLevel.Debug, "Asset: {0}", relativeOrigin);
            }
        }

        private bool IsTemplate(string fullPath)
        {
            return fullPath.StartsWith(this.TemplatesPath);
        }

        private void HandleMarkdownPage(string originPath, string destinationPath)
        {
            var page = new Page(originPath);
            page.LoadFile(Configuration);

            var template = page.Metadata["template"] ?? "_page";

            try
            {
                page.Body = TemplateManager.Current.RenderMarkdownPage(originPath, template, page);
            }
            catch (InvalidOperationException exception)
            {
                if (!exception.Message.StartsWith("No template exists")) throw;

                Logger.Current.LogError("{0}\n  No template exists with name '{1}'",
                    originPath, template);
            }

            Io.WriteFile(destinationPath, page.Body);
        }

        private void HandleHtmlPage(string originPath, string destinationPath)
        {
            var model = new Page(originPath);
            model.LoadFile(Configuration);

            var result = TemplateManager.Current.RenderPage(originPath.Remove(0, BasePath.Length + 1), model);

            Io.WriteFile(destinationPath, result);
        }

        protected string GetFileDestinationPath(string fullPath)
        {
            if (fullPath.StartsWith(this.PostsPath))
            {
                var post = Post.FromFile(fullPath);
                var relativePath = post.GetDestinationFilePath(this.Configuration);
                return Path.GetFullPath(Path.Combine(this.SitePath, relativePath));
            }

            var destination = fullPath.Replace(this.BasePath, this.SitePath);

            if (destination.EndsWith(".md"))
                destination = destination.Replace(".md", ".html");

            return destination;
        }

        protected void EnsureDirectoryExists(string fullPath)
        {
            var destinationFolder = Path.GetDirectoryName(fullPath);
            if (!Io.DirectoryExists(destinationFolder))
            {
                Io.CreateDirectory(destinationFolder);
            }
        }

        private bool IsSiteContent(string fullPath)
        {
            if (fullPath.StartsWith(this.TemplatesPath)) return false;
            if (fullPath.StartsWith(this.SitePath)) return false;
            if (fullPath.StartsWith(this.PostsPath)) return false;
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
                try
                {
                    post.ExecuteTransformationPipeline(this.BasePath, Configuration);
                    this.posts.Add(post);
                }
                catch (TemplateNotFoundException exception)
                {
                    Logger.Current.LogError(exception.Message);
                }
            }

            this.siteContext.Posts = this.posts.OrderBy(p => p.Date).ToList().AsReadOnly();
        }

        public void WriteAllPosts(string root, string outputPath)
        {
            foreach (var post in posts)
            {
                var permalink = post.Permalink.Substring(1);
                var folderPath = Path.Combine(this.SitePath, permalink);
                var filePath = Path.Combine(folderPath, "index.html");

                if (!Io.DirectoryExists(folderPath)) Io.CreateDirectory(folderPath);
                Io.WriteFile(filePath, post.Body);
            }
        }
    }
}