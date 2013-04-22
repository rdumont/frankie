namespace RDumont.Frankie.Core
{
    public class Page : ContentFile
    {
        private readonly string _absoluteFilePath;
        private readonly Io _io;

        public Page(string absoluteFilePath)
        {
            this._absoluteFilePath = absoluteFilePath;
            this._io = new Io();
        }

        protected Page()
        {
        }

        public void LoadFile(SiteConfiguration configuration)
        {
            var contents = _io.ReadFile(_absoluteFilePath); ;
            this.Body = contents;

            ExtractMetadata();
        }
    }
}