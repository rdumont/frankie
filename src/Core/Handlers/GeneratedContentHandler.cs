namespace RDumont.Frankie.Core.Handlers
{
    public class GeneratedContentHandler : IAssetHandler
    {
        private readonly Generator _generator;

        public GeneratedContentHandler(Generator generator)
        {
            _generator = generator;
        }

        public bool Matches(string path)
        {
            return path.StartsWith(_generator.RelativeSitePath);
        }

        public void Handle(string path)
        {
        }
    }
}