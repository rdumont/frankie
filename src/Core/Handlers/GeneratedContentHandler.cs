namespace RDumont.Frankie.Core.Handlers
{
    public class GeneratedContentHandler : IAssetHandler
    {
        private readonly SiteConfiguration _configuration;

        public GeneratedContentHandler(SiteConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool Matches(string path)
        {
            return path.StartsWith(_configuration.GetRelativePath(_configuration.SitePath));
        }

        public void Handle(string path)
        {
            // don't touch generated files
        }

        public void HandleRemoval(string path)
        {
            // don't touch generated files
        }
    }
}