namespace RDumont.Frankie.Core
{
    public class TemplatePage : ContentFile
    {
        public TemplatePage(string body)
        {
            this.Body = body;

            ExtractMetadata();
        }
    }
}