using System.Collections.ObjectModel;

namespace RDumont.Frankie.Core
{
    public class SiteContext
    {
        private static SiteContext instance;

        public ReadOnlyCollection<Post> Posts { get; internal set; }

        public static SiteContext Current
        {
            get { return instance ?? (instance = new SiteContext()); }
        }

        protected SiteContext()
        {
        }
    }
}