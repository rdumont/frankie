using System.Collections.Specialized;
using RDumont.Frankie.Core;

namespace RDumont.Frankie.Tests
{
    public class TestablePage : Page
    {
        public new NameValueCollection Metadata
        {
            get { return base.Metadata; }
            set { base.Metadata = value; }
        }
    }
}