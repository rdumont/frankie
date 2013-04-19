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
            TrackDependency(name);
            return base.ResolveLayout(name);
        }

        public override TemplateWriter Include(string cacheName, object model = null)
        {
            TrackDependency(cacheName);
            return base.Include(cacheName, model);
        }

        private void TrackDependency(string name)
        {
            var thisTemplatePath = ViewBag.PagePath ?? ((RazorTemplateManager)TemplateManager.Current).GetTemplatePath(this.GetType());
            if (thisTemplatePath != null)
            {
                var layoutFilePath = TemplateManager.GetFullPath(name);
                DependencyTracker.Current.Add(thisTemplatePath, layoutFilePath);
            }
        }
    }
}