using RazorEngine.Templating;

namespace RDumont.Frankie.Core
{
    public class PageTemplate<T> : TemplateBase<T>
    {
        private readonly SiteContext siteContext;

        public SiteContext Site { get { return this.siteContext; } }

        public PageTemplate() : this(SiteContext.Current)
        {
        }

        public PageTemplate(SiteContext siteContext)
        {
            this.siteContext = siteContext;
        }
    }
}