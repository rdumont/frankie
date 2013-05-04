using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
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
        private bool _postsAreDirty;

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
            Configuration = SiteConfiguration.Load(configPath);

            if (Configuration.Culture != null)
            {
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture
                    = CultureInfo.GetCultureInfo(Configuration.Culture);
            }

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

        public void LoadPosts(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                LoadSinglePost(file);
            }

            siteContext.UpdatePostsCollection(posts);
            _postsAreDirty = false;
        }

        private Post LoadSinglePost(string file)
        {
            var post = Post.FromFile(file);
            if (post == null) return null;

            Logger.Current.Log(LoggingLevel.Debug, "Loading post: {0}", file);
            post.LoadFile(Configuration);
            try
            {
                post.ExecuteTransformationPipeline(this.BasePath, Configuration);
                posts.RemoveAll(p => p.Slug == post.Slug && p.Date == post.Date);
                posts.Add(post);
            }
            catch (TemplateNotFoundException exception)
            {
                Logger.Current.LogError(exception.Message);
            }
            _postsAreDirty = true;
            return post;
        }

        public void WriteAllPosts(string root, string outputPath)
        {
            foreach (var post in posts)
            {
                WritePost(post);
            }
        }

        private void WritePost(Post post)
        {
            var permalink = post.Permalink.Substring(1);
            var folderPath = Path.Combine(this.SitePath, permalink);
            var filePath = Path.Combine(folderPath, "index.html");

            if (!Io.DirectoryExists(folderPath)) Io.CreateDirectory(folderPath);
            Io.WriteFile(filePath, post.Body);
        }

        public void AddFile(string fullPath)
        {
            var path = GetRelativePath(fullPath);

            if (Configuration.IsExcluded(path))
                return;

            if (IsTemplate(path))
                HandleTemplateChange(fullPath);

            else if (IsPost(path))
                HandlePostChange(fullPath);

            else if (IsGeneratedContent(path))
                return;

            else if (IsMarkdown(path))
                HandleMarkdownPage(path);

            else if (IsTransformableContent(path))
                HandleHtmlPage(path);

            else HandleContentFile(path);
        }

        private void HandleContentFile(string relativePath)
        {
            var destination = Path.Combine(this.SitePath, relativePath);
            try
            {
                Io.CopyFile(destination, destination, true);
                Logger.Current.Log(LoggingLevel.Debug, "Asset: {0}", relativePath);
            }
            catch (FileNotFoundException)
            {
                // ok, probably a temp file
            }
        }

        private bool IsTransformableContent(string relativePath)
        {
            return relativePath.EndsWith(".html");
        }

        private bool IsMarkdown(string relativePath)
        {
            return relativePath.EndsWith(".md");
        }

        private bool IsPost(string relativePath)
        {
            return relativePath.StartsWith(Post.POSTS_FOLDER);
        }

        private bool IsTemplate(string relativePath)
        {
            return relativePath.StartsWith(TemplateManager.TEMPLATES_FOLDER);
        }

        private bool IsGeneratedContent(string relativePath)
        {
            return relativePath.StartsWith(this.SitePath);
        }

        public void RemoveFile(string fullPath)
        {
            var relativeOrigin = GetRelativePath(fullPath);
            if (Configuration.IsExcluded(relativeOrigin)) return;

            if (IsGeneratedContent(fullPath)) return;

            var destination = GetFileDestinationPath(fullPath);
            try
            {
                Io.DeleteFile(destination);
                Logger.Current.Log(LoggingLevel.Debug, "Removed file: {0}", GetRelativePath(fullPath));
            }
            catch (FileNotFoundException)
            {
                // ok, probably a temp file
            }
        }

        private void HandlePostChange(string file)
        {
            var post = LoadSinglePost(file);
            WritePost(post);
        }

        private void HandleTemplateChange(string file)
        {
            var path = GetRelativePath(file);
            var dependentFiles = DependencyTracker.Current.FindAllDependentFiles(path);

            try
            {
                CompileTemplate(file);
            }
            catch (FileNotFoundException)
            {
                // ok, probably a temp file
            }

            foreach (var dependentFile in dependentFiles)
            {
                ReAddDependentFile(dependentFile);
            }

            if (_postsAreDirty)
            {
                siteContext.UpdatePostsCollection(posts);
                _postsAreDirty = false;
            }
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
                var post = LoadSinglePost(fullPath);
                WritePost(post);
            }
            else
            {
                AddFile(fullPath);
            }
        }

        private void CompileTemplate(string file)
        {
            var name = file.Remove(0, TemplatesPath.Length + 1).Replace(".html", "");
            TemplateManager.Current.CompileTemplate(GetRelativePath(file));

            Logger.Current.Log(LoggingLevel.Debug, "Compiled template: {0}", name);
        }

        private void HandleMarkdownPage(string relativePath)
        {
            var page = new Page(Path.Combine(this.BasePath, relativePath));
            var destination = Path.Combine(this.SitePath, relativePath.Replace(".md", ".html"));
            EnsureDirectoryExists(destination);
            page.LoadFile(Configuration);

            var template = page.Metadata["template"] ?? "_page";

            try
            {
                page.Body = TemplateManager.Current.RenderMarkdownPage(relativePath, template, page);
            }
            catch (InvalidOperationException exception)
            {
                if (!exception.Message.StartsWith("No template exists")) throw;

                Logger.Current.LogError("{0}\n  No template exists with name '{1}'",
                    relativePath, template);
            }

            Io.WriteFile(destination, page.Body);
            Logger.Current.Log(LoggingLevel.Debug, "Markdown page: {0}", relativePath);
        }

        private void HandleHtmlPage(string relativePath)
        {
            var destination = Path.Combine(this.SitePath, relativePath);
            EnsureDirectoryExists(destination);

            var model = new Page(Path.Combine(this.SitePath, relativePath));
            model.LoadFile(Configuration);

            var result = TemplateManager.Current.RenderPage(GetRelativePath(relativePath), model);

            Io.WriteFile(destination, result);
            Logger.Current.Log(LoggingLevel.Debug, "HTML page: {0}", relativePath);
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

        private string GetRelativePath(string fullPath)
        {
            return fullPath.Remove(0, BasePath.Length + 1);
        }
    }
}