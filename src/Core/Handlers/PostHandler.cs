using System.Collections.Generic;
using System.IO;

namespace RDumont.Frankie.Core.Handlers
{
    public class PostHandler : IAssetHandler
    {
        private readonly Generator _generator;
        private readonly List<Post> _posts;
        private bool _postsAreDirty;

        public PostHandler(Generator generator)
        {
            _generator = generator;
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
            post.LoadFile(_generator.Configuration);
            try
            {
                post.ExecuteTransformationPipeline(_generator.Configuration);
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
            {
                WritePost(post);
            }
        }

        private void WritePost(Post post)
        {
            var permalink = post.Permalink.Substring(1);
            var folderPath = Path.Combine(_generator.SitePath, permalink);
            var filePath = Path.Combine(folderPath, "index.html");

            if (!_generator.Io.DirectoryExists(folderPath))
                _generator.Io.CreateDirectory(folderPath);
            _generator.Io.WriteFile(filePath, post.Body);
        }
    }
}