using System.Linq;

namespace RDumont.Frankie.Core.Handlers
{
    public class AssetHandlerManager
    {
        private readonly SiteConfiguration _configuration;
        private readonly Io _io;

        public TemplateHandler TemplateHandler { get; private set; }
        public PostHandler PostHandler { get; private set; }
        public GeneratedContentHandler GeneratedContentHandler { get; private set; }
        public MarkdownPageHandler MarkdownPageHandler { get; private set; }
        public TransformableContentHandler TransformableContentHandler { get; private set; }
        public StaticContentHandler StaticContentHandler { get; private set; }

        private readonly IAssetHandler[] _allHandlers;

        public AssetHandlerManager(SiteConfiguration configuration, Io io)
        {
            _configuration = configuration;
            _io = io;
            TemplateHandler = new TemplateHandler(configuration, io, this);
            PostHandler = new PostHandler(configuration, io);
            GeneratedContentHandler = new GeneratedContentHandler(configuration);
            MarkdownPageHandler = new MarkdownPageHandler(configuration, io);
            TransformableContentHandler = new TransformableContentHandler(configuration, io);
            StaticContentHandler = new StaticContentHandler(configuration, io);

            _allHandlers = new IAssetHandler[]
                {
                    TemplateHandler,
                    PostHandler,
                    GeneratedContentHandler,
                    MarkdownPageHandler,
                    TransformableContentHandler,
                    StaticContentHandler
                };
        }

        public IAssetHandler FindMatchingHandler(string path)
        {
            return _allHandlers.FirstOrDefault(handler => handler.Matches(path));
        }

        public void Handle(string path)
        {
            if (_configuration.IsExcluded(path))
                return;

            var handler = FindMatchingHandler(path);
            if (handler != null) handler.Handle(path);
        }
    }
}