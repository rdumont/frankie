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