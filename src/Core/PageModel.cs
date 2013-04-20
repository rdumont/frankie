using System.IO;

namespace RDumont.Frankie.Core
{
    public class Page : ContentFile
    {
        private readonly string absoluteFilePath;

        public Page(string absoluteFilePath)
        {
            this.absoluteFilePath = absoluteFilePath;
        }

        public void LoadFile(SiteConfiguration configuration)
        {
            var contents = File.ReadAllText(this.absoluteFilePath);
            this.Body = contents;

            ExtractMetadata();
        }
    }
}