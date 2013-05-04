namespace RDumont.Frankie.Core.Handlers
{
    public class PostHandler : IAssetHandler
    {
        private Generator _generator;

        public PostHandler(Generator generator)
        {
            _generator = generator;
        }

        public bool Matches(string path)
        {
            return path.StartsWith(Post.POSTS_FOLDER);
        }

        public void Handle(string path)
        {
            var post = _generator.LoadSinglePost(path);
            _generator.WritePost(post);
        }
    }
}