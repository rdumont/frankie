using RDumont.Frankie.Core;

namespace RDumont.Frankie.Tests
{
    public class TestableGenerator : Generator
    {
        public new Io Io
        {
            get { return base.Io; }
            set { base.Io = value; }
        }

        public new string BasePath
        {
            get { return base.BasePath; }
            set { base.BasePath = value; }
        }

        public new string SitePath
        {
            get { return base.SitePath; }
            set { base.SitePath = value; }
        }

        public new SiteConfiguration Configuration
        {
            get { return base.Configuration; }
            set { base.Configuration = value; }
        }

        public new string GetFileDestinationPath(string fullPath)
        {
            return base.GetFileDestinationPath(fullPath);
        }
    }
}