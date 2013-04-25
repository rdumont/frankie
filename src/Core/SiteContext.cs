using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RDumont.Frankie.Core
{
    public class SiteContext
    {
        private static SiteContext instance;

        public ReadOnlyCollection<Post> Posts { get; private set; }

        public static SiteContext Current
        {
            get { return instance ?? (instance = new SiteContext()); }
        }

        protected SiteContext()
        {
        }

        internal void UpdatePostsCollection(IEnumerable<Post> posts)
        {
            Posts = posts.OrderBy(p => p.Date).ToList().AsReadOnly();
        }
    }
}