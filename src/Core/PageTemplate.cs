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

        protected override ITemplate ResolveLayout(string name)
        {
            var thisTemplatePath = TemplateManager.GetTemplatePath(this.GetType());
            if (thisTemplatePath != null)
            {
                var layoutFilePath = TemplateManager.GetFullPath(name);
                DependencyTracker.Current.Add(thisTemplatePath, layoutFilePath);
            }

            return base.ResolveLayout(name);
        }

        public override TemplateWriter Include(string cacheName, object model = null)
        {
            var thisTemplatePath = TemplateManager.GetTemplatePath(this.GetType());
            if (thisTemplatePath != null)
            {
                var layoutFilePath = TemplateManager.GetFullPath(cacheName);
                DependencyTracker.Current.Add(thisTemplatePath, layoutFilePath);
            }

            return base.Include(cacheName, model);
        }
    }
}