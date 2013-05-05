using System.Linq;

namespace RDumont.Frankie.Core.Handlers
{
    public class AssetHandlerManager
    {
        private readonly Generator _generator;

        public TemplateHandler TemplateHandler { get; private set; }
        public PostHandler PostHandler { get; private set; }
        public GeneratedContentHandler GeneratedContentHandler { get; private set; }
        public MarkdownPageHandler MarkdownPageHandler { get; private set; }
        public TransformableContentHandler TransformableContentHandler { get; private set; }
        public StaticContentHandler StaticContentHandler { get; private set; }

        private readonly IAssetHandler[] _allHandlers;

        public AssetHandlerManager(Generator generator)
        {
            _generator = generator;
            TemplateHandler = new TemplateHandler(generator.Configuration, generator.Io, this);
            PostHandler = new PostHandler(generator.Configuration, generator.Io);
            GeneratedContentHandler = new GeneratedContentHandler(generator.Configuration);
            MarkdownPageHandler = new MarkdownPageHandler(_generator.Configuration, _generator.Io);
            TransformableContentHandler = new TransformableContentHandler(_generator.Configuration, _generator.Io);
            StaticContentHandler = new StaticContentHandler(_generator.Configuration, _generator.Io);

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
            if (_generator.Configuration.IsExcluded(path))
                return;

            var handler = FindMatchingHandler(path);
            if (handler != null) handler.Handle(path);
        }
    }
}