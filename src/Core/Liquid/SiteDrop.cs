using DotLiquid;

namespace RDumont.Frankie.Core.Liquid
{
    public class SiteDrop : Drop
    {
        public string Url { get; private set; }

        public SiteDrop(SiteConfiguration configuration)
        {
            Url = configuration.Url;
        }
    }
}