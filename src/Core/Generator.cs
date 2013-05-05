using RDumont.Frankie.Core.Handlers;

namespace RDumont.Frankie.Core
{
    public class Generator
    {
        private readonly SiteContext siteContext;
        private AssetHandlerManager _handlers;

        public Io Io { get; set; }
        public SiteConfiguration Configuration { get; set; }

        protected Generator()
        {
        }

        public Generator(SiteContext siteContext)
        {
            this.siteContext = siteContext;
        }
        
        public void Init(AssetHandlerManager handlers, SiteConfiguration configuration, Io io)
        {
            _handlers = handlers;
            Configuration = configuration;
            Io = io;

            TemplateManager.SetTemplateManager(new LiquidTemplateManager());
            TemplateManager.Current.Init(configuration.SourcePath);
        }
    }
}