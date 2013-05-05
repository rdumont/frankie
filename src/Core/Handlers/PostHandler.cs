using System.Collections.Generic;
using System.IO;

namespace RDumont.Frankie.Core.Handlers
{
    public class PostHandler : IAssetHandler
    {
        private readonly SiteConfiguration _configuration;
        private readonly Io _io;
        private readonly List<Post> _posts;
        private bool _postsAreDirty;

        public PostHandler(SiteConfiguration configuration, Io io)
        {
            _configuration = configuration;
            _io = io;
            _posts = new List<Post>();
        }

        public bool Matches(string path)
        {
            return path.StartsWith(Post.POSTS_FOLDER);
        }

        public void Handle(string path)
        {
            var post = LoadPost(path);
            WritePost(post);
        }

        public void LoadAllPosts(IEnumerable<string> paths, SiteContext siteContext)
        {
            foreach (var path in paths)
                LoadPost(path);
            UpdatePostsCollection(siteContext);
        }

        public Post LoadPost(string file)
        {
            var post = Post.FromFile(file);
            if (post == null) return null;

            Logger.Current.Log(LoggingLevel.Debug, "Loading post: {0}", file);
            post.LoadFile(_configuration);
            try
            {
                post.ExecuteTransformationPipeline(_configuration);
                _posts.RemoveAll(p => p.Slug == post.Slug && p.Date == post.Date);
                _posts.Add(post);
            }
            catch (TemplateNotFoundException exception)
            {
                Logger.Current.LogError(exception.Message);
            }
            _postsAreDirty = true;
            return post;
        }

        public void UpdatePostsCollection(SiteContext siteContext)
        {
            if (_postsAreDirty)
                siteContext.UpdatePostsCollection(_posts);
            _postsAreDirty = false;
        }

        public void WriteAllPosts()
        {
            foreach (var post in _posts)
                WritePost(post);
        }

        private void WritePost(Post post)
        {
            var permalink = post.Permalink.Substring(1);
            var folderPath = Path.Combine(_configuration.SitePath, permalink);
            var filePath = Path.Combine(folderPath, "index.html");

            if (!_io.DirectoryExists(folderPath))
                _io.CreateDirectory(folderPath);
            _io.WriteFile(filePath, post.Body);
        }
    }
}