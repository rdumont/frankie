namespace RDumont.Frankie.Core.Handlers
{
    public interface IAssetHandler
    {
        bool Matches(string path);
        void Handle(string path);
        void HandleRemoval(string path);
    }
}