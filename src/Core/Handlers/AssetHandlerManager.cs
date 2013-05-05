namespace RDumont.Frankie.Core.Handlers
{
    public class AssetHandlerManager
    {
        public TemplateHandler TemplateHandler { get; private set; }
        public PostHandler PostHandler { get; private set; }
        public GeneratedContentHandler GeneratedContentHandler { get; private set; }
        public MarkdownPageHandler MarkdownPageHandler { get; private set; }
        public TransformableContentHandler TransformableContentHandler { get; private set; }

        public IAssetHandler[] AllHandlers { get; private set; }

        public AssetHandlerManager(Generator generator)
        {
            TemplateHandler = new TemplateHandler(generator); 
            PostHandler = new PostHandler(generator);
            GeneratedContentHandler = new GeneratedContentHandler(generator);
            MarkdownPageHandler = new MarkdownPageHandler(generator);
            TransformableContentHandler = new TransformableContentHandler(generator);

            AllHandlers = new IAssetHandler[]
                {
                    TemplateHandler,
                    PostHandler,
                    GeneratedContentHandler,
                    MarkdownPageHandler,
                    TransformableContentHandler
                };
        }
    }
}