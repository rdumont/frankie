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

            TemplateManager.Init();

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
            var name = file.Remove(0, TemplatesPath.Length + 1).Replace(".cshtml", "");
            var contents = Io.ReadFile(file, 5);
            TemplateManager.CompileTemplate(file.Remove(0, BasePath.Length + 1), contents);

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
                Io.CopyFile(fullPath, destination, true);
                Logger.Current.Log(LoggingLevel.Debug, "Content: {0}", relativeOrigin);
            }
        }

        private bool IsTemplate(string fullPath)
        {
            return fullPath.StartsWith(this.TemplatesPath);
        }

        private void HandleMarkdownPage(string originPath, string destinationPath)
        {
            // TODO
        }

        private void HandleRazorPage(string originPath, string destinationPath)
        {
            var contents = Io.ReadFile(originPath, 5);

            var model = new Page();
            var result = TemplateManager.RenderPage(originPath.Remove(0, BasePath.Length + 1), contents, model);

            var finalPath = destinationPath.Replace(".cshtml", ".html");
            Io.WriteFile(finalPath, result);
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
                post.ExecuteTransformationPipeline(this.BasePath, Configuration);

                this.posts.Add(post);
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